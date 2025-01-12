using System;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.FreeDesktop;
using Avalonia.Platform;
using Avalonia.Utilities;
using Avalonia.Wayland.FreeDesktop;
using NWayland.Protocols.Wayland;

namespace Avalonia.Wayland
{
    internal class WlBitmapCursor : WlCursor, IFramebufferPlatformSurface
    {
        private readonly IBitmapImpl _cursor;
        private readonly int _stride;
        private readonly int _fd;
        private readonly int _size;
        private readonly IntPtr _data;
        private readonly WlShmPool _wlShmPool;
        private readonly WlBuffer _wlBuffer;
        private readonly WlCursorImage _wlCursorImage;

        public WlBitmapCursor(AvaloniaWaylandPlatform platform, IBitmapImpl cursor, PixelPoint hotspot) : base(1)
        {
            _cursor = cursor;
            _stride = cursor.PixelSize.Width * 4;
            _size = cursor.PixelSize.Height * _stride;
            _fd = FdHelper.CreateAnonymousFile(_size, "wayland-shm");
            if (_fd == -1)
                throw new WaylandPlatformException("Failed to create FrameBuffer.");
            _data = LibC.mmap(IntPtr.Zero, new IntPtr(_size), MemoryProtection.PROT_READ | MemoryProtection.PROT_WRITE, SharingType.MAP_SHARED, _fd, IntPtr.Zero);
            _wlShmPool= platform.WlShm.CreatePool(_fd, _size);
            _wlBuffer = _wlShmPool.CreateBuffer(0, cursor.PixelSize.Width, cursor.PixelSize.Height, _stride, WlShm.FormatEnum.Argb8888);
            var platformRenderInterface = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>();
            using var renderTarget = platformRenderInterface.CreateRenderTarget(new[] { this });
            using var ctx = renderTarget.CreateDrawingContext(null);
            var r = new Rect(cursor.PixelSize.ToSize(1));
            ctx.DrawBitmap(RefCountable.CreateUnownedNotClonable(cursor), 1, r, r);
            _wlCursorImage = new WlCursorImage(_wlBuffer, cursor.PixelSize, hotspot, TimeSpan.Zero);
        }

        public override WlCursorImage this[int index] => _wlCursorImage;

        public override void Dispose()
        {
            _wlBuffer.Dispose();
            _wlShmPool.Dispose();
            if (_data != IntPtr.Zero)
                LibC.munmap(_data, new IntPtr(_size));
            if (_fd != -1)
                LibC.close(_fd);
        }

        public ILockedFramebuffer Lock() => new LockedFramebuffer(_data, _cursor.PixelSize, _stride, new Vector(96, 96), PixelFormat.Bgra8888, null);
    }
}
