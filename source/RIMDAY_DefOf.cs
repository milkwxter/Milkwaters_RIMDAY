using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RIMDAY
{
    [DefOf]
    public static class RIMDAY_ClamorDefOf
    {
        public static ClamorDef Gunshot;

        static RIMDAY_ClamorDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RIMDAY_ClamorDefOf));
        }
    }

    public class DefModExtension_Suppressed : DefModExtension
    {
        // LOL
    }
}
