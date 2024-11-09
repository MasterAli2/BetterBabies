using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace BetterBabies.Patches
{
    

    internal class BabyLeave
    {
        public static bool wasLeaving = false;

        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.BabyUpdate))]
        [HarmonyPrefix]
        static void BabyUpdateIsLeavingPrefix()
        {
            if (!ConfigManager.CanBabyGoIntoOrbit.Value) return;

            wasLeaving = StartOfRound.Instance.shipIsLeaving;
            StartOfRound.Instance.shipIsLeaving = false;
        }

        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.BabyUpdate))]
        [HarmonyPostfix]
        static void BabyUpdateIsLeavingPostfix()
        {
            if (!ConfigManager.CanBabyGoIntoOrbit.Value) return;

            StartOfRound.Instance.shipIsLeaving = wasLeaving;
            wasLeaving = false;
        }





        // Yes i know this is bad pratice but im not making another transpiler
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.UnloadSceneObjectsEarly))]
        [HarmonyPrefix]
        static bool DontDespawnBaby(RoundManager __instance)
        {
            if (!ConfigManager.CanBabyGoIntoOrbit.Value) return true;

            if (!__instance.IsServer)
            {
                return false;
            }
            Debug.Log("Despawning props and enemies #3");
            __instance.isSpawningEnemies = false;
            EnemyAI[] array = UnityEngine.Object.FindObjectsOfType<EnemyAI>();
            Debug.Log($"Enemies on map: {array.Length}");
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].GetType() == typeof(CaveDwellerAI))
                {
                    CaveDwellerAI _ = (CaveDwellerAI)array[i];
                    BetterBabies.Logger.LogDebug($"Found a wild baby!");

                    if (_.propScript.isHeld && _.propScript.playerHeldBy.isInHangarShipRoom)
                    {
                        BetterBabies.Logger.LogDebug($"Kidnaping the wild baby!");

                        BetterBabies.Instance.babiesInShip.Add(_);
                        BetterBabies.Instance.babyItemsInShip.Add(_.propScript);

                        continue;
                    }
                }

                if (array[i].thisNetworkObject.IsSpawned)
                {
                    Debug.Log("despawn enemies on map");
                    array[i].thisNetworkObject.Despawn();
                }
                else
                {
                    Debug.Log($"{array[i].thisNetworkObject} was not spawned on network, so it could not be removed.");
                }
            }
            __instance.SpawnedEnemies.Clear();
            EnemyAINestSpawnObject[] array2 = UnityEngine.Object.FindObjectsByType<EnemyAINestSpawnObject>(FindObjectsSortMode.None);
            for (int j = 0; j < array2.Length; j++)
            {
                NetworkObject component = array2[j].gameObject.GetComponent<NetworkObject>();
                if (component != null && component.IsSpawned)
                {
                    Debug.Log("despawn nest spawn object");
                    component.Despawn();
                }
                else
                {
                    UnityEngine.Object.Destroy(array2[j].gameObject);
                }
            }
            __instance.currentEnemyPower = 0f;
            __instance.currentDaytimeEnemyPower = 0f;
            __instance.currentOutsideEnemyPower = 0f;

            return false;
        }


        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.StartGame))]
        [HarmonyPrefix]
        static void RespawnBaby(StartOfRound __instance)
        {
            if (!ConfigManager.CanBabyGoIntoOrbit.Value) return;

            if (!__instance.IsServer && !__instance.inShipPhase) return;

            foreach (CaveDwellerAI baby in BetterBabies.Instance.babiesInShip.ToList())
            {
                if (baby == null) continue;

                baby.SetCryingLocalClient(false);
                baby.SetBabyCryingServerRpc(false);
                baby.growthMeter = 0f;
                baby.nearTransforming = false;

                BetterBabies.Instance.babiesInShip.Remove(baby);
            }

            foreach (CaveDwellerPhysicsProp baby in BetterBabies.Instance.babyItemsInShip.ToList())
            {
                if (baby == null) continue;

                BetterBabies.Instance.babyItemsInShip.Remove(baby);
            }
        }



        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.IncreaseBabyGrowthMeter))]
        [HarmonyPrefix]
        static bool disableBabyGrowth(CaveDwellerAI __instance)
        {
            if (!ConfigManager.CanBabyGoIntoOrbit.Value) return true;

            if (BetterBabies.babyInShipState(__instance))
            {
                if (__instance.growthMeter == 0 || !__instance.babyCrying) return true;

                __instance.SetCryingLocalClient(false);
                __instance.SetBabyCryingServerRpc(false);

                __instance.growthMeter = 0f;
                return false;
            }
            return true;
        }





    }
}
