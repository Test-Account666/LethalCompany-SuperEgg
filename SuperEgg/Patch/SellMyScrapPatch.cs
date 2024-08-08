using com.github.zehsteam.SellMyScrap.Patches;
using HarmonyLib;
using static SuperEgg.AudioAppend;

namespace SuperEgg.Patch;

[HarmonyPatch(typeof(DepositItemsDeskPatch))]
public static class SellMyScrapPatch {
    [HarmonyPatch(nameof(DepositItemsDeskPatch.MicrophoneSpeakPatch))]
    [HarmonyPostfix]
    private static void AppendEggSounds() {
        if (!played && !DeskCounterPatch.playAudio) return;

        StartOfRound.Instance.StartCoroutine(AppendSound(StartOfRound.Instance.speakerAudioSource));
    }

    [HarmonyPatch(nameof(DepositItemsDeskPatch.PlaceItemOnCounter))]
    [HarmonyPostfix]
    private static void CheckForEggs(GrabbableObject grabbableObject) => DeskCounterPatch.TestGrabbableObject(grabbableObject);
}