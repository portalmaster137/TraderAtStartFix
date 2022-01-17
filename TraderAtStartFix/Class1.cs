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
            return Config.Bind("TraderAtStart", "RabbitCount", 4, new BepInEx.Configuration.ConfigDescription("how many pelts are guaranteed")).Value;
        }

        public int getHareOptional()
        {
            return Config.Bind("TraderAtStart", "RabbitCountOptional", 0, new BepInEx.Configuration.ConfigDescription("max number of additional pelts given")).Value;
        }

        public int getHareChance()
        {
            return Config.Bind("TraderAtStart", "RabbitChance", 2, new BepInEx.Configuration.ConfigDescription("chance to get 1 additonal pelt (0 for never)")).Value;
        }

        public int getWolf()
        {
            return Config.Bind("TraderAtStart", "WolfCount", 0, new BepInEx.Configuration.ConfigDescription("how many pelts are guaranteed")).Value;
        }

        public int getWolfOptional()
        {
            return Config.Bind("TraderAtStart", "WolfCountOptional", 1, new BepInEx.Configuration.ConfigDescription("max number of additional pelts given")).Value;
        }

        public int getWolfChance()
        {
            return Config.Bind("TraderAtStart", "WolfChance", 3, new BepInEx.Configuration.ConfigDescription("chance to get 1 additonal pelt (0 for never)")).Value;
        }

        public int getGolden()
        {
            return Config.Bind("TraderAtStart", "GoldenCount", 0, new BepInEx.Configuration.ConfigDescription("how many pelts are guaranteed")).Value;
        }

        public int getGoldenOptional()
        {
            return Config.Bind("TraderAtStart", "GoldenCountOptional", 1, new BepInEx.Configuration.ConfigDescription("max number of additional pelts given")).Value;
        }

        public int getGoldenChance()
        {
            return Config.Bind("TraderAtStart", "GoldenChance", 10, new BepInEx.Configuration.ConfigDescription("chance to get 1 additonal pelt (0 for never)")).Value;
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
            getSuperSecretConfig();
            getHare();
            getHareChance();
            getHareOptional();
            getWolf();
            getWolfChance();
            getWolfOptional();
            getGolden();
            getGoldenChance();
            getGoldenOptional();

            Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(DeckInfo), "InitializeAsPlayerDeck")]
        public class TraderStartPatch : DeckInfo
        {
            [HarmonyPrefix]
            public static bool Prefix(ref DeckInfo __instance)
            {
                Log.LogInfo("Prefix init");
                Plugin c = new Plugin();

                if (c.getSuperSecretConfig())
                {
                    for (int i = 0; i < c.getHare(); i++)
                    {
                        __instance.AddCard(CardLoader.GetCardByName("PeltGolden"));
                    }
                }
                else
                {
                    GenerateDeck(ref __instance, "PeltHare", c.getHare(), c.getHareOptional(), c.getHareChance());
                    GenerateDeck(ref __instance, "PeltWolf", c.getWolf(), c.getWolfOptional(), c.getWolfChance());
                    GenerateDeck(ref __instance, "PeltGolden", c.getGolden(), c.getGoldenOptional(), c.getGoldenChance());
                }

                return false;
            }

            private static void GenerateDeck(ref DeckInfo __instance, string peltName, int count, int countOptional, int chance)
            {
                for (int i = 0; i < count; i++)
                {
                    __instance.AddCard(CardLoader.GetCardByName(peltName));
                }
                if (chance > 0)
                {
                    for (int j = 0; j < countOptional; j++)
                    {
                        System.Random r = new System.Random();

                        int randInt = SeededRandom.Range(1, chance, r.GetHashCode());

                        if (randInt == 1)
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
