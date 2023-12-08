using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GreatAsset.Assets
{
    internal static class AssetLoader
    {
        private static AssetBundle _assetBundle;

        public static void LoadEmbeddedAssetBundle(Assembly assembly, string name)
        {
            IEnumerable<string> manifestResourceNames = assembly.GetManifestResourceNames();
            Plugin.Log.LogInfo($"Manifest resource names in assembly {assembly.GetName()}: {manifestResourceNames.Count()}");
            foreach (var item in manifestResourceNames)
            {
                Plugin.Log.LogInfo($"Resource name: {item}");
            }
            AssetBundle result = null;
            if (manifestResourceNames.Contains(name))
            {
                Plugin.Log.LogInfo("Loading embedded resource: " + name);
                using (Stream manifestResourceStream = assembly.GetManifestResourceStream(name))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        if (manifestResourceStream != null)
                        {
                            manifestResourceStream.CopyTo(memoryStream);
                        }
                        Plugin.Log.LogInfo("Loading AssetBundle...");
                        byte[] array = memoryStream.ToArray();
                        Plugin.Log.LogInfo("Loaded AssetBundle into memory");
                        result = AssetBundle.LoadFromMemory(array);
                        Plugin.Log.LogInfo("Done loading AssetBundle.");
                    }
                }
            }
            _assetBundle = result;
            foreach (var assetName in _assetBundle.GetAllAssetNames())
            {
                Plugin.Log.LogInfo($"Asset in bundle: '{assetName}'");
            }
        }

        public static UnityEngine.Object LoadEmbeddedAsset(string name)
        {
            if (!_assetBundle)
                Plugin.Log.LogError($"Error loading embedded asset {name}, embedded AssetBundle has not been loaded.");

            if (!_assetBundle.Contains(name))
                Plugin.Log.LogError($"Error loading asset '{name}', it was not found in the AssetBundle.");
            
            Plugin.Log.LogInfo($"Loading embedded asset '{name}' from '{_assetBundle.name}'");
            return _assetBundle.LoadAsset(name);
        }
    }
}
