using BepInEx;
using BepInEx.Logging;
using BetterBabies.Patches;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;



namespace BetterBabies
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("dev.kittenji.NavMeshInCompany", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.sigurd.csync", BepInDependency.DependencyFlags.HardDependency)]
    public class BetterBabies : BaseUnityPlugin
    {
        public static BetterBabies Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        internal static ConfigManager config;



        public const int babyItemId = 123984;

        public List<CaveDwellerAI> babiesInShip = new();
        public List<CaveDwellerPhysicsProp> babyItemsInShip = new();

        public bool babyInInv = false;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            config = new ConfigManager(base.Config);

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            //Harmony.PatchAll(typeof(BetterBabies));

            Harmony.PatchAll(typeof(BabyOutside));

            Harmony.PatchAll(typeof(BabyLeave));

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
            if (Instance.babiesInShip.Contains(__))
            {
                return true;
            }
            return false;
        }
        
        public static void setBabyPrice(CaveDwellerPhysicsProp __)
        {
            System.Random random = new System.Random((__.caveDwellerScript.thisEnemyIndex + 1) * UnityEngine.Object.FindObjectOfType<Terminal>().groupCredits + babyItemId); ;

            __.scrapValue = random.Next(ConfigManager.BabyPriceMinInclusive.Value, ConfigManager.BabyPriceMaxExclusive.Value);
            __.itemProperties.isScrap = true;
        }


        public static bool shouldBabyPersist(CaveDwellerAI _)
        {
            if (_.propScript.isHeld && _.propScript.playerHeldBy.isInHangarShipRoom && _.currentBehaviourStateIndex == 0) return true;
            return false;
        }


       
        
        #region aaa
        /*
        [HarmonyPatch(typeof(BetterBabies), nameof(BetterBabies.aaa))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> aaa_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            _1x00 = instructions.ToList();
            foreach(CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Stloc_3) 
                {
                    _1x00.Remove(instruction);
                    break;
                }

                _1x00.Remove(instruction);
            }
            yield break;
        }

        [HarmonyPatch(typeof(BetterBabies), nameof(BetterBabies.aaa))]
        [HarmonyFinalizer]
        static Exception aaa_Finalizer() { return null; }*/

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static void aaa()
        {
            EnemyAI[] array = null; //stloc 0
            bool _1 = false; //stloc 1
            bool _2 = false; //stloc 2
            int i = 0; //stloc 3

            if (array[i] is CaveDwellerAI)
            {
                CaveDwellerAI _ = (CaveDwellerAI)array[i];

                if (shouldBabyPersist(_))
                {

                    BetterBabies.Instance.babiesInShip.Add(_);
                    
                    BetterBabies.Instance.babyItemsInShip.Add(_.propScript);

                    aab();
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static void aab() { }

        #endregion
    }
}
