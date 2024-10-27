using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterBabies.Patches
{
    internal class BabySell
    {
        [HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.PlaceItemOnCounter))]
        [HarmonyPrefix]
        static void disableBabyOnSell(object[] __args)
        {
            PlayerControllerB _ = (PlayerControllerB)__args[0];

            if (_.currentlyHeldObjectServer.GetType() != typeof(CaveDwellerAI)) return;


            CaveDwellerPhysicsProp physicsProp = ((CaveDwellerPhysicsProp)_.currentlyHeldObjectServer);
            CaveDwellerAI ai = physicsProp.caveDwellerScript;


            physicsProp.enabled = false;
            ai.enabled = false;

            if (BetterBabies.Instance.config.disableAgentOnSell.Value) ai.agent.enabled = false;

            UnityEngine.Object.DestroyImmediate(ai);

            BetterBabies.Logger.LogDebug("imagine selling a baby");

            return;
        }

        [HarmonyPatch(typeof(CaveDwellerPhysicsProp), nameof(CaveDwellerPhysicsProp.DiscardItem))]
        [HarmonyFinalizer]
        static Exception babySellErrorHandle() { return null; }


        [HarmonyPatch(typeof(CaveDwellerPhysicsProp), nameof(CaveDwellerPhysicsProp.Start))]
        [HarmonyPrefix]
        static void setBabyPrice(CaveDwellerPhysicsProp __instance)
        {
            __instance.itemProperties.isScrap = true;
            BetterBabies.setBabyPrice(__instance);
        }



        /*
        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DiscardItemOnClient))]
        [HarmonyPrefix]
        static bool disableBabyDropOnSell(GrabbableObject __instance)
        {
            if (__instance.GetType() != typeof(CaveDwellerPhysicsProp)) return true;
            if (!__instance.IsOwner || __instance.enabled) return true;

            HUDManager.Instance.ClearControlTips();
            __instance.SyncBatteryServerRpc((int)(__instance.insertedBattery.charge * 100f));

            return false;
        }*/
    }
}
