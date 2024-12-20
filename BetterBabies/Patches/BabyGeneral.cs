﻿using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BetterBabies.Patches
{
    internal class BabyGeneral
    {

        public static bool wasTwoHanded = false;


        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.TransformIntoAdult))]
        [HarmonyPrefix]
        static bool DisableTransformOfBaby(CaveDwellerAI __instance)
        {
            if (BetterBabies.babyInShipState(__instance)) return false;

            DepositItemsDesk companyDesk = UnityEngine.Object.FindObjectOfType<DepositItemsDesk>();

            if (companyDesk is not null && companyDesk.itemsOnCounter.Contains(__instance.propScript)) return false;

            if (!ConfigManager.CanBabyGrowUp.Value) return false;
        

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

        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.IncreaseBabyGrowthMeter))]
        [HarmonyPostfix]
        static void babyGrowPostfix(CaveDwellerAI __instance)
        {
            CaveDwellerAI baby = __instance;

            if (!ConfigManager.CanBabyGrowUp.Value)
            {
                baby.growthMeter = 0f;
                baby.nearTransforming = false;
            }

            if (ConfigManager.DecreaseBabyGrowthMeter.Value && !baby.babyCrying && !baby.nearTransforming)
            {
                baby.growthMeter = Mathf.Clamp((float)(baby.growthMeter - ConfigManager.BabyGrowthMeterDecreasePersecond * Time.deltaTime), 0f, 25f);
            }
            //BetterBabies.Logger.LogDebug($"Growth Meter: {baby.growthMeter}");
        }
    }
}
