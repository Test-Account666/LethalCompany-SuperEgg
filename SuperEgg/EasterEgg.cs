using System;
using SuperEgg.Patch;
using Unity.Netcode;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace SuperEgg;

public class EasterEgg : NetworkBehaviour {
    internal GrabbableObject grabbableObject = null!;
    private bool _isSuperEgg;
    private Random _random;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        SuperEgg.LogDebug("Network haz spawned!!");

        if (!IsHost && !IsServer) return;

        var networkObject = grabbableObject.NetworkObject;

        SetGrabbableObjectClientRpc(networkObject);
    }

    [ClientRpc]
    private void SetGrabbableObjectClientRpc(NetworkObjectReference networkObjectReference) {
        var networkObject = (NetworkObject) networkObjectReference;

        grabbableObject = networkObject.GetComponent<GrabbableObject>();
    }

    private void Start() {
        _random = new((uint) (DateTime.Now.Ticks & 0x0000FFFF));
        _isSuperEgg = _random.NextInt(1, 100) <= EggConfig.superEggChance.Value;
    }

    private bool DisableExplosion() {
        if (!EggConfig.disableThrowAndDamageOnDeskCounter.Value) return false;

        var depositItemsDesk = DeskCounterPatch.depositItemsDesk;

        if (depositItemsDesk == null || !depositItemsDesk) return false;

        return depositItemsDesk.itemsOnCounter.Contains(grabbableObject);
    }

    public static void SpawnExplosion(
        Vector3 explosionPosition,
        bool spawnExplosionEffect = false,
        float killRange = 1f,
        float damageRange = 1f,
        int nonLethalDamage = 50,
        float physicsForce = 0.0f,
        GameObject? overridePrefab = null,
        bool goThroughCar = false,
        StunGrenadeItem? stunGrenadeItem = null) {
        if (stunGrenadeItem is null) {
            Landmine.SpawnExplosion(explosionPosition, spawnExplosionEffect, killRange, damageRange,
                                    nonLethalDamage, physicsForce, overridePrefab, goThroughCar);
            return;
        }

        var hasDestroyListener = stunGrenadeItem.TryGetComponent<DestroyListener>(out var destroyListener);

        if (!hasDestroyListener) {
            SuperEgg.Logger.LogDebug("No destroy listener!");
            Landmine.SpawnExplosion(explosionPosition, spawnExplosionEffect, killRange, damageRange,
                                    nonLethalDamage, physicsForce, overridePrefab, goThroughCar);
            return;
        }

        EasterEgg easterEgg = null!;

        foreach (var easterEggObject in destroyListener.destroyWithListener) {
            var hasEasterEgg = easterEggObject.TryGetComponent(out easterEgg);

            if (hasEasterEgg) break;
        }

        if (easterEgg == null || !easterEgg) {
            SuperEgg.Logger.LogDebug("No easter egg!");
            Landmine.SpawnExplosion(explosionPosition, spawnExplosionEffect, killRange, damageRange,
                                    nonLethalDamage, physicsForce, overridePrefab, goThroughCar);
            return;
        }

        SuperEgg.Logger.LogDebug("Executing on my end!");

        easterEgg.ExplodeServerRpc(explosionPosition);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ExplodeServerRpc(Vector3 explosionPosition) {
        SuperEgg.Logger.LogDebug("DO SOMETHING YOU PIECE OF EGG!");

        if (DisableExplosion()) {
            SuperEgg.LogDebug("Explosion disabled!");
            return;
        }

        SuperEgg.LogDebug("Explode!");
        SuperEgg.LogDebug($"IsSuperEgg? {_isSuperEgg}");

        var generatedChance = _isSuperEgg? _random.NextInt(1, 100) : 101;

        SuperEgg.LogDebug($"Generated Chance? {generatedChance}");
        SuperEgg.LogDebug($"Required Chance? {EggConfig.superExplosionChance.Value}");


        var isSuperExplosion = generatedChance <= EggConfig.superExplosionChance.Value;

        SuperEgg.LogDebug($"IsSuperExplosion? {isSuperExplosion}");

        var killRange = isSuperExplosion? EggConfig.superExplosionKillRange.Value : EggConfig.normalExplosionKillRange.Value;
        SuperEgg.LogDebug($"KillRange? {killRange}");

        var damageRange = isSuperExplosion? EggConfig.superExplosionDamageRange.Value : EggConfig.normalExplosionDamageRange.Value;
        SuperEgg.LogDebug($"DamageRange? {damageRange}");

        var nonLethalDamage = isSuperExplosion? EggConfig.superExplosionDamage.Value : EggConfig.normalExplosionDamage.Value;
        SuperEgg.LogDebug($"NonLethalDamage? {nonLethalDamage}");

        var minPhysicsForce = isSuperExplosion? EggConfig.superExplosionMinStrength.Value : EggConfig.normalExplosionMinStrength.Value;
        SuperEgg.LogDebug($"MinPhysicsForce? {minPhysicsForce}");

        var maxPhysicsForce = minPhysicsForce + (isSuperExplosion
            ? EggConfig.superExplosionAdditionalStrength.Value
            : EggConfig.normalExplosionAdditionalStrength.Value);
        SuperEgg.LogDebug($"MaxPhysicsForce? {maxPhysicsForce}");

        var physicsForce = _random.NextFloat(minPhysicsForce, maxPhysicsForce);
        SuperEgg.LogDebug($"Generated Physics? {physicsForce}");

        ExplodeClientRpc(explosionPosition, killRange, damageRange, nonLethalDamage, physicsForce);
    }

    [ClientRpc]
    public void ExplodeClientRpc(Vector3 explosionPosition, float killRange, float damageRange, int nonLethalDamage, float physicsForce) {
        Landmine.SpawnExplosion(explosionPosition, false, killRange, damageRange,
                                nonLethalDamage, physicsForce);
    }
}