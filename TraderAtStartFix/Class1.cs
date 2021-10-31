using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using UnityEngine;
using DiskCardGame;
using HarmonyLib;


namespace TraderAtStartFix
{
	

	public class ConfigHandler : BaseUnityPlugin
    {
		public int getWolf()
        {
			return Config.Bind("TraderAtStart", "WolfChance", 3, new BepInEx.Configuration.ConfigDescription("")).Value; Config.Bind("TraderAtStart", "", 3, new BepInEx.Configuration.ConfigDescription(""));
		}
		public int getGolden()
		{
			return Config.Bind("TraderAtStart", "GoldenChance", 10, new BepInEx.Configuration.ConfigDescription("")).Value; Config.Bind("TraderAtStart", "", 3, new BepInEx.Configuration.ConfigDescription(""));
		}
	}
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	public class Plugin : BaseUnityPlugin
	{
		private const string PluginGuid = "bobthenerd.inscryption.traderstart";
		private const string PluginName = "TraderAtStart";
		private const string PluginVersion = "1.0.0";

		internal static ManualLogSource Log;

		private void Awake()
		{
			Logger.LogInfo($"Loaded {PluginName}!");
			Plugin.Log = base.Logger;
			
			Harmony harmony = new Harmony(PluginGuid);
			harmony.PatchAll();
		}

		[HarmonyPatch(typeof(DeckInfo), "InitializeAsPlayerDeck")]
		public class TraderStartPatch : DeckInfo
		{
			[HarmonyPrefix]
			public static bool Prefix(ref DeckInfo __instance)
            {
				ConfigHandler c = new ConfigHandler();

				__instance.AddCard(CardLoader.GetCardByName("PeltHare"));
				__instance.AddCard(CardLoader.GetCardByName("PeltHare"));
				__instance.AddCard(CardLoader.GetCardByName("PeltHare"));
				__instance.AddCard(CardLoader.GetCardByName("PeltHare"));
				if (SeededRandom.Range(1, c.getWolf(), SaveManager.SaveFile.GetCurrentRandomSeed()) == 1)
                {
					__instance.AddCard(CardLoader.GetCardByName("PeltWolf"));
				}
				if (SeededRandom.Range(1, c.getGolden(), SaveManager.SaveFile.GetCurrentRandomSeed()) == 1)
				{
					__instance.AddCard(CardLoader.GetCardByName("PeltGolden"));
				}
				return false;
			}

		}

		[HarmonyPatch(typeof(PaperGameMap), "TryInitializeMapData")]
		public class RunState_TryInitializeMapData
        {
			public static bool Prefix(ref PaperGameMap __instance)
            {
				if (RunState.Run.map == null)
				{
					PredefinedNodes nodes = ScriptableObject.CreateInstance<PredefinedNodes>();
					PredefinedScenery scenery = ScriptableObject.CreateInstance<PredefinedScenery>();
					var node1 = new NodeData();
					var node2 = new TradePeltsNodeData();
					nodes.nodeRows.Add(new List<NodeData>() { node1 });
					nodes.nodeRows.Add(new List<NodeData>() { node2 });
					RunState.Run.map = MapGenerator.GenerateMap(RunState.CurrentMapRegion, 3, 13, nodes, scenery);
					RunState.Run.currentNodeId = SaveManager.SaveFile.currentRun.map.RootNode.id;

				}
				return false;
			}
        }
	}
}
