using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NWayland.Protocols.Wayland;
using NWayland.Interop;
#nullable enable
// <auto-generated/>
namespace NWayland.Protocols.Viewporter
{
    /// <summary>
    /// The global interface exposing surface cropping and scalingcapabilities is used to instantiate an interface extension for awl_surface object. This extended interface will then allowcropping and scaling the surface contents, effectivelydisconnecting the direct relationship between the buffer and thesurface size.<br/><br/>
    /// </summary>
    public sealed unsafe partial class WpViewporter : WlProxy
    {
        [FixedAddressValueType]
        public static WlInterface WlInterface;

        static WpViewporter()
        {
            NWayland.Protocols.Viewporter.WpViewporter.WlInterface = new WlInterface("wp_viewporter", 1, new WlMessage[] {
                new WlMessage("destroy", "", new WlInterface*[] { }),
                new WlMessage("get_viewport", "no", new WlInterface*[] { WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Viewporter.WpViewport.WlInterface), WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Wayland.WlSurface.WlInterface) })
            }, new WlMessage[] { });
        }

        protected override WlInterface* GetWlInterface()
        {
            return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Viewporter.WpViewporter.WlInterface);
        }

        protected override void Dispose(bool disposing)
        {
            WlArgument* __args = stackalloc WlArgument[] {
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 0, __args);
            base.Dispose(true);
        }

        /// <summary>
        /// Instantiate an interface extension for the given wl_surface tocrop and scale its content. If the given wl_surface already hasa wp_viewport object associated, the viewport_existsprotocol error is raised.<br/><br/>
        /// </summary>
        public NWayland.Protocols.Viewporter.WpViewport GetViewport(NWayland.Protocols.Wayland.WlSurface @surface)
        {
            if (@surface == null)
                throw new ArgumentNullException("surface");
            WlArgument* __args = stackalloc WlArgument[] {
                WlArgument.NewId,
                @surface
            };
            var __ret = LibWayland.wl_proxy_marshal_array_constructor_versioned(this.Handle, 1, __args, ref NWayland.Protocols.Viewporter.WpViewport.WlInterface, (uint)this.Version);
            return __ret == IntPtr.Zero ? null : new NWayland.Protocols.Viewporter.WpViewport(__ret, Version);
        }

        public interface IEvents
        {
        }

        public IEvents? Events { get; set; }

        protected override void DispatchEvent(uint opcode, WlArgument* arguments)
        {
        }

        public enum ErrorEnum
        {
            /// <summary>
            /// the surface already has a viewport object associated<br/><br/>
            /// </summary>
            ViewportExists = 0
        }

        private class ProxyFactory : IBindFactory<WpViewporter>
        {
            public WlInterface* GetInterface()
            {
                return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Viewporter.WpViewporter.WlInterface);
            }

            public WpViewporter Create(IntPtr handle, int version)
            {
                return new WpViewporter(handle, version);
            }
        }

        public static IBindFactory<WpViewporter> BindFactory { get; } = new ProxyFactory();

        public const string InterfaceName = "wp_viewporter";
        public const int InterfaceVersion = 1;

        public WpViewporter(IntPtr handle, int version) : base(handle, version)
        {
        }
    }

    /// <summary>
    /// An additional interface to a wl_surface object, which allows theclient to specify the cropping and scaling of the surfacecontents.<br/><br/>
    /// This interface works with two concepts: the source rectangle (src_x,src_y, src_width, src_height), and the destination size (dst_width,dst_height). The contents of the source rectangle are scaled to thedestination size, and content outside the source rectangle is ignored.This state is double-buffered, and is applied on the nextwl_surface.commit.<br/><br/>
    /// The two parts of crop and scale state are independent: the sourcerectangle, and the destination size. Initially both are unset, thatis, no scaling is applied. The whole of the current wl_buffer isused as the source, and the surface size is as defined inwl_surface.attach.<br/><br/>
    /// If the destination size is set, it causes the surface size to becomedst_width, dst_height. The source (rectangle) is scaled to exactlythis size. This overrides whatever the attached wl_buffer size is,unless the wl_buffer is NULL. If the wl_buffer is NULL, the surfacehas no content and therefore no size. Otherwise, the size is alwaysat least 1x1 in surface local coordinates.<br/><br/>
    /// If the source rectangle is set, it defines what area of the wl_buffer istaken as the source. If the source rectangle is set and the destinationsize is not set, then src_width and src_height must be integers, and thesurface size becomes the source rectangle size. This results in croppingwithout scaling. If src_width or src_height are not integers anddestination size is not set, the bad_size protocol error is raised whenthe surface state is applied.<br/><br/>
    /// The coordinate transformations from buffer pixel coordinates up tothe surface-local coordinates happen in the following order:1. buffer_transform (wl_surface.set_buffer_transform)2. buffer_scale (wl_surface.set_buffer_scale)3. crop and scale (wp_viewport.set*)This means, that the source rectangle coordinates of crop and scaleare given in the coordinates after the buffer transform and scale,i.e. in the coordinates that would be the surface-local coordinatesif the crop and scale was not applied.<br/><br/>
    /// If src_x or src_y are negative, the bad_value protocol error is raised.Otherwise, if the source rectangle is partially or completely outside ofthe non-NULL wl_buffer, then the out_of_buffer protocol error is raisedwhen the surface state is applied. A NULL wl_buffer does not raise theout_of_buffer error.<br/><br/>
    /// If the wl_surface associated with the wp_viewport is destroyed,all wp_viewport requests except 'destroy' raise the protocol errorno_surface.<br/><br/>
    /// If the wp_viewport object is destroyed, the crop and scalestate is removed from the wl_surface. The change will be appliedon the next wl_surface.commit.<br/><br/>
    /// </summary>
    public sealed unsafe partial class WpViewport : WlProxy
    {
        [FixedAddressValueType]
        public static WlInterface WlInterface;

        static WpViewport()
        {
            NWayland.Protocols.Viewporter.WpViewport.WlInterface = new WlInterface("wp_viewport", 1, new WlMessage[] {
                new WlMessage("destroy", "", new WlInterface*[] { }),
                new WlMessage("set_source", "ffff", new WlInterface*[] { null, null, null, null }),
                new WlMessage("set_destination", "ii", new WlInterface*[] { null, null })
            }, new WlMessage[] { });
        }

        protected override WlInterface* GetWlInterface()
        {
            return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Viewporter.WpViewport.WlInterface);
        }

        protected override void Dispose(bool disposing)
        {
            WlArgument* __args = stackalloc WlArgument[] {
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 0, __args);
            base.Dispose(true);
        }

        /// <summary>
        /// Set the source rectangle of the associated wl_surface. Seewp_viewport for the description, and relation to the wl_buffersize.<br/><br/>
        /// If all of x, y, width and height are -1.0, the source rectangle isunset instead. Any other set of values where width or height are zeroor negative, or x or y are negative, raise the bad_value protocolerror.<br/><br/>
        /// The crop and scale state is double-buffered state, and will beapplied on the next wl_surface.commit.<br/><br/>
        /// </summary>
        public void SetSource(WlFixed @x, WlFixed @y, WlFixed @width, WlFixed @height)
        {
            WlArgument* __args = stackalloc WlArgument[] {
                @x,
                @y,
                @width,
                @height
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 1, __args);
        }

        /// <summary>
        /// Set the destination size of the associated wl_surface. Seewp_viewport for the description, and relation to the wl_buffersize.<br/><br/>
        /// If width is -1 and height is -1, the destination size is unsetinstead. Any other pair of values for width and height thatcontains zero or negative values raises the bad_value protocolerror.<br/><br/>
        /// The crop and scale state is double-buffered state, and will beapplied on the next wl_surface.commit.<br/><br/>
        /// </summary>
        public void SetDestination(int @width, int @height)
        {
            WlArgument* __args = stackalloc WlArgument[] {
                @width,
                @height
            };
            LibWayland.wl_proxy_marshal_array(this.Handle, 2, __args);
        }

        public interface IEvents
        {
        }

        public IEvents? Events { get; set; }

        protected override void DispatchEvent(uint opcode, WlArgument* arguments)
        {
        }

        public enum ErrorEnum
        {
            /// <summary>
            /// negative or zero values in width or height<br/><br/>
            /// </summary>
            BadValue = 0,
            /// <summary>
            /// destination size is not integer<br/><br/>
            /// </summary>
            BadSize = 1,
            /// <summary>
            /// source rectangle extends outside of the content area<br/><br/>
            /// </summary>
            OutOfBuffer = 2,
            /// <summary>
            /// the wl_surface was destroyed<br/><br/>
            /// </summary>
            NoSurface = 3
        }

        private class ProxyFactory : IBindFactory<WpViewport>
        {
            public WlInterface* GetInterface()
            {
                return WlInterface.GeneratorAddressOf(ref NWayland.Protocols.Viewporter.WpViewport.WlInterface);
            }

            public WpViewport Create(IntPtr handle, int version)
            {
                return new WpViewport(handle, version);
            }
        }

        public static IBindFactory<WpViewport> BindFactory { get; } = new ProxyFactory();

        public const string InterfaceName = "wp_viewport";
        public const int InterfaceVersion = 1;

        public WpViewport(IntPtr handle, int version) : base(handle, version)
        {
        }
    }
}