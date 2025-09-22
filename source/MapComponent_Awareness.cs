using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace RIMDAY
{
    public class MapComponent_BankGuardAwareness : MapComponent
    {
        public MapComponent_BankGuardAwareness(Map map) : base(map) { }

        public override void MapComponentTick()
        {
            // check every second
            if (Find.TickManager.TicksGame % 60 != 0)
                return;

            // only run on bank maps
            if (!IsBankMap(map))
                return;

            // get all pawns on map
            foreach (var pawn in map.mapPawns.AllPawnsSpawned)
            {
                var guardComp = pawn.TryGetComp<CompBankGuard>();
                if (guardComp == null || !guardComp.IsBankGuard)
                    continue;

                if (!pawn.Spawned || pawn.Dead || pawn.Downed)
                    continue;

                if (CanSeeDeadOrDowned(pawn))
                {
                    TriggerHostility(pawn);
                }
            }
        }

        private bool CanSeeDeadOrDowned(Pawn guard)
        {
            float range = 10f;

            foreach (var target in map.mapPawns.AllPawnsSpawned)
            {
                if (target == guard) continue;
                if (!target.Dead && !target.Downed) continue;

                if (guard.Position.DistanceTo(target.Position) > range) continue;

                if (GenSight.LineOfSight(guard.Position, target.Position, map))
                    return true;
            }
            return false;
        }

        private void TriggerHostility(Pawn guard)
        {
            if (guard.Faction != null && !guard.Faction.HostileTo(Faction.OfPlayer))
            {
                Lord lord = guard.GetLord();
                lord.ReceiveMemo("RIMDAY_BodySpotted");
                Messages.Message($"[RIMDAY] Guard {guard.LabelShort} spotted a body!", MessageTypeDefOf.ThreatBig, historical: true);
            }
        }

        private bool IsBankMap(Map map)
        {
            if (map.Parent is Site site)
            {
                foreach (var part in site.parts)
                {
                    if (part.def?.defName == "m_Bank")
                        return true;
                }
            }
            return false;
        }
    }
}
