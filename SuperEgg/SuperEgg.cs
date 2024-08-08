using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SuperEgg.Patch;
using TestAccountCore.Dependencies;
using TestAccountCore.Dependencies.Compatibility;
using UnityEngine;
using UnityEngine.Networking;
using static TestAccountCore.Netcode;
using Debug = System.Diagnostics.Debug;

namespace SuperEgg;

[BepInDependency("com.github.zehsteam.SellMyScrap", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("TestAccount666.TestAccountCore", "1.3.0")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SuperEgg : BaseUnityPlugin {
    public static GameObject superEggPrefab = null!;
    public static SuperEgg Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }


    internal static void Patch() {
        Harmony ??= new(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll(typeof(DeskCounterPatch));
        Harmony.PatchAll(typeof(NetworkManagerPatch));
        Harmony.PatchAll(typeof(StunGrenadeItemPatch));

        if (SellMyScrapChecker.IsSellMyScrapInstalled()) Harmony.PatchAll(typeof(SellMyScrapPatch));

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

        StartCoroutine(LoadSellAudioClip());

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

    private static IEnumerator LoadSellAudioClip() {
        var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        Logger.LogInfo("Loading Sounds...");

        Debug.Assert(assemblyDirectory != null, nameof(assemblyDirectory) + " != null");
        var audioPath = Path.Combine(assemblyDirectory, "sounds");

        audioPath = Directory.Exists(audioPath)? audioPath : Path.Combine(assemblyDirectory);

        LoadSellAudioClip(audioPath);

        yield break;
    }

    private static void LoadSellAudioClip(string audioPath) {
        Logger.LogInfo("Loading Ghost Hand Sounds...");

        var eggAudioPath = Path.Combine(audioPath, "EggSell");

        eggAudioPath = Directory.Exists(eggAudioPath)? eggAudioPath : Path.Combine(audioPath);

        var eggAudioClip = LoadAudioClipFromFile(new(Path.Combine(eggAudioPath, "EggSell.ogg")), "EggSell");

        if (eggAudioClip is null) {
            Logger.LogInfo("Failed to load clip 'EggSell'!");
            return;
        }

        AudioAppend.eggAudioClip = eggAudioClip;

        Logger.LogInfo($"Loaded clip '{eggAudioClip.name}'!");
    }

    private static AudioClip? LoadAudioClipFromFile(Uri filePath, string name) {
        using var unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.OGGVORBIS);

        var asyncOperation = unityWebRequest.SendWebRequest();

        while (!asyncOperation.isDone) Thread.Sleep(100);

        if (unityWebRequest.result != UnityWebRequest.Result.Success) {
            Logger.LogError("Failed to load AudioClip: " + unityWebRequest.error);
            return null;
        }

        var clip = DownloadHandlerAudioClip.GetContent(unityWebRequest);

        clip.name = name;

        return clip;
    }
}