using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using Verse;

namespace RIMDAY
{
    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch(nameof(Faction.TryAffectGoodwillWith),
        new Type[] {
            typeof(Faction),
            typeof(int),
            typeof(bool),
            typeof(bool),
            typeof(HistoryEventDef),
            typeof(Nullable<GlobalTargetInfo>)
        })]
    public static class Patch_RIMDAY_TryAffectGoodwillWith
    {
        static void Prefix(Faction __instance,  Faction other, ref int goodwillChange, bool canSendMessage, bool canSendHostilityLetter, HistoryEventDef reason, GlobalTargetInfo? lookTarget)
        {
            Map map = Find.CurrentMap;
            if (map == null)
                return;

            if (!IsBankMap(map))
                return;

            // let the alarm sounding actually trigger
            if (reason != null && reason.defName == "RIMDAY_BankHeistEscalation")
                return;

            if (goodwillChange < 0)
            {
                goodwillChange = 0;
                Log.Message($"[RIMDAY] Neutralized goodwill loss on bank map. Reason='{reason?.defName ?? "(null)"}', Target='{lookTarget?.ToString() ?? "(null)"}'.");
            }
        }

        private static bool IsBankMap(Map map)
        {
            if (map.Parent is Site site && site.parts != null)
            {
                foreach (var part in site.parts)
                {
                    if (part?.def?.defName == "m_Bank")
                        return true;
                }
            }
            return false;
        }
    }
}
