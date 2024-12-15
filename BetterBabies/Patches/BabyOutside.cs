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
    internal class BabyOutside
    {
        static bool wasOutside;


        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.BabyUpdate))]
        [HarmonyPrefix]
        static void babyOutsidePrefix(CaveDwellerAI __instance)
        {
            wasOutside = __instance.isOutside;
            __instance.isOutside = false;
        }

        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.BabyUpdate))]
        [HarmonyPostfix]
        static void babyOutsidePostfix(CaveDwellerAI __instance) => __instance.isOutside = wasOutside;



        /*
        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.BabyUpdate))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> BabyOutsideTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {

            Label configJumpLabel = il.DefineLabel();

            var isOutside = AccessTools.Field(typeof(EnemyAI), "isOutside");

            var config = AccessTools.Field(typeof(ConfigManager), "CanBabyGoOutside");
            //var configValueObj = AccessTools.Field(typeof(BepInEx.Configuration.ConfigEntry<bool>), "Value");
            var configValue = AccessTools.Property(typeof(CSync.Lib.SyncedEntry<bool>), "Value");

            var foundFirst = false;
            var foundTarget = false;

            var wasOutside = il.DeclareLocal(typeof(bool));

            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand as FieldInfo == isOutside)
                {
                    if (foundTarget)
                    {
                        yield return instruction;
                        continue;
                    }

                    if (!foundFirst)
                    {
                        foundFirst = true;
                    }
                    else
                    {
                        foundTarget = true;

                        // bool localIsOutside = isOutside
                        yield return new CodeInstruction(OpCodes.Ldarg_0);

                        yield return new CodeInstruction(OpCodes.Ldfld, isOutside);

                        yield return new CodeInstruction(OpCodes.Stloc, wasOutside);

                        // if (!ConfigManager.CanBabyGoOutside) goto jumpIfFalse;
                        yield return new CodeInstruction(OpCodes.Ldsfld, config);
                        yield return new CodeInstruction(OpCodes.Callvirt, configValue.GetGetMethod());

                        yield return new CodeInstruction(OpCodes.Brfalse, configJumpLabel);


                        // isOutside = false
                        yield return new CodeInstruction(OpCodes.Ldarg_0); 

                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);

                        yield return new CodeInstruction(OpCodes.Stfld, isOutside);


                        // jumpIfFalse:
                        // if (isOutside && !isInsidePlayerShip && !babyCrying)
                        var _ = instruction.Clone();
                        _.labels.Add(configJumpLabel);
                        yield return _;

                        // isOutside = localIsOutside
                        yield return new CodeInstruction(OpCodes.Ldarg_0);

                        yield return new CodeInstruction(OpCodes.Ldloc, wasOutside);

                        yield return new CodeInstruction(OpCodes.Stfld, isOutside);
                        continue;
                    }
                }
                yield return instruction;
            }
            
        }
        */
        /*
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.UnloadSceneObjectsEarly))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> DontDespawnBabyTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {

            var conditionMethod = AccessTools.Method(typeof(BetterBabies), nameof(BetterBabies.DontDespawnBabyCondition));

            int timesFound2 = 0;
            int timesFound1 = 0;

            var continueLabel = il.DefineLabel();


            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldloc_3)
                {
                    timesFound1++;

                    if(timesFound1 == 4)
                    {
                        instruction.labels.Add(continueLabel);
                    }
                    
                    if(timesFound1 == 5)
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_0);
                        yield return new CodeInstruction(OpCodes.Ldloc_3);

                        yield return new CodeInstruction(OpCodes.Ldelem);

                        yield return new CodeInstruction(OpCodes.Call, conditionMethod);
                        yield return new CodeInstruction(OpCodes.Brtrue, continueLabel);
                    }
                    
                }
                



                yield return instruction;
                if (instruction.opcode == OpCodes.Ldloc_0)
                {
                    timesFound2++;

                    if (timesFound2 == 2)
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_0);

                        yield return new CodeInstruction(OpCodes.Ldloc_3);
                        yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                        yield return new CodeInstruction(OpCodes.Sub);

                        yield return new CodeInstruction(OpCodes.Ldelem_Ref);

                        yield return new CodeInstruction(OpCodes.Call, conditionMethod);
                        yield return new CodeInstruction(OpCodes.Brtrue_S, continueLabel);
                    }
                }
        */

    }

}
