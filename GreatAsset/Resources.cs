using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace GreatAsset.Assets
{
    class Resources
    {
        public static Dictionary<int, List<AudioClip>> emoteClips;

        public static void Load()
        {
            emoteClips = new Dictionary<int, List<AudioClip>>();

            LoadEmbeddedFallbacks();
        }

        static void LoadEmbeddedFallbacks()
        {
            Plugin.Log.LogInfo("Loading embedded assets...");

            Assets.AssetLoader.LoadEmbeddedAssetBundle(Assembly.GetExecutingAssembly(), "GreatAsset.Resources.Audio.bundle");

            AudioClip greatAsset = (AudioClip)Assets.AssetLoader.LoadEmbeddedAsset("greatasset.wav");

            emoteClips.Add(2, new List<AudioClip>() { greatAsset });
        }
    }
}