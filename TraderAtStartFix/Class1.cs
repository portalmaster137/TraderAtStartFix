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

	

    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	[BepInIncompatibility("porta.inscryption.constantdeck")]
	public class Plugin : BaseUnityPlugin
	{




		public bool getSuperSecretConfig()
        {
		 	return Config.Bind("TraderAtStart", "SuperSecretOption", false, new BepInEx.Configuration.ConfigDescription("dont use this unless you wanna have a bad time")).Value;
		}

		public int getHare()
        {
			return Config.Bind("TraderAtStart", "RabbitCount", 4, new BepInEx.Configuration.ConfigDescription("")).Value;
        }
		public int getWolf()
		{
			return Config.Bind("TraderAtStart", "WolfChance", 3, new BepInEx.Configuration.ConfigDescription("")).Value;
		}
		public int getGolden()
		{
			return Config.Bind("TraderAtStart", "GoldenChance", 10, new BepInEx.Configuration.ConfigDescription("")).Value;
		}

		private const string PluginGuid = "porta.inscryption.traderstart";
		private const string PluginName = "TraderAtStart";
		private const string PluginVersion = "1.1.0";

		internal static ManualLogSource Log;

		private void Awake()
		{
			Logger.LogInfo($"Loading {PluginName}!");
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
				Plugin c = new Plugin();

				if (c.getSuperSecretConfig())
                {
					for (int i = 0; i < c.getHare(); i++)
					{
						__instance.AddCard(CardLoader.GetCardByName("PeltGolden"));
					}
				} else
                {
					for (int i = 0; i < c.getHare(); i++)
					{
						__instance.AddCard(CardLoader.GetCardByName("PeltHare"));
					}

					if (SeededRandom.Range(1, c.getWolf(), SaveManager.SaveFile.GetCurrentRandomSeed()) == 1)
					{
						__instance.AddCard(CardLoader.GetCardByName("PeltWolf"));
					}
					if (SeededRandom.Range(1, c.getGolden(), SaveManager.SaveFile.GetCurrentRandomSeed()) == 1)
					{
						__instance.AddCard(CardLoader.GetCardByName("PeltGolden"));
					}
				}

                
				return false;
			}

		}

		[HarmonyPatch(typeof(PaperGameMap), "TryInitializeMapData")]
		public class RunState_TryInitializeMapData
        {
			
			public static void Prefix(ref PaperGameMap __instance)
            {
				Plugin c = new Plugin();
				if (RunState.Run.map == null)
				{
					if (c.getSuperSecretConfig())
                    {
						PredefinedNodes nodes = ScriptableObject.CreateInstance<PredefinedNodes>();
						PredefinedScenery scenery = ScriptableObject.CreateInstance<PredefinedScenery>();
						var node1 = new NodeData();
						var node2 = new TradePeltsNodeData();
						var node3 = new CardChoicesNodeData();
						var node4 = new CardBattleNodeData();
						var node5 = new CardChoicesNodeData();
						node5.choicesType = CardChoicesType.Random;
						var node6 = new CardChoicesNodeData();
						node6.choicesType = CardChoicesType.Random;
						var node7 = new CardChoicesNodeData();
						node7.choicesType = CardChoicesType.Random;
						var node8a = new CardChoicesNodeData();
						node8a.choicesType = CardChoicesType.Random;
						var node8b = new TradePeltsNodeData();
						var boss1 = new BossBattleNodeData();
						boss1.bossType = Opponent.Type.ProspectorBoss;
						var boss2 = new BossBattleNodeData();
						boss2.bossType = Opponent.Type.AnglerBoss;
						var boss3 = new BossBattleNodeData();
						boss3.bossType = Opponent.Type.TrapperTraderBoss;
						Plugin.Log.LogInfo("Loading Rows");
						nodes.nodeRows.Add(new List<NodeData>() { node1 });
						nodes.nodeRows.Add(new List<NodeData>() { node2 });
						nodes.nodeRows.Add(new List<NodeData>() { node3 });
						nodes.nodeRows.Add(new List<NodeData>() { node4 });
						nodes.nodeRows.Add(new List<NodeData>() { node5 });
						nodes.nodeRows.Add(new List<NodeData>() { node6 });
						nodes.nodeRows.Add(new List<NodeData>() { node7 });
						nodes.nodeRows.Add(new List<NodeData>() { node8a, node8b });
						nodes.nodeRows.Add(new List<NodeData>() { boss1 });
						nodes.nodeRows.Add(new List<NodeData>() { boss2 });
						nodes.nodeRows.Add(new List<NodeData>() { boss3 });
						__instance.PredefinedNodes = nodes;
						//RunState.Run.map = MapGenerator.GenerateMap(RunState.CurrentMapRegion, 3, 13, nodes, scenery);
						//RunState.Run.currentNodeId = SaveManager.SaveFile.currentRun.map.RootNode.id;
					} else
                    {
						PredefinedNodes nodes = ScriptableObject.CreateInstance<PredefinedNodes>();
						PredefinedScenery scenery = ScriptableObject.CreateInstance<PredefinedScenery>();
						var node1 = new NodeData();
						var node2 = new TradePeltsNodeData();
						nodes.nodeRows.Add(new List<NodeData>() { node1 });
						nodes.nodeRows.Add(new List<NodeData>() { node2 });
						//RunState.Run.map = MapGenerator.GenerateMap(RunState.CurrentMapRegion, 3, 13, nodes, scenery);
						__instance.PredefinedNodes = nodes;
						//RunState.Run.currentNodeId = SaveManager.SaveFile.currentRun.map.RootNode.id;
						Log.LogInfo("You have been EGG'd");
					}

				}
			}
        }


	}
}
