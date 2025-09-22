using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using Verse.AI;

namespace RIMDAY
{
    [StaticConstructorOnStartup]
    public static class Patch_RIMDAY_GunshotClamorInjector
    {
        public static bool IsCombatExtendedActive => LoadedModManager.RunningModsListForReading.Any(mod => mod.PackageId == "ceteam.combatextended");

        static Patch_RIMDAY_GunshotClamorInjector()
        {
            var harmony = new Harmony("rimday.patch.gunshot");

            if (IsCombatExtendedActive)
            {
                Log.Message("Combat Extended detected — patching Verb_ShootCE");

                var ceVerbType = AccessTools.TypeByName("CombatExtended.Verb_ShootCE");
                var ceMethod = AccessTools.Method(ceVerbType, "TryCastShot");

                if (ceMethod != null)
                {
                    harmony.Patch(ceMethod,
                        postfix: new HarmonyMethod(typeof(Patch_RIMDAY_GunshotClamorInjector), nameof(Postfix)));
                }
                else
                {
                    Log.Error("Failed to find CombatExtended.Verb_ShootCE.TryCastShot");
                }
            }
            else
            {
                Log.Message("Vanilla RimWorld — patching Verb_LaunchProjectile");

                var method = AccessTools.Method(typeof(Verb_LaunchProjectile), "TryCastShot");
                if (method != null)
                {
                    harmony.Patch(method,
                        postfix: new HarmonyMethod(typeof(Patch_RIMDAY_GunshotClamorInjector), nameof(Postfix)));
                }
                else
                {
                    Log.Error("Failed to find Verb_LaunchProjectile.TryCastShot");
                }
            }
        }

        public static void Postfix(Verb __instance)
        {
            if (__instance.Caster is Pawn pawn)
            {
                ThingWithComps weapon = pawn.equipment?.Primary;

                // null check
                if (weapon == null) return;

                // see if the weapon has the suppressed tag
                if (!weapon.def.HasModExtension<DefModExtension_Suppressed>())
                {
                    GenClamor.DoClamor(pawn, 25f, RIMDAY_ClamorDefOf.RIMDAY_Gunshot);
                    Log.Message("UNSUPPRESSED SHOT: Clamor triggered");
                }
                else
                {
                    Log.Message("SUPPRESSED SHOT: No clamor");
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.HearClamor))]
    public static class Patch_RIMDAY_HearClamorInjector
    {
        static void Postfix(Pawn __instance, Thing source, ClamorDef type)
        {
            // finally... a custom clamor
            if (type == RIMDAY_ClamorDefOf.RIMDAY_Gunshot)
            {
                // stop sleeping
                if (__instance.CurJob != null && !__instance.Awake())
                {
                    __instance.jobs.EndCurrentJob(JobCondition.InterruptForced);
                }

                // why is this method private
                MethodInfo notifyMethod = AccessTools.Method(typeof(Pawn), "NotifyLordOfClamor");
                notifyMethod.Invoke(__instance, new object[] { source, type });
            }
        }
    }
}
