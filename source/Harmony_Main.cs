using HarmonyLib;
using Verse;

namespace RIMDAY
{
    [StaticConstructorOnStartup]
    public static class RIMDAY_Main_Startup
    {
        static RIMDAY_Main_Startup()
        {
            new Harmony("rimday.main").PatchAll();
        }
    }
}
