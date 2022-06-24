using System;
using System.Collections.Generic;
using System.Linq;
using NWayland.Interop;
using NWayland.Protocols.Wayland;

namespace SimpleWindow
{
    internal class WlRegistryHandler : WlRegistry.IEvents, IDisposable
    {
        private readonly WlRegistry _registry;
        private readonly Dictionary<uint, GlobalInfo> _globals = new();

        public WlRegistryHandler(WlRegistry registry)
        {
            _registry = registry;
            registry.Events = this;
        }

        private Action<GlobalInfo>? _globalAdded;
        public event Action<GlobalInfo>? GlobalAdded
        {
            add
            {
                _globalAdded += value;
                foreach (var global in _globals.Values)
                    value?.Invoke(global);
            }
            remove => _globalAdded -= value;
        }

        public event Action<GlobalInfo>? GlobalRemoved;

        public void OnGlobal(WlRegistry eventSender, uint name, string @interface, uint version)
        {
            var global = new GlobalInfo(name, @interface, (int)version);
            _globals[name] = global;
            _globalAdded?.Invoke(global);
        }

        public void OnGlobalRemove(WlRegistry eventSender, uint name)
        {
            if (!_globals.TryGetValue(name, out var glob)) return;
            _globals.Remove(name);
            GlobalRemoved?.Invoke(glob);
        }

        public T BindRequiredInterface<T>(IBindFactory<T> factory, string @interface, int version) where T : WlProxy =>
            Bind(factory, @interface, version) ?? throw new NWaylandException($"Failed to bind required interface {@interface}");

        public T BindRequiredInterface<T>(IBindFactory<T> factory, int version, GlobalInfo global) where T : WlProxy =>
            Bind(factory, version, global) ?? throw new NWaylandException($"Failed to bind required interface {global.Interface}");

        public T? Bind<T>(IBindFactory<T> factory, string @interface, int version) where T : WlProxy
        {
            var global = _globals.Values.FirstOrDefault(g => g.Interface == @interface);

            if (global is null)
                throw new NotSupportedException($"Unable to find {@interface} in the registry");

            return Bind(factory, version, global);
        }

        public unsafe T? Bind<T>(IBindFactory<T> factory, int version, GlobalInfo global) where T : WlProxy
        {
            if (version > factory.GetInterface()->Version)
                throw new ArgumentException($"Version {version} is not supported");
            var requestVersion = Math.Min(version, global.Version);
            return _registry.Bind(global.Name, factory, requestVersion);
        }

        public void Dispose()
        {
            _registry.Dispose();
        }

        public sealed class GlobalInfo
        {
            public uint Name { get; }
            public string Interface { get; }
            public int Version { get; }

            internal GlobalInfo(uint name, string @interface, int version)
            {
                Name = name;
                Interface = @interface;
                Version = version;
            }

            public override string ToString() => $"{Interface} version {Version} at {Name}";
        }
    }
}