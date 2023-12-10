using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using HarmonyLib;
using BepInEx;
using GameNetcodeStuff;


/*
 * Layer: EmotesNoArms 
 * Clip: Dance2
 */


namespace GreatAsset.Patches
{
    /// <summary>
    /// Custom component that manages playing the audio clip using an animation event added to the appropriate clip.
    /// </summary>
    public class CustomAudioAnimationEvent : MonoBehaviour
    {
        // Reference to the associated PlayerControllerB, this is set from the patch right after the component is instantiated and added to the player
        public PlayerControllerB player;

        Animator animator;
        AnimationClip animationClip;
        AudioSource SoundSource;

        private void Start()
        {
            this.animator = base.GetComponent<Animator>();
            // There might be a better way to choose a soundsource (spawn a new one?) but this works for now
            this.SoundSource = this.player.movementAudio;

            AddAnimationEventToAnimationClip();
        }

        // There might be a better place to run this from but it needs to have access to the animation clip which I get from the player's animator.
        // So this solution is kind of a hack but it works so for the moment, it stays.
        /// <summary>
        /// This function adds the event to the clip, but the clip is shared by all instances of the player so it needs to only happen once.
        /// </summary>
        void AddAnimationEventToAnimationClip()
        {
            //if (addedAnimationEvent)
            //    return;

            Plugin.Log.LogInfo($"Trying to add AnimationEvent to point animation clip on player {player.playerUsername}...");

            // Find the animation clip and add an event to it
            AnimationClip[] animationClips = this.animator.runtimeAnimatorController.animationClips;
            animationClip = animationClips.FirstOrDefault(clip => clip.name.Contains("Dance2"));

            if (animationClip == null)
            {
                Plugin.Log.LogError($"Error finding Dance2 clip. {MyPluginInfo.PLUGIN_NAME} will not work.");
                return;
            }

            if (animationClip.events.Count() > 0)
            {
                Plugin.Log.LogInfo("AnimationEvent already added.");
                return;
            }
            animationClip.AddEvent(CreateAnimationEvent());

            Plugin.Log.LogInfo("AnimationEvent added to point animation clip.");
        }

        AnimationEvent CreateAnimationEvent()
        {
            Plugin.Log.LogInfo("Creating animation event...");
            AnimationEvent animationEvent = new AnimationEvent();

            animationEvent.time = 0f;
            animationEvent.functionName = "PlayAudioClip";

            return animationEvent;
        }

        void PlayAudioClip()
        {
            Plugin.Log.LogInfo($"PlayAudioClip event");
            SoundSource.PlayOneShot(Assets.Resources.GreatAssetClip);
        }
    }
}