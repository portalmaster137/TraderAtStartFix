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


        public bool GetSuperSecretConfig()
        {
            return Config.Bind("TraderAtStart", "SuperSecretOption", false, new BepInEx.Configuration.ConfigDescription("dont use this unless you wanna have a bad time")).Value;
        }

        public bool GetUsePercents()
        {
            return Config.Bind("TraderAtStart", "UsePercents", false, new BepInEx.Configuration.ConfigDescription("use percents instead of the default chances")).Value;
        }

        public int GetHareCount()
        {
            return Config.Bind("TraderAtStart", "RabbitCount", 4, new BepInEx.Configuration.ConfigDescription("how many pelts are guaranteed")).Value;
        }

        public int GetHareOptional()
        {
            return Config.Bind("TraderAtStart", "RabbitCountOptional", 0, new BepInEx.Configuration.ConfigDescription("max number of additional pelts given")).Value;
        }

        public int GetHareChance()
        {
            return Config.Bind("TraderAtStart", "RabbitChance", 2, new BepInEx.Configuration.ConfigDescription("chance to get 1 additonal pelt (0 for never)")).Value;
        }

        public double GetHarePercent()
        {
            return Config.Bind("TraderAtStart", "RabbitPercentChance", 0.0, new BepInEx.Configuration.ConfigDescription("chance to get an extra pelt as a percent out of 100 (0 for never)")).Value;
        }

        public int GetWolfCount()
        {
            return Config.Bind("TraderAtStart", "WolfCount", 0, new BepInEx.Configuration.ConfigDescription("how many pelts are guaranteed")).Value;
        }

        public int GetWolfOptional()
        {
            return Config.Bind("TraderAtStart", "WolfCountOptional", 1, new BepInEx.Configuration.ConfigDescription("max number of additional pelts given")).Value;
        }

        public int GetWolfChance()
        {
            return Config.Bind("TraderAtStart", "WolfChance", 3, new BepInEx.Configuration.ConfigDescription("chance to get 1 additonal pelt (0 for never)")).Value;
        }

        public double GetWolfPercent()
        {
            return Config.Bind("TraderAtStart", "WolfPercentChance", 0.0, new BepInEx.Configuration.ConfigDescription("chance to get an extra pelt as a percent out of 100 (0 for never)")).Value;
        }

        public int GetGoldenCount()
        {
            return Config.Bind("TraderAtStart", "GoldenCount", 0, new BepInEx.Configuration.ConfigDescription("how many pelts are guaranteed")).Value;
        }

        public int GetGoldenOptional()
        {
            return Config.Bind("TraderAtStart", "GoldenCountOptional", 1, new BepInEx.Configuration.ConfigDescription("max number of additional pelts given")).Value;
        }

        public int GetGoldenChance()
        {
            return Config.Bind("TraderAtStart", "GoldenChance", 10, new BepInEx.Configuration.ConfigDescription("chance to get 1 additonal pelt (0 for never)")).Value;
        }

        public double GetGoldenPercent()
        {
            return Config.Bind("TraderAtStart", "GoldenPercentChance", 0.0, new BepInEx.Configuration.ConfigDescription("chance to get an extra pelt as a percent out of 100 (0 for never)")).Value;
        }

        public double GetAbsoluteChanceHare()
        {
            if (GetUsePercents())
            {
                return GetHarePercent() / 100;
            }

            if (GetHareChance() > 0)
            {
                return 1.0 / GetHareChance();
            }
            return 0.0;
        }

        public double GetAbsoluteChanceWolf()
        {
            if (GetUsePercents())
            {
                return GetWolfPercent() / 100;
            }

            if (GetWolfChance() > 0)
            {
                return 1.0 / GetWolfChance();
            }
            return 0.0;
        }

        public double GetAbsoluteChanceGolden()
        {
            if (GetUsePercents())
            {
                return GetGoldenPercent() / 100;
            }

            if (GetGoldenChance() > 0)
            {
                return 1.0 / GetGoldenChance();
            }
            return 0.0;
        }


        private const string PluginGuid = "porta.inscryption.traderstart";
        private const string PluginName = "TraderAtStart";
        private const string PluginVersion = "1.2.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            Logger.LogInfo($"Loading {PluginName}!");
            Plugin.Log = Logger;

            // should generate the config as soon as the game boots
            GetSuperSecretConfig();
            GetUsePercents();
            GetHareCount();
            GetHareChance();
            GetHareOptional();
            GetHarePercent();
            GetWolfCount();
            GetWolfChance();
            GetWolfOptional();
            GetWolfPercent();
            GetGoldenCount();
            GetGoldenChance();
            GetGoldenOptional();
            GetGoldenPercent();

            Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(DeckInfo), "InitializeAsPlayerDeck")]
        public class TraderStartPatch : DeckInfo
        {
            private static readonly System.Random r = new System.Random();

            [HarmonyPrefix]
            public static bool Prefix(ref DeckInfo __instance)
            {
                Plugin c = new Plugin();

                if (c.GetSuperSecretConfig())
                {
                    for (int i = 0; i < c.GetHareCount(); i++)
                    {
                        __instance.AddCard(CardLoader.GetCardByName("PeltGolden"));
                    }
                }
                else
                {
                    PopulatePelts(ref __instance, "PeltHare", c.GetHareCount(), c.GetHareOptional(), c.GetAbsoluteChanceHare());
                    PopulatePelts(ref __instance, "PeltWolf", c.GetWolfCount(), c.GetWolfOptional(), c.GetAbsoluteChanceWolf());
                    PopulatePelts(ref __instance, "PeltGolden", c.GetGoldenCount(), c.GetGoldenOptional(), c.GetAbsoluteChanceGolden());
                }

                return false;
            }

            private static void PopulatePelts(ref DeckInfo __instance, string peltName, int count, int countOptional, double chance)
            {
                for (int i = 0; i < count; i++)
                {
                    __instance.AddCard(CardLoader.GetCardByName(peltName));
                }
                if (chance > 0.0)
                {
                    for (int j = 0; j < countOptional; j++)
                    {
                        if (r.NextDouble() < chance)
                        {
                            __instance.AddCard(CardLoader.GetCardByName(peltName));
                        }
                    }
                }
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
                    if (c.GetSuperSecretConfig())
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
                    }
                    else
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
                        Plugin.Log.LogInfo("You have been EGG'd");
                    }
                }
            }
        }
    }
}
