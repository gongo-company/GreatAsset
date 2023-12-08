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
    /// <summary>
    /// Class that holds all of the Harmony patches for this mod.
    /// Possibly pending refactor depending on how many patches are required in the future.
    /// </summary>
    internal class GreatAssetPatches
    {
        /// <summary>
        /// Dictionary used to decide which clip to play next for a given emote. 
        /// Key is the emote ID and value is the index of the AudioClip to play from the list stored in Resources.
        /// </summary>
        private static Dictionary<int, int> clipIndices = new Dictionary<int, int>();

        /// <summary>
        /// Patch meant to be triggered whenever a player starts an emote.
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.StartPerformingEmoteServerRpc))]
        [HarmonyPostfix]
        private static void ClientPostfix(PlayerControllerB __instance)
        {
            int emoteID = __instance.playerBodyAnimator.GetInteger("emoteNumber");

            Plugin.Log.LogInfo($"StartPerformingEmote on player {__instance.playerUsername} emoteID: {emoteID}");

            //TEMP: this makes it always play for every emote because I can't get the code to check specific emotes working.
            emoteID = 2;
            
            PlayClipForEmote(emoteID, __instance.transform);
        }

        /// <summary>
        /// Chooses and plays an audio clip based on which emote is being performed.
        /// AudioClip choice is not randomized currently so that it stays synced across clients. It just plays the next AudioClip in the list, which might be the preferred behavior anyway so players can set up "sentences" with emotes.
        /// </summary>
        /// <param name="emoteID">Corresponds to the number key pressed, fetched from the playerBodyAnimator (probably).</param>
        /// <param name="position">Where to spawn the AudioSource to play the AudioClip. This may be replaced in the future so that can be attached to the player's position.</param>
        private static void PlayClipForEmote(int emoteID, Transform attachTo)
        {
            // Return early without playing anything if there is no AudioClip available for the given emote
            if (!Assets.Resources.emoteClips.ContainsKey(emoteID))
                return;

            // Choose which clip to play from the list, starting with the first one.
            int clipIndex = 0;
            if (clipIndices.ContainsKey(emoteID))
            {
                // Increment to the next clip and wrap back to 0 if the end of the list is reached
                clipIndex = clipIndex + 1 == Assets.Resources.emoteClips[emoteID].Count() ? 0 : clipIndices[emoteID] + 1;
            }
            // Update the list for next time
            clipIndices[emoteID] = clipIndex;

            AudioClip clip = Assets.Resources.emoteClips[emoteID][clipIndex];

            if (clip)
            {
                //TODO: Look into spawning an AudioSource attached to the player so that longer audio clips follow the player
                //TODO: ...also so that it can be stopped early if the emote is stopped before playback completes.
                AudioSource.PlayClipAtPoint(clip, attachTo.position);
            }
        }
    }
}
