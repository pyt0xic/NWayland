using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.Platform;
using NWayland.Protocols.Wayland;

namespace Avalonia.Wayland
{
    internal class WlScreens : IScreenImpl, IDisposable
    {
        private readonly AvaloniaWaylandPlatform _platform;
        private readonly Dictionary<uint, WlScreen> _wlScreens = new();
        private readonly Dictionary<WlOutput, WlScreen> _wlOutputs = new();
        private readonly Dictionary<WlSurface, WlWindow> _wlWindows = new();
        private readonly List<WlScreen> _allScreens = new();

        public WlScreens(AvaloniaWaylandPlatform platform)
        {
            _platform = platform;
            platform.WlRegistryHandler.GlobalAdded += OnGlobalAdded;
            _platform.WlRegistryHandler.GlobalRemoved += OnGlobalRemoved;
        }

        public int ScreenCount => _allScreens.Count;

        public IReadOnlyList<Screen> AllScreens => _allScreens;

        public Screen? ScreenFromWindow(IWindowBaseImpl window) => ScreenHelper.ScreenFromWindow(window, AllScreens);

        public Screen? ScreenFromPoint(PixelPoint point) => ScreenHelper.ScreenFromPoint(point, AllScreens);

        public Screen? ScreenFromRect(PixelRect rect) => ScreenHelper.ScreenFromRect(rect, AllScreens);

        public void Dispose()
        {
            _platform.WlRegistryHandler.GlobalAdded -= OnGlobalAdded;
            _platform.WlRegistryHandler.GlobalRemoved -= OnGlobalRemoved;
            foreach (var wlScreen in _wlScreens.Values)
                wlScreen.Dispose();
        }

        internal WlScreen ScreenFromOutput(WlOutput wlOutput) => _wlOutputs[wlOutput];

        internal WlWindow? WindowFromSurface(WlSurface? wlSurface) => wlSurface is not null && _wlWindows.TryGetValue(wlSurface, out var wlWindow) ? wlWindow : null;

        internal void AddWindow(WlWindow window)
        {
            if (window.WlSurface != null)
            {
                _wlWindows.Add(window.WlSurface, window);
            }            
        }

        internal void RemoveWindow(WlWindow window)
        {
            _platform.WlInputDevice.InvalidateFocus(window);
            if (window.WlSurface != null)
            {
                _wlWindows.Remove(window.WlSurface);
            }            
        }

        private void OnGlobalAdded(WlRegistryHandler.GlobalInfo globalInfo)
        {
            if (globalInfo.Interface != WlOutput.InterfaceName)
                return;
            var wlOutput = _platform.WlRegistryHandler.BindRequiredInterface(WlOutput.BindFactory, WlOutput.InterfaceVersion, globalInfo);
            var wlScreen = new WlScreen(wlOutput);
            _wlScreens.Add(globalInfo.Name, wlScreen);
            _wlOutputs.Add(wlOutput, wlScreen);
            _allScreens.Add(wlScreen);
        }

        private void OnGlobalRemoved(WlRegistryHandler.GlobalInfo globalInfo)
        {
            if (globalInfo.Interface is not WlOutput.InterfaceName || !_wlScreens.TryGetValue(globalInfo.Name, out var wlScreen))
                return;
            _wlScreens.Remove(globalInfo.Name);
            _wlOutputs.Remove(wlScreen.WlOutput);
            _allScreens.Remove(wlScreen);
            wlScreen.Dispose();
        }

        internal sealed class WlScreen : Screen, WlOutput.IEvents, IDisposable
        {
            private static readonly ConcurrentDictionary<string, FieldInfo> cache = new();

            public WlScreen(WlOutput wlOutput) : base(default, default, default, default(bool))
            {
                WlOutput = wlOutput;
                wlOutput.Events = this;
            }

            public WlOutput WlOutput { get; }

            public void OnGeometry(WlOutput eventSender, int x, int y, int physicalWidth, int physicalHeight, WlOutput.SubpixelEnum subpixel, string make, string model, WlOutput.TransformEnum transform)
            {
                var value = new PixelRect(x, y, Bounds.Width, Bounds.Height);
                SetProperty(nameof(Bounds), value);
            }

            private void SetProperty(string name, object value)
            {
                var prop = cache.GetOrAdd(name, (k) =>
                {
#pragma warning disable CS8603 // Possible null reference return.
                    return typeof(Screen).GetField($"<{name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
#pragma warning restore CS8603 // Possible null reference return.
                });
                prop.SetValue(this, value);
            }

            public void OnMode(WlOutput eventSender, WlOutput.ModeEnum flags, int width, int height, int refresh)
            {
                if (flags.HasAllFlags(WlOutput.ModeEnum.Current))
                {
                    var value = new PixelRect(Bounds.X, Bounds.Y, width, height);
                    SetProperty(nameof(Bounds), value);
                }
            }

            public void OnScale(WlOutput eventSender, int factor) => SetProperty(nameof(PixelDensity), factor);

            public void OnName(WlOutput eventSender, string name) { }

            public void OnDescription(WlOutput eventSender, string description) { }

            public void OnDone(WlOutput eventSender) { }

            public void Dispose() => WlOutput.Dispose();

            internal void SetBounds(int width, int height) => SetProperty(nameof(Bounds), new PixelRect(Bounds.X, Bounds.Y, width, height));
        }
    }
}
