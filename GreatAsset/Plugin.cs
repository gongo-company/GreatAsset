using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;


namespace GreatAsset
{
	[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
	public class Plugin : BaseUnityPlugin
	{
		internal static Harmony _harmony;
		// Use this to log from other classes (standard BepInEx practice).
		internal static ManualLogSource Log;

		private void Awake()
		{
			Plugin.Log = base.Logger;

			Assets.Resources.Load();

			_harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
			_harmony.PatchAll(typeof(Patches.GreatAssetPatches));

			Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
		}
	}
}