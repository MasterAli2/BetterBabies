using BepInEx;
using BepInEx.Logging;
using BetterBabies.Patches;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace BetterBabies
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("dev.kittenji.NavMeshInCompany", BepInDependency.DependencyFlags.HardDependency)]
    public class BetterBabies : BaseUnityPlugin
    {
        public static BetterBabies Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        internal ConfigManager config;

        public const int babyItemId = 123984;

        public List<CaveDwellerAI> babiesInShip = new();
        public List<CaveDwellerPhysicsProp> babyItemsInShip = new();

        public bool babyInInv = false;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            config = new ConfigManager(this);

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll(typeof(BabyOutside));

            if (Instance.config.CanBabyGoIntoOrbit.Value)
            {
                Harmony.PatchAll(typeof(BabyLeave));
            }
            Harmony.PatchAll(typeof(BabyGeneral));
            Harmony.PatchAll(typeof(BabySell));


            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }


        public void babyInInventory(PlayerControllerB __)
        {
            foreach (GrabbableObject _ in __.ItemSlots)
            {
                if (_ == null) continue;

                if (_.itemProperties.itemId == babyItemId)
                {
                    babyInInv = true;
                }
            }
            babyInInv = false;
        }

        public static bool babyInShipState(CaveDwellerAI __)
        {
            if (Instance.babiesInShip.Contains(__) ||
                !StartOfRound.Instance.shipHasLanded
                || StartOfRound.Instance.inShipPhase)
            {
                return true;
            }
            return false;
        }
        
        public static void setBabyPrice(CaveDwellerPhysicsProp __)
        {
            __.scrapValue = Random.Range(Instance.config.BabyPriceMinInclusive.Value, Instance.config.BabyPriceMaxExclusive.Value);
        }
    }
}
