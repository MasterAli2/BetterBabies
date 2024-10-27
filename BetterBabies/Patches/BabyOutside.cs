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

        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.BabyUpdate))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> BabyOutsideTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            if (!BetterBabies.Instance.config.CanBabyGoOutside.Value)
            {
                foreach(var _ in instructions)
                {
                    yield return _;
                }
                yield break;
            }

            var targetField = AccessTools.Field(typeof(EnemyAI), "isOutside");

            var foundFirst = false;
            var foundTarget = false;

            var localIsOutside = il.DeclareLocal(typeof(bool));

            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand as FieldInfo == targetField)
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

                        yield return new CodeInstruction(OpCodes.Ldfld, targetField);

                        yield return new CodeInstruction(OpCodes.Stloc, localIsOutside);



                        // isOutside = false
                        yield return new CodeInstruction(OpCodes.Ldarg_0); 

                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);

                        yield return new CodeInstruction(OpCodes.Stfld, targetField);



                        // if (isOutside && !isInsidePlayerShip && !babyCrying)
                        yield return instruction;

                        // isOutside = localIsOutside
                        yield return new CodeInstruction(OpCodes.Ldarg_0);

                        yield return new CodeInstruction(OpCodes.Ldloc, localIsOutside);

                        yield return new CodeInstruction(OpCodes.Stfld, targetField);
                        continue;
                    }
                }
                yield return instruction;
            }
        }

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
