using HarmonyLib;
using Unity.Netcode;
using static SuperEgg.AudioAppend;

namespace SuperEgg.Patch;

[HarmonyPatch(typeof(DepositItemsDesk))]
public static class DeskCounterPatch {
    public static DepositItemsDesk? depositItemsDesk;
    public static bool playAudio;

    [HarmonyPatch(nameof(DepositItemsDesk.Start))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void SetDepositItemDesk(DepositItemsDesk __instance) => depositItemsDesk = __instance;

    [HarmonyPatch(nameof(DepositItemsDesk.AddObjectToDeskClientRpc))]
    [HarmonyPostfix]
    private static void CheckForEggs(NetworkObjectReference grabbableObjectNetObject) {
#pragma warning disable Harmony003
        var foundNetworkObject = grabbableObjectNetObject.TryGet(out var networkObject);
#pragma warning restore Harmony003

        if (!foundNetworkObject) {
            SuperEgg.Logger.LogError("Could not find NetworkObject in the object that was placed on desk.");
            return;
        }

        var grabbableObject = networkObject.gameObject.GetComponentInChildren<GrabbableObject>();

        TestGrabbableObject(grabbableObject);
    }

    internal static void TestGrabbableObject(GrabbableObject grabbableObject) {
        if (grabbableObject == null || !grabbableObject) {
            SuperEgg.Logger.LogError("Could not find GrabbableObject in the object that was placed on desk.");
            return;
        }

        var itemName = grabbableObject.itemProperties.itemName;

        if (!itemName.Equals(EggConfig.eggName.Value)) {
            SuperEgg.LogDebug($"{itemName} is not {EggConfig.eggName.Value}!");
            return;
        }

        playAudio = true;
    }

    [HarmonyPatch(nameof(DepositItemsDesk.MicrophoneSpeak))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void AppendEggSounds(DepositItemsDesk __instance) {
        if (!playAudio) return;

        __instance.StartCoroutine(AppendSound(__instance.speakerAudio));
    }
}