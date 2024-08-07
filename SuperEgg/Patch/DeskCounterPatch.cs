using HarmonyLib;

namespace SuperEgg.Patch;

[HarmonyPatch(typeof(DepositItemsDesk))]
public static class DeskCounterPatch {
    public static DepositItemsDesk? depositItemsDesk;

    [HarmonyPatch(nameof(DepositItemsDesk.Start))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void SetDepositItemDesk(DepositItemsDesk __instance) => depositItemsDesk = __instance;
}