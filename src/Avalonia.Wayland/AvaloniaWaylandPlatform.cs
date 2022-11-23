using System;
using System.Reflection;
using System.Xml.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.FreeDesktop;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Egl;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Wayland;
using NWayland.Protocols.PointerGesturesUnstableV1;
using NWayland.Protocols.TextInputUnstableV3;
using NWayland.Protocols.Wayland;
using NWayland.Protocols.XdgActivationV1;
using NWayland.Protocols.XdgDecorationUnstableV1;
using NWayland.Protocols.XdgForeignUnstableV2;
using NWayland.Protocols.XdgShell;
using Wanhjor.ObjectInspector;

namespace Avalonia.Wayland
{
    interface IDuckDBusTrayIconImpl
    {        
        [Duck(Name = "IsActive", Kind = DuckKind.Property, Flags = DuckAttribute.AllFlags)]
        bool IsActive { get; }

        [Duck(Name = "IconConverterDelegate", Kind = DuckKind.Property, Flags = DuckAttribute.AllFlags)]
        Func<IWindowIconImpl?, uint[]>? IconConverterDelegate { get; set; }
    }

    class PlatformSettingsStub : IPlatformSettings
    {
        public Size DoubleClickSize { get; } = new Size(2, 2);
        public TimeSpan DoubleClickTime { get; } = TimeSpan.FromMilliseconds(500);
    }

    internal class AvaloniaWaylandPlatform : IWindowingPlatform, IDisposable, XdgWmBase.IEvents
    {
        public AvaloniaWaylandPlatform(WaylandPlatformOptions options)
        {
            Options = options;
            WlDisplay = WlDisplay.Connect();
            var registry = WlDisplay.GetRegistry();
            WlRegistryHandler = new WlRegistryHandler(registry);
            WlDisplay.Roundtrip();

            WlCompositor = WlRegistryHandler.BindRequiredInterface(WlCompositor.BindFactory, WlCompositor.InterfaceName, WlCompositor.InterfaceVersion);
            WlSeat = WlRegistryHandler.BindRequiredInterface(WlSeat.BindFactory, WlSeat.InterfaceName, WlSeat.InterfaceVersion);
            WlShm = WlRegistryHandler.BindRequiredInterface(WlShm.BindFactory, WlShm.InterfaceName, WlShm.InterfaceVersion);
            WlDataDeviceManager = WlRegistryHandler.BindRequiredInterface(WlDataDeviceManager.BindFactory, WlDataDeviceManager.InterfaceName, WlDataDeviceManager.InterfaceVersion);
            XdgWmBase = WlRegistryHandler.BindRequiredInterface(XdgWmBase.BindFactory, XdgWmBase.InterfaceName, XdgWmBase.InterfaceVersion);
            XdgActivation = WlRegistryHandler.BindRequiredInterface(XdgActivationV1.BindFactory, XdgActivationV1.InterfaceName, XdgActivationV1.InterfaceVersion);
            ZxdgDecorationManager = WlRegistryHandler.Bind(ZxdgDecorationManagerV1.BindFactory, ZxdgDecorationManagerV1.InterfaceName, ZxdgDecorationManagerV1.InterfaceVersion);
            ZxdgExporter = WlRegistryHandler.Bind(ZxdgExporterV2.BindFactory, ZxdgExporterV2.InterfaceName, ZxdgExporterV2.InterfaceVersion);
            ZwpTextInput = WlRegistryHandler.Bind(ZwpTextInputManagerV3.BindFactory, ZwpTextInputManagerV3.InterfaceName, ZwpTextInputManagerV3.InterfaceVersion);
            ZwpPointerGestures = WlRegistryHandler.Bind(ZwpPointerGesturesV1.BindFactory, ZwpPointerGesturesV1.InterfaceName, ZwpPointerGesturesV1.InterfaceVersion);

            XdgWmBase.Events = this;

            var wlDataHandler = new WlDataHandler(this);
            AvaloniaLocator.CurrentMutable
                .Bind<IWindowingPlatform>().ToConstant(this)
                .Bind<IPlatformThreadingInterface>().ToConstant(new WlPlatformThreading(this))
                .Bind<IRenderTimer>().ToConstant(new DefaultRenderTimer(60))
                .Bind<IRenderLoop>().ToConstant(new RenderLoop())
                .Bind<PlatformHotkeyConfiguration>().ToConstant(new PlatformHotkeyConfiguration(KeyModifiers.Control))
                .Bind<IPlatformSettings>().ToConstant(new PlatformSettingsStub())
                .Bind<IKeyboardDevice>().ToConstant(new KeyboardDevice())
                .Bind<ICursorFactory>().ToConstant(new WlCursorFactory(this))
                .Bind<IClipboard>().ToConstant(wlDataHandler)
                .Bind<IPlatformDragSource>().ToConstant(wlDataHandler)
                .Bind<IPlatformIconLoader>().ToConstant(new WlIconLoader())                
                .Bind<IMountedVolumeInfoProvider>().ToConstant(new LinuxMountedVolumeInfoProvider());
            // UseManaged file browse system dialog

            WlScreens = new WlScreens(this);
            WlInputDevice = new WlInputDevice(this);

            if (ZwpTextInput is not null)
                WlTextInputMethod = new WlTextInputMethod(this);

            WlDisplay.Roundtrip();

            DBusHelper.TryInitialize();

            IPlatformOpenGlInterface? gl = null;
            if (options.UseGpu)
            {
                const int EGL_PLATFORM_WAYLAND_KHR = 0x31D8;
                gl = EglPlatformOpenGlInterface.TryCreate(() => new EglDisplay(new EglInterface(), true, EGL_PLATFORM_WAYLAND_KHR, WlDisplay.Handle, null));
                if (gl is not null)
                    AvaloniaLocator.CurrentMutable
                        .Bind<IPlatformOpenGlInterface>().ToConstant(gl);
            }            
        }

        internal WaylandPlatformOptions Options { get; }        

        internal WlDisplay WlDisplay { get; }

        internal WlRegistryHandler WlRegistryHandler { get; }

        internal WlCompositor WlCompositor { get; }

        internal WlSeat WlSeat { get; }

        internal WlShm WlShm { get; }

        internal WlDataDeviceManager WlDataDeviceManager { get; }

        internal XdgWmBase XdgWmBase { get; }

        internal XdgActivationV1 XdgActivation { get; }

        internal ZxdgDecorationManagerV1? ZxdgDecorationManager { get; }

        internal ZxdgExporterV2? ZxdgExporter { get; }

        internal ZwpTextInputManagerV3? ZwpTextInput { get; }

        internal ZwpPointerGesturesV1? ZwpPointerGestures { get; }

        internal WlScreens WlScreens { get; }

        internal WlInputDevice WlInputDevice { get; }

        internal WlTextInputMethod? WlTextInputMethod { get; }

        public IWindowImpl CreateWindow() => new WlToplevel(this);

        public IWindowImpl CreateEmbeddableWindow() => throw new NotSupportedException();

        public ITrayIconImpl? CreateTrayIcon()
        {
            // Why the hell hide all this stuff?
            var type = typeof(DBusHelper).Assembly.GetType("Avalonia.FreeDesktop.DBusTrayIconImpl");
            if (type == null)
            {
                throw new NullReferenceException(nameof(type));
            }    
            var instance = Activator.CreateInstance(type);
            if (instance == null)
            {
                throw new NullReferenceException(nameof(instance));
            }
            var duckInstance = instance.DuckAs<IDuckDBusTrayIconImpl>();

            if (!duckInstance.IsActive)
                return null;
            duckInstance.IconConverterDelegate = static impl => impl is WlIconData wlIconData ? wlIconData.Data : Array.Empty<uint>();
            return instance as ITrayIconImpl;
        }

        public void OnPing(XdgWmBase eventSender, uint serial) => XdgWmBase.Pong(serial);

        public void Dispose()
        {
            WlTextInputMethod?.Dispose();
            ZxdgDecorationManager?.Dispose();
            ZxdgExporter?.Dispose();
            ZwpTextInput?.Dispose();
            ZwpPointerGestures?.Dispose();
            WlDataDeviceManager.Dispose();
            WlInputDevice.Dispose();
            WlScreens.Dispose();
            WlSeat.Dispose();
            WlShm.Dispose();
            XdgActivation.Dispose();
            XdgWmBase.Dispose();
            WlCompositor.Dispose();
            WlRegistryHandler.Dispose();
            WlDisplay.Dispose();
        }
    }
}

namespace Avalonia
{
    public static class AvaloniaWaylandPlatformExtensions
    {
        public static T UseWayland<T>(this T builder) where T : AppBuilderBase<T>, new() =>
            builder.UseWindowingSubsystem(static () =>
            {
                var options = AvaloniaLocator.Current.GetService<WaylandPlatformOptions>() ?? new WaylandPlatformOptions();
                var platform = new AvaloniaWaylandPlatform(options);
                AvaloniaLocator.CurrentMutable.BindToSelf(platform);
            });
    }

    public class WaylandPlatformOptions
    {
        /// <summary>
        /// Determines whether to use GPU for rendering in your project. The default value is true.
        /// </summary>
        public bool UseGpu { get; set; } = true;

        /// <summary>
        /// Deferred renderer would be used when set to true. Immediate renderer when set to false. The default value is true.
        /// </summary>
        /// <remarks>
        /// Avalonia has two rendering modes: Immediate and Deferred rendering.
        /// Immediate re-renders the whole scene when some element is changed on the scene. Deferred re-renders only changed elements.
        /// </remarks>
        public bool UseDeferredRendering { get; set; } = true;

        /// <summary>
        /// The app ID identifies the general class of applications to which the surface belongs. <br/>
        /// The compositor can use this to group multiple surfaces together, or to determine how to launch a new application. <br/>
        /// As a best practice, it is suggested to select app ID's that match the basename of the application's .desktop file. For example, "org.freedesktop.FooViewer" where the .desktop file is "org.freedesktop.FooViewer.desktop".
        /// </summary>
        public string? AppId { get; set; }
    }
}
