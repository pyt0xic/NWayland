using System;
using System.Text;
using Avalonia.FreeDesktop;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.Wayland.FreeDesktop;
using NWayland.Protocols.Wayland;

namespace Avalonia.Wayland
{
    internal class WlKeyboardHandler : WlKeyboard.IEvents, IDisposable
    {
        private readonly AvaloniaWaylandPlatform _platform;
        private readonly IPlatformThreadingInterface _platformThreading;
        private readonly WlInputDevice _wlInputDevice;
        private readonly WlKeyboard _wlKeyboard;
        private readonly IntPtr _xkbContext;

        private WlWindow? _window;

        private IntPtr _xkbKeymap;
        private IntPtr _xkbState;
        private IntPtr _xkbComposeState;

        private TimeSpan _repeatDelay;
        private TimeSpan _repeatInterval;
        private bool _firstRepeat;
        private uint _repeatTime;
        private uint _repeatCode;
        private XkbKey _repeatSym;
        private Key _repeatKey;
        private IDisposable? _keyboardTimer;

        private int _ctrlMask;
        private int _altMask;
        private int _shiftMask;
        private int _metaMask;

        public WlKeyboardHandler(AvaloniaWaylandPlatform platform, WlInputDevice wlInputDevice)
        {
            _platform = platform;
            _wlInputDevice = wlInputDevice;
            _platformThreading = AvaloniaLocator.Current.GetRequiredService<IPlatformThreadingInterface>();
            _wlKeyboard = platform.WlSeat.GetKeyboard();
            _wlKeyboard.Events = this;
            _xkbContext = LibXkbCommon.xkb_context_new(0);
            KeyboardDevice = AvaloniaLocator.Current.GetRequiredService<IKeyboardDevice>();
        }

        public IKeyboardDevice KeyboardDevice { get; }

        public uint KeyboardEnterSerial { get; private set; }

        public void OnKeymap(WlKeyboard eventSender, WlKeyboard.KeymapFormatEnum format, int fd, uint size)
        {
            var map = LibC.mmap(IntPtr.Zero, new IntPtr(size), MemoryProtection.PROT_READ, SharingType.MAP_PRIVATE, fd, IntPtr.Zero);
            if (map == new IntPtr(-1))
            {
                LibC.close(fd);
                return;
            }

            var keymap = LibXkbCommon.xkb_keymap_new_from_string(_xkbContext, map, (uint)format, 0);
            LibC.munmap(map, new IntPtr(size));
            LibC.close(fd);

            if (keymap == IntPtr.Zero)
                return;

            var state = LibXkbCommon.xkb_state_new(keymap);
            if (state == IntPtr.Zero)
            {
                LibXkbCommon.xkb_keymap_unref(keymap);
                return;
            }

            var locale = Environment.GetEnvironmentVariable("LC_ALL")
                         ?? Environment.GetEnvironmentVariable("LC_CTYPE")
                         ?? Environment.GetEnvironmentVariable("LANG")
                         ?? "C";

            var composeTable = LibXkbCommon.xkb_compose_table_new_from_locale(_xkbContext, locale, 0);
            if (composeTable != IntPtr.Zero)
            {
                var composeState = LibXkbCommon.xkb_compose_state_new(composeTable, 0);
                LibXkbCommon.xkb_compose_table_unref(composeTable);
                if (composeState != IntPtr.Zero)
                    _xkbComposeState = composeState;
            }

            LibXkbCommon.xkb_keymap_unref(_xkbKeymap);
            LibXkbCommon.xkb_state_unref(_xkbState);

            _xkbKeymap = keymap;
            _xkbState = state;

            _ctrlMask = 1 << (int)LibXkbCommon.xkb_keymap_mod_get_index(keymap, "Control");
            _altMask = 1 << (int)LibXkbCommon.xkb_keymap_mod_get_index(keymap, "Mod1");
            _shiftMask = 1 << (int)LibXkbCommon.xkb_keymap_mod_get_index(keymap, "Shift");
            _metaMask = 1 << (int)LibXkbCommon.xkb_keymap_mod_get_index(keymap, "Mod4");
        }

        public void OnEnter(WlKeyboard eventSender, uint serial, WlSurface surface, ReadOnlySpan<int> keys)
        {
            _window = _platform.WlScreens.WindowFromSurface(surface);
            _wlInputDevice.Serial = serial;
            KeyboardEnterSerial = serial;
        }

        public void OnLeave(WlKeyboard eventSender, uint serial, WlSurface surface)
        {
            _wlInputDevice.Serial = serial;
            var window = _platform.WlScreens.WindowFromSurface(surface);
            if (window == _window)
                _window = null;
        }

        public void OnKey(WlKeyboard eventSender, uint serial, uint time, uint key, WlKeyboard.KeyStateEnum state)
        {
            _wlInputDevice.Serial = serial;
            if (_window?.InputRoot is null)
                return;
            var code = key + 8;
            var sym = LibXkbCommon.xkb_state_key_get_one_sym(_xkbState, code);
            var avaloniaKey = XkbKeyTransform.ConvertKey(sym);
            var eventType = state == WlKeyboard.KeyStateEnum.Pressed ? RawKeyEventType.KeyDown : RawKeyEventType.KeyUp;
            var keyEventArgs = new RawKeyEventArgs(KeyboardDevice, time, _window.InputRoot, eventType, avaloniaKey, _wlInputDevice.RawInputModifiers);
            _window.Input?.Invoke(keyEventArgs);

            if (state == WlKeyboard.KeyStateEnum.Pressed)
            {
                _wlInputDevice.UserActionDownSerial = serial;
                var text = GetComposedString(sym, code);
                if (text is not null)
                {
                    var textEventArgs = new RawTextInputEventArgs(KeyboardDevice, time, _window.InputRoot, text);
                    _window.Input?.Invoke(textEventArgs);
                }

                if (LibXkbCommon.xkb_keymap_key_repeats(_xkbKeymap, code) && _repeatInterval > TimeSpan.Zero)
                {
                    _keyboardTimer?.Dispose();
                    _repeatTime = time;
                    _repeatCode = code;
                    _repeatSym = sym;
                    _repeatKey = avaloniaKey;
                    _firstRepeat = true;
                    _keyboardTimer = _platformThreading.StartTimer(DispatcherPriority.Input, _repeatDelay, OnRepeatKey);
                }
            }
            else if (_repeatKey == avaloniaKey)
            {
                _keyboardTimer?.Dispose();
                _keyboardTimer = null;
            }
        }

        public void OnModifiers(WlKeyboard eventSender, uint serial, uint modsDepressed, uint modsLatched, uint modsLocked, uint group)
        {
            _wlInputDevice.Serial = serial;
            LibXkbCommon.xkb_state_update_mask(_xkbState, modsDepressed, modsLatched, modsLocked, 0, 0, group);
            var mask = LibXkbCommon.xkb_state_serialize_mods(_xkbState, LibXkbCommon.XkbStateComponent.XKB_STATE_MODS_EFFECTIVE);
            if ((mask & _ctrlMask) != 0)
                _wlInputDevice.RawInputModifiers |= RawInputModifiers.Control;
            else
                _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.Control;
            if ((mask & _altMask) != 0)
                _wlInputDevice.RawInputModifiers |= RawInputModifiers.Alt;
            else
                _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.Alt;
            if ((mask & _shiftMask) != 0)
                _wlInputDevice.RawInputModifiers |= RawInputModifiers.Shift;
            else
                _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.Shift;
            if ((mask & _metaMask) != 0)
                _wlInputDevice.RawInputModifiers |= RawInputModifiers.Meta;
            else
                _wlInputDevice.RawInputModifiers &= ~RawInputModifiers.Meta;
        }

        public void OnRepeatInfo(WlKeyboard eventSender, int rate, int delay)
        {
            _repeatDelay = TimeSpan.FromMilliseconds(delay);
            _repeatInterval = TimeSpan.FromSeconds(1d / rate);
        }

        public void Dispose()
        {
            if (_xkbContext != IntPtr.Zero)
                LibXkbCommon.xkb_context_unref(_xkbContext);
            _wlKeyboard.Dispose();
            _keyboardTimer?.Dispose();
        }

        internal void InvalidateFocus(WlWindow window)
        {
            if (_window == window)
                _window = null;
        }

        private void OnRepeatKey()
        {
            if (_window?.InputRoot is null)
                return;
            _window.Input?.Invoke(new RawKeyEventArgs(KeyboardDevice, _repeatTime, _window.InputRoot, RawKeyEventType.KeyDown, _repeatKey, _wlInputDevice.RawInputModifiers));
            var text = GetComposedString(_repeatSym, _repeatCode);
            if (text is not null)
                _window.Input?.Invoke( new RawTextInputEventArgs(KeyboardDevice, _repeatTime, _window.InputRoot, text));
            if (!_firstRepeat)
                return;
            _firstRepeat = false;
            _keyboardTimer?.Dispose();
            _keyboardTimer = _platformThreading.StartTimer(DispatcherPriority.Input, _repeatInterval, OnRepeatKey);
        }

        private unsafe string? GetComposedString(XkbKey sym, uint code)
        {
            LibXkbCommon.xkb_compose_state_feed(_xkbComposeState, sym);
            var status = LibXkbCommon.xkb_compose_state_get_status(_xkbComposeState);
            switch (status)
            {
                case LibXkbCommon.XkbComposeStatus.XKB_COMPOSE_COMPOSED:
                {
                    var size = LibXkbCommon.xkb_compose_state_get_utf8(_xkbComposeState, null, 0) + 1;
                    var buffer = stackalloc byte[size];
                    LibXkbCommon.xkb_compose_state_get_utf8(_xkbComposeState, buffer, size);
                    return Encoding.UTF8.GetString(buffer, size - 1);
                }
                case LibXkbCommon.XkbComposeStatus.XKB_COMPOSE_CANCELLED:
                {
                    LibXkbCommon.xkb_compose_state_reset(_xkbComposeState);
                    return null;
                }
                case LibXkbCommon.XkbComposeStatus.XKB_COMPOSE_NOTHING:
                {
                    var size = LibXkbCommon.xkb_state_key_get_utf8(_xkbState, code, null, 0) + 1;
                    var buffer = stackalloc byte[size];
                    LibXkbCommon.xkb_state_key_get_utf8(_xkbState, code, buffer, size);
                    var text = Encoding.UTF8.GetString(buffer, size - 1);
                    return text.Length == 1 && (text[0] < ' ' || text[0] == 0x7f) ? null : text; // Filer control codes or DEL
                }
                case LibXkbCommon.XkbComposeStatus.XKB_COMPOSE_COMPOSING:
                default:
                    return null;
            }
        }
    }
}
