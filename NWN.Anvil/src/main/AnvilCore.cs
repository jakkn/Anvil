using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Anvil.API;
using Anvil.Internal;
using Anvil.Plugins;
using Anvil.Services;
using NLog;
using NWN.Core;

namespace Anvil
{
  /// <summary>
  /// Handles bootstrap and interop between %NWN, %NWN.Core and the %Anvil %API. The entry point of the implementing module should point to this class.<br/>
  /// Until <see cref="Init(IntPtr, int, IContainerFactory)"/> is called, all APIs are unavailable for usage.
  /// </summary>
  public sealed class AnvilCore : IServerLifeCycleEventHandler
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();

    private static AnvilCore instance;

    private CoreInteropHandler interopHandler;
    private LoggerManager loggerManager;

    private PluginManager pluginManager;
    private ServiceManager serviceManager;

    private UnhandledExceptionLogger unhandledExceptionLogger;

    private AnvilCore() {}

    /// <summary>
    /// Entrypoint to start Anvil.
    /// </summary>
    /// <param name="arg">The NativeHandles pointer, provided by the NWNX bootstrap entry point.</param>
    /// <param name="argLength">The size of the NativeHandles bootstrap structure, provided by the NWNX entry point.</param>
    /// <param name="containerFactory">An optional container factory to use instead of the default <see cref="AnvilContainerFactory"/>.</param>
    /// <returns>The init result code to return back to NWNX.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Init(IntPtr arg, int argLength, IContainerFactory containerFactory = default)
    {
      containerFactory ??= new AnvilContainerFactory();

      instance = new AnvilCore();
      instance.interopHandler = new CoreInteropHandler(instance);
      instance.loggerManager = new LoggerManager();
      instance.unhandledExceptionLogger = new UnhandledExceptionLogger();
      instance.pluginManager = new PluginManager();
      instance.serviceManager = new ServiceManager(instance.pluginManager, instance.interopHandler, containerFactory);

      return NWNCore.Init(arg, argLength, instance.interopHandler, instance.interopHandler);
    }

    /// <summary>
    /// Initiates a complete reload of plugins and Anvil services.<br/>
    /// This will reload all plugins.
    /// </summary>
    public static void Reload()
    {
      if (!EnvironmentConfig.ReloadEnabled)
      {
        Log.Error("Hot Reload of plugins is not enabled (NWM_RELOAD_ENABLED=true)");
      }

      instance.serviceManager?.GetService<SchedulerService>()?.Schedule(() =>
      {
        Log.Info("Reloading Anvil");

        instance.ShutdownServices();
        instance.serviceManager.ShutdownLateServices();
        instance.pluginManager.Unload();

        instance.pluginManager.Load();
        instance.InitServices();
      }, TimeSpan.Zero);
    }

    void IServerLifeCycleEventHandler.HandleLifeCycleEvent(LifeCycleEvent eventType)
    {
      switch (eventType)
      {
        case LifeCycleEvent.ModuleLoad:
          InitCore();
          InitServices();
          break;
        case LifeCycleEvent.DestroyServer:
          Log.Info("Server is shutting down...");
          ShutdownServices();
          break;
        case LifeCycleEvent.DestroyServerAfter:
          ShutdownCore();
          break;
        case LifeCycleEvent.Unhandled:
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
      }
    }

    private void CheckServerVersion()
    {
      AssemblyName assemblyName = Assemblies.Anvil.GetName();
      Version serverVersion = NwServer.Instance.ServerVersion;

      if (assemblyName.Version?.Major != serverVersion.Major || assemblyName.Version.Minor != serverVersion.Minor)
      {
        Log.Warn("The current version of {Name} targets version {TargetVersion}, but the server is running {ServerVersion}! You may encounter compatibility issues",
          assemblyName.Name,
          assemblyName.Version,
          serverVersion);
      }
    }

    private void InitCore()
    {
      loggerManager.Init();

      try
      {
        PrelinkNative();
      }
      catch (Exception e)
      {
        Log.Fatal(e, "Failed to load {Name:l} {Version:l} (NWN.Core: {CoreVersion}, NWN.Native: {NativeVersion})",
          Assemblies.Anvil.GetName().Name,
          AssemblyInfo.VersionInfo.InformationalVersion,
          Assemblies.Core.GetName().Version,
          Assemblies.Native.GetName().Version);
        throw;
      }

      loggerManager.InitVariables();
      unhandledExceptionLogger.Init();

      Log.Info("Loading {Name:l} {Version:l} (NWN.Core: {CoreVersion}, NWN.Native: {NativeVersion})",
        Assemblies.Anvil.GetName().Name,
        AssemblyInfo.VersionInfo.InformationalVersion,
        Assemblies.Core.GetName().Version,
        Assemblies.Native.GetName().Version);

      CheckServerVersion();
      pluginManager.Load();
    }

    private void InitServices()
    {
      serviceManager.Init(instance.pluginManager, instance.serviceManager);
    }

    private void PrelinkNative()
    {
      if (!EnvironmentConfig.NativePrelinkEnabled)
      {
        Log.Warn("Marshaller prelinking is disabled (ANVIL_PRELINK_ENABLED=false). You may encounter random crashes or issues");
        return;
      }

      Log.Info("Prelinking native methods");

      try
      {
        Marshal.PrelinkAll(typeof(NWN.Native.API.NWNXLibPINVOKE));
        Log.Info("Prelinking complete");
      }
      catch (TypeInitializationException)
      {
        Log.Fatal("The NWNX_SWIG_DotNET plugin could not be found. Has it been enabled? (NWNX_SWIG_DOTNET_SKIP=n)");
        throw;
      }
      catch (Exception)
      {
        Log.Fatal("The NWNX_SWIG_DotNET plugin could not be loaded");
        throw;
      }
    }

    private void ShutdownCore()
    {
      serviceManager.ShutdownLateServices();
      pluginManager.Unload();
      unhandledExceptionLogger.Dispose();
      loggerManager.Dispose();
    }

    private void ShutdownServices()
    {
      serviceManager.ShutdownServices();
    }
  }
}
