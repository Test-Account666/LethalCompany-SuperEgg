using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TestAccountCore.Dependencies;
using TestAccountCore.Dependencies.Compatibility;
using static TestAccountCore.Netcode;

namespace SuperEgg;

[BepInDependency("TestAccount666.TestAccountCore", "1.3.0")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SuperEgg : BaseUnityPlugin {
    public static SuperEgg Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    //TODO: Make RPC work!!!!

    internal static void Patch() {
        Harmony ??= new(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll();

        Logger.LogDebug("Finished patching!");
    }

    private void Awake() {
        Logger = base.Logger;
        Instance = this;

        if (DependencyChecker.IsLobbyCompatibilityInstalled()) {
            Logger.LogInfo("Found LobbyCompatibility Mod, initializing support :)");
            LobbyCompatibilitySupport.Initialize(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION,
                                                 CompatibilityLevel.Everyone, VersionStrictness.Minor);
        }

        EggConfig.Initialize(Config);

        Patch();

        ExecuteNetcodePatcher(Assembly.GetExecutingAssembly());

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Unpatch() {
        Logger.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }

    public static void LogDebug(object data) {
        if (!EggConfig.enableDebugLogs.Value) return;

        Logger.LogInfo(data);
    }
}