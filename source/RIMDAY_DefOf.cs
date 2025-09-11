using RimWorld;
using Verse;

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

    public class DefModExtension_Suppressed : DefModExtension
    {
        // doesnt need to do anything
    }
}
