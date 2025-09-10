using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace RIMDAY
{
    [StaticConstructorOnStartup]
    public static class Patch_GunshotClamorInjector
    {
        public static bool IsCombatExtendedActive => LoadedModManager.RunningModsListForReading.Any(mod => mod.PackageId == "ceteam.combatextended");

        static Patch_GunshotClamorInjector()
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
                        postfix: new HarmonyMethod(typeof(Patch_GunshotClamorInjector), nameof(Postfix)));
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
                        postfix: new HarmonyMethod(typeof(Patch_GunshotClamorInjector), nameof(Postfix)));
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
                if (weapon != null && !weapon.def.HasModExtension<DefModExtension_Suppressed>())
                {
                    // !TODO: NO ONE CAN HEAR THIS!!!!!
                    GenClamor.DoClamor(pawn, 12f, RIMDAY_ClamorDefOf.Gunshot);
                    Log.Message("UNSUPPRESSED SHOT: Clamor triggered");
                }
                else
                {
                    Log.Message("SUPPRESSED SHOT: No clamor");
                }
            }
        }
    }
}
