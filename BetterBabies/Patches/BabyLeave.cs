using GameNetcodeStuff;
using HarmonyLib;
using Steamworks.Ugc;
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

        /*
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.UnloadSceneObjectsEarly))]
        [HarmonyTranspiler]
        [HarmonyDebug]
        static IEnumerable<CodeInstruction> DontDespawnBaby(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {

            Label skipCustomLogic = il.DefineLabel();

            Label skipLoopIter = il.DefineLabel();

            // CaveDwellerAI castedEnemyObj;
            var castedEnemyObj = il.DeclareLocal(typeof(CaveDwellerAI));

            var shouldBabyPersist = AccessTools.Method(typeof(BetterBabies), nameof(BetterBabies.shouldBabyPersist));

            var getModInstance = AccessTools.Property(typeof(BetterBabies), nameof(BetterBabies.Instance)).GetGetMethod();

            var babiesInShip = AccessTools.Field(typeof(BetterBabies), nameof(BetterBabies.babiesInShip));

            var addItemToList = AccessTools.Method(typeof(List<CaveDwellerAI>), nameof(List<CaveDwellerAI>.Add));

            var caveDwellerProp = AccessTools.Field(typeof(CaveDwellerAI), nameof(CaveDwellerAI.propScript));



            bool foundLoopStart = false;
            bool foundLoopStart_ = false;

            bool foundLoopEnd = false;
            int timesFoundLdloc3 = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                var _ = instruction.Clone();

                if (foundLoopStart_)
                {
                    foundLoopStart_ = false;
                    BetterBabies.Logger.LogDebug("found skip 1");
                    _.labels.Add(skipCustomLogic);
                }
                if (instruction.opcode == OpCodes.Ldloc_3)
                {
                    timesFoundLdloc3++;
                    if (timesFoundLdloc3 == 4)
                    {
                        _.labels.Add(skipLoopIter);
                        BetterBabies.Logger.LogDebug(instruction);
                    }

                }

                yield return _;


                if (!foundLoopStart && instruction.opcode == OpCodes.Br)
                {
                    foundLoopStart = true;
                    foundLoopStart_ = true;


                    // if (array[num] is CaveDwellerAI)
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(OpCodes.Ldelem_Ref);
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(CaveDwellerAI));
                    yield return new CodeInstruction(OpCodes.Brfalse, skipCustomLogic);

                    // castedEnemyObj = (CaveDwellerAI)array[num];
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(OpCodes.Ldelem_Ref);
                    yield return new CodeInstruction(OpCodes.Castclass, typeof(CaveDwellerAI));
                    yield return new CodeInstruction(OpCodes.Stloc, castedEnemyObj);

                    // if (shouldBabyPersist(castedEnemyObj))
                    yield return new CodeInstruction(OpCodes.Ldloc, castedEnemyObj);
                    yield return new CodeInstruction(OpCodes.Call, shouldBabyPersist);
                    yield return new CodeInstruction(OpCodes.Brfalse, skipCustomLogic);

                    // Instance.babiesInShip.Add(casyedEnemyObj);
                    yield return new CodeInstruction(OpCodes.Call, getModInstance);
                    yield return new CodeInstruction(OpCodes.Ldfld, babiesInShip);
                    yield return new CodeInstruction(OpCodes.Ldloc, castedEnemyObj);
                    yield return new CodeInstruction(OpCodes.Callvirt, addItemToList);

                    // Instance.babyItemsInShip.Add(castedEnemyObj.propScript);
                    yield return new CodeInstruction(OpCodes.Call, getModInstance);
                    yield return new CodeInstruction(OpCodes.Ldfld, babiesInShip);
                    yield return new CodeInstruction(OpCodes.Ldloc, castedEnemyObj);
                    yield return new CodeInstruction(OpCodes.Ldfld, caveDwellerProp);
                    yield return new CodeInstruction(OpCodes.Callvirt, addItemToList);

                    // continue;
                    yield return new CodeInstruction(OpCodes.Br, skipLoopIter);

                }
            }
        }
        */

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

                    if (BetterBabies.shouldBabyPersist(_))
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

                __instance.SetCryingLocalClient(false);
                __instance.SetBabyCryingServerRpc(false);
                __instance.SetBabyRunningServerRpc(false);

                __instance.growthMeter = 0f;
                return false;
            }
            return true;
        }





    }
}
