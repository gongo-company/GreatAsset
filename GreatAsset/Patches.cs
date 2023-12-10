using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using HarmonyLib;
using GameNetcodeStuff;

namespace GreatAsset.Patches
{
    /// <summary>
    /// Class that holds all of the Harmony patches for this mod.
    /// Possibly pending refactor depending on how many patches are required in the future.
    /// </summary>
    internal class GreatAssetPatches
    {
        [HarmonyPatch(typeof(PlayerControllerB), "Start")]
        [HarmonyPostfix]
        private static void StartPatch(PlayerControllerB __instance)
        {
            GameObject gameObject = __instance.gameObject.transform.Find("ScavengerModel").transform.Find("metarig").gameObject;
            CustomAudioAnimationEvent customAudioAnimationEvent = gameObject.AddComponent<CustomAudioAnimationEvent>();
            customAudioAnimationEvent.player = __instance;
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.StartPerformingEmoteClientRpc))]
        [HarmonyPostfix]
        private static void PerformEmotePatch(PlayerControllerB __instance)
        {
            if (!__instance.NetworkManager.IsClient)
                return;
            // This is a stupid hack.
            // But it works.
            // Every time the player presses the emote button, reset the animator's time to 0 so that my custom event can trigger again.
            // I hate this, it uses a deprecated property and it's a stupid hack but it works.
            // Without this, the animation clip only gets played the first time the player uses an emote. So if they use the same emote twice in a row, it just keeps the same clip playing (and so won't trigger my event again).
            var animator = __instance.playerBodyAnimator;
            Plugin.Log.LogInfo($"PerformEmote on player {__instance.playerUsername}");
            animator.Play(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("EmotesNoArms")).fullPathHash, animator.GetLayerIndex("EmotesNoArms"), 0f);
        }
    }
}
