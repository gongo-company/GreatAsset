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
        // Cached reference to the embedded AssetBundle
        private static AssetBundle _assetBundle;

        /// <summary>
        /// Fetches the embedded AssetBundle by name and loads it into memory, caching the result.
        /// </summary>
        public static void LoadEmbeddedAssetBundle(Assembly assembly, string name)
        {
            IEnumerable<string> manifestResourceNames = assembly.GetManifestResourceNames();

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
                        Plugin.Log.LogInfo($"Loading AssetBundle '{name}' from '{assembly.GetName()}'...");
                        byte[] array = memoryStream.ToArray();
                        Plugin.Log.LogDebug("Loaded AssetBundle into memory");
                        result = AssetBundle.LoadFromMemory(array);
                        Plugin.Log.LogInfo($"Done loading AssetBundle '{name}' from '{assembly.GetName()}'.");
                    }
                }
            }

            // Cache result
            _assetBundle = result;
            
            // For Debug only. Remove this if you end up embedding more assets.
            foreach (var assetName in _assetBundle.GetAllAssetNames())
            {
                Plugin.Log.LogDebug($"Asset in bundle: '{assetName}'");
            }
        }

        public static UnityEngine.Object LoadEmbeddedAsset(string name)
        {
            if (!_assetBundle)
                Plugin.Log.LogError($"Error loading embedded asset {name}, embedded AssetBundle has not been loaded.");

            if (!_assetBundle.Contains(name))
                Plugin.Log.LogError($"Error loading asset '{name}', it was not found in the cached AssetBundle '{_assetBundle.name}'.");
            
            Plugin.Log.LogDebug($"Loading embedded asset '{name}' from '{_assetBundle.name}' ...");
            return _assetBundle.LoadAsset(name);
        }
    }
}
