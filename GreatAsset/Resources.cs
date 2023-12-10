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
        public static AudioClip GreatAssetClip;

        public static void Load()
        {
            Plugin.Log.LogInfo("Loading embedded assets...");

            Assets.AssetLoader.LoadEmbeddedAssetBundle(Assembly.GetExecutingAssembly(), "GreatAsset.Resources.Audio.bundle");

            // Just load the default "Great asset!" file for pointing and nothing else, for now.
            GreatAssetClip = (AudioClip)Assets.AssetLoader.LoadEmbeddedAsset("greatasset.wav");
        }
    }
}