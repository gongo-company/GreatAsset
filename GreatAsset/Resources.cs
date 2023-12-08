using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace GreatAsset.Assets
{
    /// <summary>
    /// Helper class that loads all assets needed by the plugin and stores static references to them.
    /// Planned in the future to support loading assets from disk (as defined by a configuration file) using embedded assets as a fallback.
    ///
    /// Currently only uses embedded asset(s).
    /// </summary>
    class Resources
    {
        /// <summary>
        /// Cached references to all the AudioClips.
        /// Keys are emoteIDs
        /// Values are a list of AudioClips that should be played (in order) when that emote is triggered.
        /// </summary>
        public static Dictionary<int, List<AudioClip>> emoteClips;

        /// <summary>
        /// Load and cache static references to all assets/resources required by the plugin.
        /// Takes care of loading assets from disk and loads fallbacks from the embedded AssetBundle if necessary.
        /// </summary>
        public static void Load()
        {
            emoteClips = new Dictionary<int, List<AudioClip>>();

            //TODO: Load configuration (Probably in another class)
            //TODO: Load assets from disk if present

            LoadEmbeddedFallbacks();
        }

        /// <summary>
        /// Fetch the embedded AssetBundle and cache the assets from it. 
        /// This method should not be called if assets were loaded from disk since the user can't easily modify the embedded assets.
        /// </summary>
        static void LoadEmbeddedFallbacks()
        {
            Plugin.Log.LogInfo("Loading embedded assets...");

            Assets.AssetLoader.LoadEmbeddedAssetBundle(Assembly.GetExecutingAssembly(), "GreatAsset.Resources.Audio.bundle");

            AudioClip greatAsset = (AudioClip)Assets.AssetLoader.LoadEmbeddedAsset("greatasset.wav");

            // Just load the default "Great asset!" file for pointing and nothing else, for now.
            emoteClips.Add(2, new List<AudioClip>() { greatAsset });
        }
    }
}