using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterBabies.Patches
{
    internal class BabySell
    {

        // cleanup
        [HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.PlaceItemOnCounter))]
        [HarmonyPrefix]
        static bool disableBabyOnSell(object[] __args)
        {

            PlayerControllerB player = (PlayerControllerB)__args[0];

            if (player.currentlyHeldObjectServer.GetType() != typeof(CaveDwellerPhysicsProp)) return true;
            if (!ConfigManager.CanSellBaby.Value) return false;


            CaveDwellerPhysicsProp caveDwellerPhysicsProp = (CaveDwellerPhysicsProp)player.currentlyHeldObjectServer;
            CaveDwellerAI caveDwellerAI = caveDwellerPhysicsProp.caveDwellerScript;

            BetterBabies.setBabyPrice(caveDwellerPhysicsProp);

            caveDwellerPhysicsProp.enabled = false;

            caveDwellerAI.enabled = false;
            caveDwellerAI.agent.enabled = false;

            // messes up Networking
            //UnityEngine.Object.DestroyImmediate(ai);


            return true;
        }

        [HarmonyPatch(typeof(CaveDwellerPhysicsProp), nameof(CaveDwellerPhysicsProp.DiscardItem))]
        [HarmonyFinalizer]
        static Exception babySellErrorHandle() { return null; }





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
