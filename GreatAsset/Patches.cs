using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using GameNetcodeStuff;

namespace GreatAsset.Patches
{
    internal class GreatAssetPatches
    {
        private static Dictionary<int, int> clipIndices = new Dictionary<int, int>();

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.StartPerformingEmoteServerRpc))]
        [HarmonyPostfix]
        private static void ClientPostfix(PlayerControllerB __instance)
        {
            
            PlayClipForEmoteAfterDelay(__instance, 0.25f);
        }

        private static void PlayClipForEmoteAfterDelay(PlayerControllerB player, float delay)
        {
            int emoteID = player.playerBodyAnimator.GetInteger("emoteNumber");
            emoteID = 2; // TEMPORARY, this makes it always play for every emote because I can't get the code to check emotes working.

            Plugin.Log.LogInfo($"StartPerformingEmote on player {player.playerUsername} emoteID: {emoteID}");

            // Return early without playing anything if there is no AudioClip available for the given emote
            if (!Assets.Resources.emoteClips.ContainsKey(emoteID))
                return;

            int clipIndex = 0;
            if (clipIndices.ContainsKey(emoteID))
            {
                // Increment to the next clip and wrap back to 0 if the end of the list is reached
                clipIndex = clipIndex + 1 == Assets.Resources.emoteClips[emoteID].Count() ? 0 : clipIndices[emoteID] + 1;
            }
            clipIndices[emoteID] = clipIndex;

            AudioClip clip = Assets.Resources.emoteClips[emoteID][clipIndex];

            if (clip)
            {
                AudioSource.PlayClipAtPoint(clip, player.transform.position);
            }
        }
    }
}
