using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SuperEgg;

public class DestroyListener : MonoBehaviour {
    public readonly HashSet<GameObject> destroyWithListener = [
    ];

    public void AddObject(GameObject objectToDestroy) => destroyWithListener.Add(objectToDestroy);

    private void OnDestroy() {
        if (StartOfRound.Instance == null) return;

        foreach (var objectToDestroy in destroyWithListener) {
            var hasNetworkObject = objectToDestroy.TryGetComponent<NetworkObject>(out var networkObject);

            if (!hasNetworkObject) {
                Destroy(objectToDestroy);
                continue;
            }


            if (StartOfRound.Instance.IsHost || StartOfRound.Instance.IsServer) networkObject.Despawn();
            Destroy(objectToDestroy);
        }
    }
}