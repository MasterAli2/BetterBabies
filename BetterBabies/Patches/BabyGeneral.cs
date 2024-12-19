using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterBabies.Patches
{
    internal class BabyGeneral
    {

        public static bool wasTwoHanded = false;


        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.TransformIntoAdult))]
        [HarmonyPrefix]
        static bool DisableTransformOfBaby(CaveDwellerAI __instance)
        {
            if (false) return false;

            if (BetterBabies.babyInShipState(__instance)) return false;

            DepositItemsDesk companyDesk = UnityEngine.Object.FindObjectOfType<DepositItemsDesk>();

            if (companyDesk == null) return true;
            if (companyDesk.itemsOnCounter.Contains(__instance.propScript)) return false;

            return true;
        }

        /*
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DiscardHeldObject))]
        [HarmonyPrefix]
        static bool dontDropBabyinShip(PlayerControllerB __instance)
        {
            if (!BetterBabies.Instance.babiesInShip.Contains(((CaveDwellerPhysicsProp)__instance.currentlyHeldObjectServer).caveDwellerScript)) return true;

            return false;
        }*/


        #region baby scroll

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.ScrollMouse_performed))]
        [HarmonyPrefix]
        static void enableBabyScrollPrefix(PlayerControllerB __instance)
        {
            if (!__instance.IsOwner) return;

            if (__instance.currentlyHeldObjectServer == null) return;
            if (__instance.currentlyHeldObjectServer.itemProperties.itemId != BetterBabies.babyItemId) return;

            __instance.twoHanded = false;
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.ScrollMouse_performed))]
        [HarmonyPostfix]
        static void enableBabyScrollPostfix(PlayerControllerB __instance)
        {
            if (!__instance.IsOwner) return;

            if (__instance.currentlyHeldObjectServer == null) return;
            if (__instance.currentlyHeldObjectServer.itemProperties.itemId != BetterBabies.babyItemId) return;

            __instance.twoHanded = true;
        }
        #endregion


        #region baby 2 handed
        // FIX: make it target ClickHoldInteraction()

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
        [HarmonyPrefix]
        static void disable2HandedInteractionsIfHoldingBabyPrefix(PlayerControllerB __instance)
        {

            if (BetterBabies.Instance.babyInInv && StartOfRound.Instance.shipHasLanded)
            {
                wasTwoHanded = __instance.twoHanded;
                __instance.twoHanded = true;
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
        [HarmonyPostfix]
        static void disable2HandedInteractionsIfHoldingBabyPostfix(PlayerControllerB __instance)
        {

            if (BetterBabies.Instance.babyInInv && StartOfRound.Instance.shipHasLanded)
            {
                __instance.twoHanded = wasTwoHanded;
                wasTwoHanded = false;
            }
        }
        #endregion

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SwitchToItemSlot))]
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DiscardHeldObject))]
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.BeginGrabObject))]
        [HarmonyPrefix]
        static void updateBabyInInvState(PlayerControllerB __instance)
        {
            BetterBabies.Logger.LogDebug("Updating baby inventory state");
            BetterBabies.Instance.babyInInventory(__instance);
        }


        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.TransformIntoAdult))]
        [HarmonyPrefix]
        static bool babyTransformPrefix()
        {
            if (!ConfigManager.CanBabyGrowUp.Value) return false;

            return true;
        }

        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.IncreaseBabyGrowthMeter))]
        [HarmonyPostfix]
        static void babyCryPostfix(CaveDwellerAI __instance)
        {
            if (ConfigManager.CanBabyGrowUp.Value) return;
            
            __instance.growthMeter = 0f;
            __instance.nearTransforming = false;
        }
    }
}
