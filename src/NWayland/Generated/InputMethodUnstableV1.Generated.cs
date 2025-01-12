using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NWayland.Protocols.Wayland;
using NWayland.Interop;
#nullable enable
// <auto-generated/>
namespace NWayland.Protocols.InputMethodUnstableV1
{
    /// <summary>
    /// Corresponds to a text input on the input method side. An input method contextis created on text input activation on the input method side. It allowsreceiving information about the text input from the application via events.Input method contexts do not keep state after deactivation and should bedestroyed after deactivation is handled.<br/><br/>
    /// Text is generally UTF-8 encoded, indices and lengths are in bytes.<br/><br/>
    /// Serials are used to synchronize the state between the text input andan input method. New serials are sent by the text input in thecommit_state request and are used by the input method to indicatethe known text input state in events like preedit_string, commit_string,and keysym. The text input can then ignore events from the input methodwhich are based on an outdated state (for example after a reset).<br/><br/>
    /// Warning! The protocol described in this file is experimental andbackward incompatible changes may be made. Backward compatible changesmay be added together with the corresponding interface version bump.Backward incompatible changes are done by bumping the version number inthe protocol and interface names and resetting the interface version.Once the protocol is to be declared stable, the 'z' prefix and theversion number in the protocol and interface names are removed and theinterface version number is reset.<br/><br/>
    /// </summary>
    public sealed unsafe partial class ZwpInputMethodContextV1 : WlProxy
    {
        [FixedAddressValueType]
        public static WlInterface WlInterface;

        static ZwpInputMethodContextV1()
        {
            NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1.WlInterface = new WlInterface("zwp_input_method_context_v1", 1, new WlMessage[] {
                new WlMessage("destroy", "", new WlInterface*[] { }),
                new WlMessage("commit_string", "us", new WlInterface*[] { null, null }),
                new WlMessage("preedit_string", "uss", new WlInterface*[] { null, null, null }),
                new WlMessage("preedit_styling", "uuu", new WlInterface*[] { null, null, null }),
                new WlMessage("preedit_cursor", "i", new WlInterface*[] { null }),
                new WlMessage("delete_surrounding_text", "iu", new WlInterface*[] { null, null }),
                new WlMessage("cursor_position", "ii", new WlInterface*[] { null, null }),
                new WlMessage("modifiers_map", "a", new WlInterface*[] { null }),
                new WlMessage("keysym", "uuuuu", new WlInterface*[] { null, null, null, null, null }),
                new WlMessage("grab_keyboard", "n", new WlInterface*[] { WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Wayland.WlKeyboard.WlInterface) }),
                new WlMessage("key", "uuuu", new WlInterface*[] { null, null, null, null }),
                new WlMessage("modifiers", "uuuuu", new WlInterface*[] { null, null, null, null, null }),
                new WlMessage("language", "us", new WlInterface*[] { null, null }),
                new WlMessage("text_direction", "uu", new WlInterface*[] { null, null })
            }, new WlMessage[] {
                new WlMessage("surrounding_text", "suu", new WlInterface*[] { null, null, null }),
                new WlMessage("reset", "", new WlInterface*[] { }),
                new WlMessage("content_type", "uu", new WlInterface*[] { null, null }),
                new WlMessage("invoke_action", "uu", new WlInterface*[] { null, null }),
                new WlMessage("commit_state", "u", new WlInterface*[] { null }),
                new WlMessage("preferred_language", "s", new WlInterface*[] { null })
            });
        }

        protected override WlInterface* GetWlInterface()
        {
            return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1.WlInterface);
        }

        protected override void Dispose(bool disposing)
        {
            WlArgument* __args = stackalloc WlArgument[] {
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 0, __args);
            base.Dispose(true);
        }

        /// <summary>
        /// Send the commit string text for insertion to the application.<br/><br/>
        /// The text to commit could be either just a single character after a keypress or the result of some composing (pre-edit). It could be also anempty text when some text should be removed (seedelete_surrounding_text) or when the input cursor should be moved (seecursor_position).<br/><br/>
        /// Any previously set composing text will be removed.<br/><br/>
        /// </summary>
        public void CommitString(uint @serial, string @text)
        {
            if (@text == null)
                throw new ArgumentNullException("text");
            using var __marshalled__text = new NWaylandMarshalledString(@text);
            WlArgument* __args = stackalloc WlArgument[] {
                @serial,
                __marshalled__text
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 1, __args);
        }

        /// <summary>
        /// Send the pre-edit string text to the application text input.<br/><br/>
        /// The commit text can be used to replace the pre-edit text on reset (forexample on unfocus).<br/><br/>
        /// Previously sent preedit_style and preedit_cursor requests are alsoprocessed by the text_input.<br/><br/>
        /// </summary>
        public void PreeditString(uint @serial, string @text, string @commit)
        {
            if (@commit == null)
                throw new ArgumentNullException("commit");
            if (@text == null)
                throw new ArgumentNullException("text");
            using var __marshalled__text = new NWaylandMarshalledString(@text);
            using var __marshalled__commit = new NWaylandMarshalledString(@commit);
            WlArgument* __args = stackalloc WlArgument[] {
                @serial,
                __marshalled__text,
                __marshalled__commit
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 2, __args);
        }

        /// <summary>
        /// Set the styling information on composing text. The style is applied forlength in bytes from index relative to the beginning ofthe composing text (as byte offset). Multiple styles canbe applied to a composing text.<br/><br/>
        /// This request should be sent before sending a preedit_string request.<br/><br/>
        /// </summary>
        public void PreeditStyling(uint @index, uint @length, uint @style)
        {
            WlArgument* __args = stackalloc WlArgument[] {
                @index,
                @length,
                @style
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 3, __args);
        }

        /// <summary>
        /// Set the cursor position inside the composing text (as byte offset)relative to the start of the composing text.<br/><br/>
        /// When index is negative no cursor should be displayed.<br/><br/>
        /// This request should be sent before sending a preedit_string request.<br/><br/>
        /// </summary>
        public void PreeditCursor(int @index)
        {
            WlArgument* __args = stackalloc WlArgument[] {
                @index
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 4, __args);
        }

        /// <summary>
        /// Remove the surrounding text.<br/><br/>
        /// This request will be handled on the text_input side directly followinga commit_string request.<br/><br/>
        /// </summary>
        public void DeleteSurroundingText(int @index, uint @length)
        {
            WlArgument* __args = stackalloc WlArgument[] {
                @index,
                @length
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 5, __args);
        }

        /// <summary>
        /// Set the cursor and anchor to a new position. Index is the new cursorposition in bytes (when &gt;= 0 this is relative to the end of the inserted text,otherwise it is relative to the beginning of the inserted text). Anchor isthe new anchor position in bytes (when &gt;= 0 this is relative to the end of theinserted text, otherwise it is relative to the beginning of the insertedtext). When there should be no selected text, anchor should be the sameas index.<br/><br/>
        /// This request will be handled on the text_input side directly followinga commit_string request.<br/><br/>
        /// </summary>
        public void CursorPosition(int @index, int @anchor)
        {
            WlArgument* __args = stackalloc WlArgument[] {
                @index,
                @anchor
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 6, __args);
        }

        public void ModifiersMap(ReadOnlySpan<byte> @map)
        {
            fixed (byte* __pointer__map = @map)
            {
                var __marshalled__map = WlArray.FromPointer(__pointer__map, @map.Length);
                WlArgument* __args = stackalloc WlArgument[] {
                    &__marshalled__map
                };
                LibWayland.wl_proxy_marshal_array(this.Handle, 7, __args);
            }
        }

        /// <summary>
        /// Notify when a key event was sent. Key events should not be used fornormal text input operations, which should be done with commit_string,delete_surrounding_text, etc. The key event follows the wl_keyboard keyevent convention. Sym is an XKB keysym, state is a wl_keyboard key_state.<br/><br/>
        /// </summary>
        public void Keysym(uint @serial, uint @time, uint @sym, uint @state, uint @modifiers)
        {
            WlArgument* __args = stackalloc WlArgument[] {
                @serial,
                @time,
                @sym,
                @state,
                @modifiers
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 8, __args);
        }

        /// <summary>
        /// Allow an input method to receive hardware keyboard input and processkey events to generate text events (with pre-edit) over the wire. Thisallows input methods which compose multiple key events for inputtingtext like it is done for CJK languages.<br/><br/>
        /// </summary>
        public NWayland.Protocols.Wayland.WlKeyboard GrabKeyboard()
        {
            WlArgument* __args = stackalloc WlArgument[] {
                WlArgument.NewId
            };
            var __ret = LibWayland.wl_proxy_marshal_array_constructor_versioned(this.Handle, 9, __args, ref NWayland.Protocols.Wayland.WlKeyboard.WlInterface, (uint)this.Version);
            return __ret == IntPtr.Zero ? null : new NWayland.Protocols.Wayland.WlKeyboard(__ret, Version);
        }

        /// <summary>
        /// Forward a wl_keyboard::key event to the client that was not processedby the input method itself. Should be used when filtering key eventswith grab_keyboard.  The arguments should be the ones from thewl_keyboard::key event.<br/><br/>
        /// For generating custom key events use the keysym request instead.<br/><br/>
        /// </summary>
        public void Key(uint @serial, uint @time, uint @key, uint @state)
        {
            WlArgument* __args = stackalloc WlArgument[] {
                @serial,
                @time,
                @key,
                @state
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 10, __args);
        }

        /// <summary>
        /// Forward a wl_keyboard::modifiers event to the client that was notprocessed by the input method itself.  Should be used when filteringkey events with grab_keyboard. The arguments should be the onesfrom the wl_keyboard::modifiers event.<br/><br/>
        /// </summary>
        public void Modifiers(uint @serial, uint @modsDepressed, uint @modsLatched, uint @modsLocked, uint @group)
        {
            WlArgument* __args = stackalloc WlArgument[] {
                @serial,
                @modsDepressed,
                @modsLatched,
                @modsLocked,
                @group
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 11, __args);
        }

        public void Language(uint @serial, string @language)
        {
            if (@language == null)
                throw new ArgumentNullException("language");
            using var __marshalled__language = new NWaylandMarshalledString(@language);
            WlArgument* __args = stackalloc WlArgument[] {
                @serial,
                __marshalled__language
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 12, __args);
        }

        public void TextDirection(uint @serial, uint @direction)
        {
            WlArgument* __args = stackalloc WlArgument[] {
                @serial,
                @direction
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 13, __args);
        }

        public interface IEvents
        {
            /// <summary>
            /// The plain surrounding text around the input position. Cursor is theposition in bytes within the surrounding text relative to the beginningof the text. Anchor is the position in bytes of the selection anchorwithin the surrounding text relative to the beginning of the text. Ifthere is no selected text then anchor is the same as cursor.<br/><br/>
            /// </summary>
            void OnSurroundingText(NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1 eventSender, string @text, uint @cursor, uint @anchor);
            void OnReset(NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1 eventSender);
            void OnContentType(NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1 eventSender, uint @hint, uint @purpose);
            void OnInvokeAction(NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1 eventSender, uint @button, uint @index);
            void OnCommitState(NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1 eventSender, uint @serial);
            void OnPreferredLanguage(NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1 eventSender, string @language);
        }

        public IEvents? Events { get; set; }

        protected override void DispatchEvent(uint opcode, WlArgument* arguments)
        {
            switch (opcode)
            {
                case 0:
                    Events?.OnSurroundingText(this, Marshal.PtrToStringAnsi(arguments[0].IntPtr), arguments[1].UInt32, arguments[2].UInt32);
                    break;
                case 1:
                    Events?.OnReset(this);
                    break;
                case 2:
                    Events?.OnContentType(this, arguments[0].UInt32, arguments[1].UInt32);
                    break;
                case 3:
                    Events?.OnInvokeAction(this, arguments[0].UInt32, arguments[1].UInt32);
                    break;
                case 4:
                    Events?.OnCommitState(this, arguments[0].UInt32);
                    break;
                case 5:
                    Events?.OnPreferredLanguage(this, Marshal.PtrToStringAnsi(arguments[0].IntPtr));
                    break;
            }
        }

        private class ProxyFactory : IBindFactory<ZwpInputMethodContextV1>
        {
            public WlInterface* GetInterface()
            {
                return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1.WlInterface);
            }

            public ZwpInputMethodContextV1 Create(IntPtr handle, int version)
            {
                return new ZwpInputMethodContextV1(handle, version);
            }
        }

        public static IBindFactory<ZwpInputMethodContextV1> BindFactory { get; } = new ProxyFactory();

        public const string InterfaceName = "zwp_input_method_context_v1";
        public const int InterfaceVersion = 1;

        public ZwpInputMethodContextV1(IntPtr handle, int version) : base(handle, version)
        {
        }
    }

    /// <summary>
    /// An input method object is responsible for composing text in response toinput from hardware or virtual keyboards. There is one input methodobject per seat. On activate there is a new input method context objectcreated which allows the input method to communicate with the text input.<br/><br/>
    /// </summary>
    public sealed unsafe partial class ZwpInputMethodV1 : WlProxy
    {
        [FixedAddressValueType]
        public static WlInterface WlInterface;

        static ZwpInputMethodV1()
        {
            NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodV1.WlInterface = new WlInterface("zwp_input_method_v1", 1, new WlMessage[] { }, new WlMessage[] {
                new WlMessage("activate", "n", new WlInterface*[] { WlInterface.GeneratorAddressOf(ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1.WlInterface) }),
                new WlMessage("deactivate", "o", new WlInterface*[] { WlInterface.GeneratorAddressOf(ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1.WlInterface) })
            });
        }

        protected override WlInterface* GetWlInterface()
        {
            return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodV1.WlInterface);
        }

        public interface IEvents
        {
            /// <summary>
            /// A text input was activated. Creates an input method context objectwhich allows communication with the text input.<br/><br/>
            /// </summary>
            void OnActivate(NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodV1 eventSender, ZwpInputMethodContextV1 @id);

            /// <summary>
            /// The text input corresponding to the context argument was deactivated.The input method context should be destroyed after deactivation ishandled.<br/><br/>
            /// </summary>
            void OnDeactivate(NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodV1 eventSender, NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1 @context);
        }

        public IEvents? Events { get; set; }

        protected override void DispatchEvent(uint opcode, WlArgument* arguments)
        {
            switch (opcode)
            {
                case 0:
                    Events?.OnActivate(this, new ZwpInputMethodContextV1(arguments[0].IntPtr, Version));
                    break;
                case 1:
                    Events?.OnDeactivate(this, WlProxy.FromNative<NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodContextV1>(arguments[0].IntPtr));
                    break;
            }
        }

        private class ProxyFactory : IBindFactory<ZwpInputMethodV1>
        {
            public WlInterface* GetInterface()
            {
                return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputMethodV1.WlInterface);
            }

            public ZwpInputMethodV1 Create(IntPtr handle, int version)
            {
                return new ZwpInputMethodV1(handle, version);
            }
        }

        public static IBindFactory<ZwpInputMethodV1> BindFactory { get; } = new ProxyFactory();

        public const string InterfaceName = "zwp_input_method_v1";
        public const int InterfaceVersion = 1;

        public ZwpInputMethodV1(IntPtr handle, int version) : base(handle, version)
        {
        }
    }

    /// <summary>
    /// Only one client can bind this interface at a time.<br/><br/>
    /// </summary>
    public sealed unsafe partial class ZwpInputPanelV1 : WlProxy
    {
        [FixedAddressValueType]
        public static WlInterface WlInterface;

        static ZwpInputPanelV1()
        {
            NWayland.Protocols.InputMethodUnstableV1.ZwpInputPanelV1.WlInterface = new WlInterface("zwp_input_panel_v1", 1, new WlMessage[] {
                new WlMessage("get_input_panel_surface", "no", new WlInterface*[] { WlInterface.GeneratorAddressOf(ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputPanelSurfaceV1.WlInterface), WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Wayland.WlSurface.WlInterface) })
            }, new WlMessage[] { });
        }

        protected override WlInterface* GetWlInterface()
        {
            return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputPanelV1.WlInterface);
        }

        public NWayland.Protocols.InputMethodUnstableV1.ZwpInputPanelSurfaceV1 GetInputPanelSurface(NWayland.Protocols.Wayland.WlSurface @surface)
        {
            if (@surface == null)
                throw new ArgumentNullException("surface");
            WlArgument* __args = stackalloc WlArgument[] {
                WlArgument.NewId,
                @surface
            };
            var __ret = LibWayland.wl_proxy_marshal_array_constructor_versioned(this.Handle, 0, __args, ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputPanelSurfaceV1.WlInterface, (uint)this.Version);
            return __ret == IntPtr.Zero ? null : new NWayland.Protocols.InputMethodUnstableV1.ZwpInputPanelSurfaceV1(__ret, Version);
        }

        public interface IEvents
        {
        }

        public IEvents? Events { get; set; }

        protected override void DispatchEvent(uint opcode, WlArgument* arguments)
        {
        }

        private class ProxyFactory : IBindFactory<ZwpInputPanelV1>
        {
            public WlInterface* GetInterface()
            {
                return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputPanelV1.WlInterface);
            }

            public ZwpInputPanelV1 Create(IntPtr handle, int version)
            {
                return new ZwpInputPanelV1(handle, version);
            }
        }

        public static IBindFactory<ZwpInputPanelV1> BindFactory { get; } = new ProxyFactory();

        public const string InterfaceName = "zwp_input_panel_v1";
        public const int InterfaceVersion = 1;

        public ZwpInputPanelV1(IntPtr handle, int version) : base(handle, version)
        {
        }
    }

    public sealed unsafe partial class ZwpInputPanelSurfaceV1 : WlProxy
    {
        [FixedAddressValueType]
        public static WlInterface WlInterface;

        static ZwpInputPanelSurfaceV1()
        {
            NWayland.Protocols.InputMethodUnstableV1.ZwpInputPanelSurfaceV1.WlInterface = new WlInterface("zwp_input_panel_surface_v1", 1, new WlMessage[] {
                new WlMessage("set_toplevel", "ou", new WlInterface*[] { WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Wayland.WlOutput.WlInterface), null }),
                new WlMessage("set_overlay_panel", "", new WlInterface*[] { })
            }, new WlMessage[] { });
        }

        protected override WlInterface* GetWlInterface()
        {
            return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputPanelSurfaceV1.WlInterface);
        }

        /// <summary>
        /// Set the input_panel_surface type to keyboard.<br/><br/>
        /// A keyboard surface is only shown when a text input is active.<br/><br/>
        /// </summary>
        public void SetToplevel(NWayland.Protocols.Wayland.WlOutput @output, uint @position)
        {
            if (@output == null)
                throw new ArgumentNullException("output");
            WlArgument* __args = stackalloc WlArgument[] {
                @output,
                @position
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 0, __args);
        }

        /// <summary>
        /// Set the input_panel_surface to be an overlay panel.<br/><br/>
        /// This is shown near the input cursor above the application window whena text input is active.<br/><br/>
        /// </summary>
        public void SetOverlayPanel()
        {
            WlArgument* __args = stackalloc WlArgument[] {
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 1, __args);
        }

        public interface IEvents
        {
        }

        public IEvents? Events { get; set; }

        protected override void DispatchEvent(uint opcode, WlArgument* arguments)
        {
        }

        public enum PositionEnum
        {
            CenterBottom = 0
        }

        private class ProxyFactory : IBindFactory<ZwpInputPanelSurfaceV1>
        {
            public WlInterface* GetInterface()
            {
                return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.InputMethodUnstableV1.ZwpInputPanelSurfaceV1.WlInterface);
            }

            public ZwpInputPanelSurfaceV1 Create(IntPtr handle, int version)
            {
                return new ZwpInputPanelSurfaceV1(handle, version);
            }
        }

        public static IBindFactory<ZwpInputPanelSurfaceV1> BindFactory { get; } = new ProxyFactory();

        public const string InterfaceName = "zwp_input_panel_surface_v1";
        public const int InterfaceVersion = 1;

        public ZwpInputPanelSurfaceV1(IntPtr handle, int version) : base(handle, version)
        {
        }
    }
}