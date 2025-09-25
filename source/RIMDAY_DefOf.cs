using RimWorld;
using Verse;
using Verse.AI;

namespace RIMDAY
{
    [DefOf]
    public static class RIMDAY_ClamorDefOf
    {
        public static ClamorDef RIMDAY_Gunshot;

        static RIMDAY_ClamorDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RIMDAY_ClamorDefOf));
        }
    }

    [DefOf]
    public static class RIMDAY_JobDefOf
    {
        public static JobDef DrillVaultDoor;

        static RIMDAY_JobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RIMDAY_JobDefOf));
        }
    }

    public class DefModExtension_Suppressed : DefModExtension
    {
        // doesnt need to do anything
    }
}
