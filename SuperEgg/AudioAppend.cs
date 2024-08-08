using System.Collections;
using SuperEgg.Patch;
using UnityEngine;

namespace SuperEgg;

public static class AudioAppend {
    public static bool played;
    public static AudioClip? eggAudioClip;

    public static IEnumerator AppendSound(AudioSource audioSource) {
        if (!EggConfig.enableSellAudio.Value || eggAudioClip is null || !eggAudioClip) {
            played = false;
            DeskCounterPatch.playAudio = false;
            yield break;
        }

        played = true;

        yield return new WaitUntil(() => !audioSource.isPlaying);
        yield return new WaitForSeconds(.5F);

        audioSource.PlayOneShot(eggAudioClip, 1F);

        yield return new WaitUntil(() => !audioSource.isPlaying);
        played = false;
        DeskCounterPatch.playAudio = false;
    }
}