using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MonoMod.Utils;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SuperEgg.Patch;

[HarmonyPatch(typeof(StunGrenadeItem))]
public static class StunGrenadeItemPatch {
    [HarmonyPatch(nameof(StunGrenadeItem.Start))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void AddEasterEggComponent(StunGrenadeItem __instance) {
        if (__instance is {
                IsHost: false, IsServer: false,
            }) return;

        var itemName = __instance.itemProperties.itemName;

        if (!itemName.Equals(EggConfig.eggName.Value)) {
            SuperEgg.LogDebug($"{itemName} is not {EggConfig.eggName.Value}!");
            return;
        }

        var easterEggObject = Object.Instantiate(SuperEgg.superEggPrefab);
        easterEggObject.name = "SuperEgg";

        var easterEgg = easterEggObject.GetComponent<EasterEgg>();
        easterEgg.grabbableObject = __instance;

        var destroyListener = __instance.gameObject.AddComponent<DestroyListener>();
        destroyListener.AddObject(easterEggObject);

        var networkObject = easterEggObject.GetComponent<NetworkObject>();
        networkObject.Spawn();

        // ReSharper disable once Unity.InstantiateWithoutParent
        easterEggObject.transform.parent = __instance.gameObject.transform;
    }

    [HarmonyPatch(nameof(StunGrenadeItem.ExplodeStunGrenade))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ModifyEggs(IEnumerable<CodeInstruction> instructions) {
        var originalInstructions = instructions.ToList();

        List<CodeInstruction> codeInstructions = [
            ..originalInstructions,
        ];

        try {
            for (var index = 0; index < codeInstructions.Count; index++) {
                var instruction = codeInstructions[index];

                SuperEgg.LogDebug(instruction);

                if (instruction.opcode != OpCodes.Call) continue;

                if (instruction.operand is not MethodInfo methodInfo) continue;

                if (!methodInfo.Name.Contains("SpawnExplosion")) continue;

                SuperEgg.LogDebug($"Injecting {typeof(EasterEgg)}.{nameof(EasterEgg.SpawnExplosion)}!");

                var easterEggExplode = AccessTools.Method(typeof(EasterEgg), nameof(EasterEgg.SpawnExplosion), [
                    typeof(Vector3), typeof(bool), typeof(float), typeof(float), typeof(int), typeof(float), typeof(GameObject), typeof(bool),
                    typeof(StunGrenadeItem),
                ]);

                codeInstructions[index] = new(OpCodes.Call, easterEggExplode);
                codeInstructions.Insert(index, new(OpCodes.Ldarg_0));
                break;
            }

            return codeInstructions;
        } catch (Exception exception) {
            SuperEgg.Logger.LogFatal("Error!");

            exception.LogDetailed();
            return originalInstructions;
        }
    }
}