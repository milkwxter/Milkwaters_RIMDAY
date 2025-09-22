using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;


namespace RIMDAY
{
    public class QuestPart_MBankUtilities : QuestPartActivable
    {
        public MapParent mapParent;
        public string outSignal;
        private bool signalSent;
        private int hostilesActive;
        private int colonistsActive;

        public override void QuestPartTick()
        {
            // only send once
            if (signalSent) return;

            // get map
            Map mapToCheck = mapParent?.Map;

            // check if it exists
            if (mapToCheck != null)
            {
                // check if hostiles are still here
                hostilesActive = mapToCheck.mapPawns.AllPawnsSpawned.Count(p => !p.DeadOrDowned && p.HostileTo(Faction.OfPlayer));

                // check if player has guys still
                colonistsActive = mapToCheck.mapPawns.AllPawnsSpawned.Count(p => !p.DeadOrDowned && p.Faction == Faction.OfPlayer);
            }
            else
            {
                // map is gone
                signalSent = true;

                // did we win
                if (hostilesActive <= 0 && colonistsActive > 0)
                {
                    quest.End(QuestEndOutcome.Success);
                }
                else
                {
                    quest.End(QuestEndOutcome.Fail);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref mapParent, "mapParent");
            Scribe_Values.Look(ref outSignal, "outSignal");
            Scribe_Values.Look(ref signalSent, "signalSent");
            Scribe_Values.Look(ref hostilesActive, "hostilesActive");
        }
    }
}