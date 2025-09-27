using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Lilly.TryStartCarryNoErr
{
    public static class Patch
    {
        public static HarmonyX harmony = null;
        public static string harmonyId = "Lilly.TryStartCarryNoErr";

        public static void OnPatch(bool repatch = false)
        {
            if (repatch)
            {
                Unpatch();
            }
            if (harmony != null || !Settings.onPatch) return;
            harmony = new HarmonyX(harmonyId);
            try
            {
                harmony.PatchAll();
                MyLog.SUCC();
            }
            catch (System.Exception e)
            {
                MyLog.Error($"Patch Fail");
                MyLog.Error(e.ToString());
                MyLog.Error($"Patch Fail");
            }
            
        }

        public static void Unpatch()
        {
            MyLog.Message($"UnPatch");
            if (harmony == null) return;
            harmony.UnpatchSelf();
            harmony = null;
        }

        [HarmonyPatch(typeof(Toils_Haul), "ErrorCheckForCarry")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MyLog.Message($"ST");
            var codes = new List<CodeInstruction>(instructions);
            var logErrorMethod = AccessTools.Method(typeof(Log), "Error", new[] { typeof(string) });

            for (int i = 0; i < codes.Count; i++)
            {
                //MyLog.Message($"{i} / {codes[i].opcode} / {codes[i].operand}");
                if (codes[i].opcode == OpCodes.Call && codes[i].operand as MethodInfo == logErrorMethod)
                {
                    codes[i].opcode = OpCodes.Pop; // 스택에서 문자열 제거
                    codes[i].operand = null;
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Nop)); // 흐름 유지용

                    MyLog.SUCC();                   
                }
            }

            MyLog.Message($"ED");
            return codes;
        }

    }
}
