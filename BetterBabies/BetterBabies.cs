using BepInEx;
using BepInEx.Logging;
using BetterBabies.Patches;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;



namespace BetterBabies
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("dev.kittenji.NavMeshInCompany", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.sigurd.csync", "5.0.1")]
    public class BetterBabies : BaseUnityPlugin
    {
        public static BetterBabies Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        internal static ConfigManager config;

        public static List<CodeInstruction> _1x00;

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

            Harmony.PatchAll(typeof(BetterBabies));

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
            System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed);

            __.scrapValue = random.Next(random.Next(ConfigManager.BabyPriceMinInclusive.Value, ConfigManager.BabyPriceMaxExclusive.Value));
        }




        #region aaa
        [HarmonyPatch(typeof(BetterBabies), nameof(BetterBabies.aaa))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> aaa_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            _1x00 = instructions.ToList();
            foreach(var instruction in _1x00)
            {
                if (instruction.opcode == OpCodes.Stloc_3) 
                {
                    _1x00.Remove(instruction);
                    break;
                }

                _1x00.Remove(instruction);
            }
            foreach (var item in instructions) yield return item;

            foreach (var _ in _1x00)
            {
                Logger.LogDebug(_);
            }
        }

        [HarmonyPatch(typeof(BetterBabies), nameof(BetterBabies.aaa))]
        [HarmonyFinalizer]
        static Exception aaa_Finalizer() { return null; }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static void aaa()
        {
            EnemyAI[] array = null; //stloc 0
            bool _1 = false; //stloc 1
            bool _2 = false; //stloc 2
            int i = 0; //stloc 3

            if (array[i].GetType() == typeof(CaveDwellerAI))
            {
                CaveDwellerAI _ = (CaveDwellerAI)array[i];
                BetterBabies.Logger.LogDebug($"Found a wild baby!");

                if (_.propScript.isHeld && _.propScript.playerHeldBy.isInHangarShipRoom)
                {
                    BetterBabies.Logger.LogDebug($"Kidnaping the wild baby!");

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
