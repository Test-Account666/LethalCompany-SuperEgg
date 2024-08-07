using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Object;

namespace SuperEgg.Patch;

[HarmonyPatch]
public static class NetworkManagerPatch {
    internal const uint NETWORK_OBJECT_ID_HASH = 666578908;

    [HarmonyPatch(typeof(NetworkManager), nameof(NetworkManager.SetSingleton))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void AfterSingleton(NetworkManager __instance) {
        SuperEgg.superEggPrefab = new("SuperEgg", typeof(NetworkObject), typeof(EasterEgg));

        var networkObject = SuperEgg.superEggPrefab.GetComponent<NetworkObject>();
        networkObject.AutoObjectParentSync = true;
        networkObject.DestroyWithScene = false;
        networkObject.GlobalObjectIdHash = NETWORK_OBJECT_ID_HASH;

        SuperEgg.superEggPrefab.hideFlags |= HideFlags.HideAndDontSave;
        DontDestroyOnLoad(SuperEgg.superEggPrefab);

        __instance.AddNetworkPrefab(SuperEgg.superEggPrefab);

        SuperEgg.LogDebug("Added SuperEgg Prefab!");
    }
}