using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NWayland.Protocols.Wayland;
using NWayland.Interop;
#nullable enable
// <auto-generated/>
namespace NWayland.Protocols.XdgDecorationUnstableV1
{
    /// <summary>
    /// This interface allows a compositor to announce support for server-sidedecorations.<br/><br/>
    /// A window decoration is a set of window controls as deemed appropriate bythe party managing them, such as user interface components used to move,resize and change a window's state.<br/><br/>
    /// A client can use this protocol to request being decorated by a supportingcompositor.<br/><br/>
    /// If compositor and client do not negotiate the use of a server-sidedecoration using this protocol, clients continue to self-decorate as theysee fit.<br/><br/>
    /// Warning! The protocol described in this file is experimental andbackward incompatible changes may be made. Backward compatible changesmay be added together with the corresponding interface version bump.Backward incompatible changes are done by bumping the version number inthe protocol and interface names and resetting the interface version.Once the protocol is to be declared stable, the 'z' prefix and theversion number in the protocol and interface names are removed and theinterface version number is reset.<br/><br/>
    /// </summary>
    public sealed unsafe partial class ZxdgDecorationManagerV1 : WlProxy
    {
        [FixedAddressValueType]
        public static WlInterface WlInterface;

        static ZxdgDecorationManagerV1()
        {
            NWayland.Protocols.XdgDecorationUnstableV1.ZxdgDecorationManagerV1.WlInterface = new WlInterface("zxdg_decoration_manager_v1", 1, new WlMessage[] {
                new WlMessage("destroy", "", new WlInterface*[] { }),
                new WlMessage("get_toplevel_decoration", "no", new WlInterface*[] { WlInterface.GeneratorAddressOf(ref NWayland.Protocols.XdgDecorationUnstableV1.ZxdgToplevelDecorationV1.WlInterface), WlInterface.GeneratorAddressOf(ref NWayland.Protocols.XdgShell.XdgToplevel.WlInterface) })
            }, new WlMessage[] { });
        }

        protected override WlInterface* GetWlInterface()
        {
            return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.XdgDecorationUnstableV1.ZxdgDecorationManagerV1.WlInterface);
        }

        protected override void Dispose(bool disposing)
        {
            WlArgument* __args = stackalloc WlArgument[] {
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 0, __args);
            base.Dispose(true);
        }

        /// <summary>
        /// Create a new decoration object associated with the given toplevel.<br/><br/>
        /// Creating an xdg_toplevel_decoration from an xdg_toplevel which has abuffer attached or committed is a client error, and any attempts by aclient to attach or manipulate a buffer prior to the firstxdg_toplevel_decoration.configure event must also be treated aserrors.<br/><br/>
        /// </summary>
        public NWayland.Protocols.XdgDecorationUnstableV1.ZxdgToplevelDecorationV1 GetToplevelDecoration(NWayland.Protocols.XdgShell.XdgToplevel @toplevel)
        {
            if (@toplevel == null)
                throw new ArgumentNullException("toplevel");
            WlArgument* __args = stackalloc WlArgument[] {
                WlArgument.NewId,
                @toplevel
            };
            var __ret = LibWayland.wl_proxy_marshal_array_constructor_versioned(this.Handle, 1, __args, ref NWayland.Protocols.XdgDecorationUnstableV1.ZxdgToplevelDecorationV1.WlInterface, (uint)this.Version);
            return __ret == IntPtr.Zero ? null : new NWayland.Protocols.XdgDecorationUnstableV1.ZxdgToplevelDecorationV1(__ret, Version);
        }

        public interface IEvents
        {
        }

        public IEvents? Events { get; set; }

        protected override void DispatchEvent(uint opcode, WlArgument* arguments)
        {
        }

        private class ProxyFactory : IBindFactory<ZxdgDecorationManagerV1>
        {
            public WlInterface* GetInterface()
            {
                return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.XdgDecorationUnstableV1.ZxdgDecorationManagerV1.WlInterface);
            }

            public ZxdgDecorationManagerV1 Create(IntPtr handle, int version)
            {
                return new ZxdgDecorationManagerV1(handle, version);
            }
        }

        public static IBindFactory<ZxdgDecorationManagerV1> BindFactory { get; } = new ProxyFactory();

        public const string InterfaceName = "zxdg_decoration_manager_v1";
        public const int InterfaceVersion = 1;

        public ZxdgDecorationManagerV1(IntPtr handle, int version) : base(handle, version)
        {
        }
    }

    /// <summary>
    /// The decoration object allows the compositor to toggle server-side windowdecorations for a toplevel surface. The client can request to switch toanother mode.<br/><br/>
    /// The xdg_toplevel_decoration object must be destroyed before itsxdg_toplevel.<br/><br/>
    /// </summary>
    public sealed unsafe partial class ZxdgToplevelDecorationV1 : WlProxy
    {
        [FixedAddressValueType]
        public static WlInterface WlInterface;

        static ZxdgToplevelDecorationV1()
        {
            NWayland.Protocols.XdgDecorationUnstableV1.ZxdgToplevelDecorationV1.WlInterface = new WlInterface("zxdg_toplevel_decoration_v1", 1, new WlMessage[] {
                new WlMessage("destroy", "", new WlInterface*[] { }),
                new WlMessage("set_mode", "u", new WlInterface*[] { null }),
                new WlMessage("unset_mode", "", new WlInterface*[] { })
            }, new WlMessage[] {
                new WlMessage("configure", "u", new WlInterface*[] { null })
            });
        }

        protected override WlInterface* GetWlInterface()
        {
            return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.XdgDecorationUnstableV1.ZxdgToplevelDecorationV1.WlInterface);
        }

        protected override void Dispose(bool disposing)
        {
            WlArgument* __args = stackalloc WlArgument[] {
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 0, __args);
            base.Dispose(true);
        }

        /// <summary>
        /// Set the toplevel surface decoration mode. This informs the compositorthat the client prefers the provided decoration mode.<br/><br/>
        /// After requesting a decoration mode, the compositor will respond byemitting an xdg_surface.configure event. The client should then updateits content, drawing it without decorations if the received mode isserver-side decorations. The client must also acknowledge the configurewhen committing the new content (see xdg_surface.ack_configure).<br/><br/>
        /// The compositor can decide not to use the client's mode and enforce adifferent mode instead.<br/><br/>
        /// Clients whose decoration mode depend on the xdg_toplevel state may senda set_mode request in response to an xdg_surface.configure event and waitfor the next xdg_surface.configure event to prevent unwanted state.Such clients are responsible for preventing configure loops and mustmake sure not to send multiple successive set_mode requests with thesame decoration mode.<br/><br/>
        /// </summary>
        public void SetMode(ModeEnum @mode)
        {
            WlArgument* __args = stackalloc WlArgument[] {
                (uint)@mode
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 1, __args);
        }

        /// <summary>
        /// Unset the toplevel surface decoration mode. This informs the compositorthat the client doesn't prefer a particular decoration mode.<br/><br/>
        /// This request has the same semantics as set_mode.<br/><br/>
        /// </summary>
        public void UnsetMode()
        {
            WlArgument* __args = stackalloc WlArgument[] {
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 2, __args);
        }

        public interface IEvents
        {
            /// <summary>
            /// The configure event asks the client to change its decoration mode. Theconfigured state should not be applied immediately. Clients must send anack_configure in response to this event. See xdg_surface.configure andxdg_surface.ack_configure for details.<br/><br/>
            /// A configure event can be sent at any time. The specified mode must beobeyed by the client.<br/><br/>
            /// </summary>
            void OnConfigure(NWayland.Protocols.XdgDecorationUnstableV1.ZxdgToplevelDecorationV1 eventSender, ModeEnum @mode);
        }

        public IEvents? Events { get; set; }

        protected override void DispatchEvent(uint opcode, WlArgument* arguments)
        {
            switch (opcode)
            {
                case 0:
                    Events?.OnConfigure(this, (ModeEnum)arguments[0].UInt32);
                    break;
            }
        }

        public enum ErrorEnum
        {
            /// <summary>
            /// xdg_toplevel has a buffer attached before configure<br/><br/>
            /// </summary>
            UnconfiguredBuffer = 0,
            /// <summary>
            /// xdg_toplevel already has a decoration object<br/><br/>
            /// </summary>
            AlreadyConstructed = 1,
            /// <summary>
            /// xdg_toplevel destroyed before the decoration object<br/><br/>
            /// </summary>
            Orphaned = 2
        }

        /// <summary>
        /// These values describe window decoration modes.<br/><br/>
        /// </summary>
        public enum ModeEnum
        {
            /// <summary>
            /// no server-side window decoration<br/><br/>
            /// </summary>
            ClientSide = 1,
            /// <summary>
            /// server-side window decoration<br/><br/>
            /// </summary>
            ServerSide = 2
        }

        private class ProxyFactory : IBindFactory<ZxdgToplevelDecorationV1>
        {
            public WlInterface* GetInterface()
            {
                return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.XdgDecorationUnstableV1.ZxdgToplevelDecorationV1.WlInterface);
            }

            public ZxdgToplevelDecorationV1 Create(IntPtr handle, int version)
            {
                return new ZxdgToplevelDecorationV1(handle, version);
            }
        }

        public static IBindFactory<ZxdgToplevelDecorationV1> BindFactory { get; } = new ProxyFactory();

        public const string InterfaceName = "zxdg_toplevel_decoration_v1";
        public const int InterfaceVersion = 1;

        public ZxdgToplevelDecorationV1(IntPtr handle, int version) : base(handle, version)
        {
        }
    }
}