using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RIMDAY
{
    public class Trigger_LoudGunshotClamor : Trigger
    {
        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            return signal.type == TriggerSignalType.Clamor && signal.clamorType == RIMDAY_ClamorDefOf.Gunshot;
        }
    }
}
