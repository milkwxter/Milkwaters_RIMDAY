using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RIMDAY
{
    [StaticConstructorOnStartup]
    public static class RIMDAY_Debug_Startup
    {
        static RIMDAY_Debug_Startup()
        {
            new Harmony("rimday.debug.clamorlogger").PatchAll();
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.HearClamor))]
    public static class Patch_Pawn_HearClamor
    {
        static void Postfix(Pawn __instance, Thing source, ClamorDef type)
        {
            var lord = __instance.lord;
            string lordInfo = lord != null
                ? $"{lord.LordJob.GetType().Name} (Faction: {lord.faction?.Name ?? "null"})"
                : "No Lord";

            Log.Message($"[ClamorDebug] Pawn '{__instance.LabelShort}' heard clamor '{type.defName}' from '{source?.Label ?? "null"}'. Lord: {lordInfo}");
        }
    }
}
