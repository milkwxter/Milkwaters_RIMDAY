using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RIMDAY
{
    public class LordJob_RIMDAYDefendTheBank : LordJob
    {
	    private Faction faction;

        private IntVec3 baseCenter;

        public LordJob_RIMDAYDefendTheBank()
        {
        }

        public LordJob_RIMDAYDefendTheBank(Faction faction, IntVec3 baseCenter)
        {
            this.faction = faction;
            this.baseCenter = baseCenter;
        }

        public override StateGraph CreateGraph()
        {
            var graph = new StateGraph();

            // defend the damn base
            var defend = new LordToil_RIMDAYDefendTheBank(baseCenter);
            graph.AddToil(defend);

            // attack the damn heisters
            var attack = new LordToil_RIMDAYAssault();
            graph.AddToil(attack);

            graph.StartingToil = defend;

            // something suspicious happened
            var toAttack = new Transition(defend, attack);
            toAttack.AddTrigger(new Trigger_OnClamor(RIMDAY_ClamorDefOf.RIMDAY_Gunshot));
            toAttack.AddTrigger(new Trigger_Memo("RIMDAY_BodySpotted"));
            toAttack.AddPostAction(new TransitionAction_Message("The jig is up!"));
            toAttack.AddPostAction(new TransitionAction_Custom(() =>
            {
                // lose rep with faction so they are hostile
                if (defend?.lord?.faction != null && !defend.lord.faction.HostileTo(Faction.OfPlayer))
                {
                    var playerFaction = Faction.OfPlayer;
                    var enemyFaction = defend.lord.faction;

                    enemyFaction.TryAffectGoodwillWith(
                        playerFaction,
                        -200,
                        canSendMessage: true,
                        canSendHostilityLetter: true,
                        reason: DefDatabase<HistoryEventDef>.GetNamed("RIMDAY_BankHeistEscalation")
                    );

                    Messages.Message($"{defend.lord.faction.Name} has become hostile!", MessageTypeDefOf.ThreatBig, historical: true);
                }
            }));
            graph.AddTransition(toAttack);

            return graph;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref faction, "faction");
            Scribe_Values.Look(ref baseCenter, "baseCenter");
        }
    }

    public class LordToil_RIMDAYDefendTheBank : LordToil
    {
        public IntVec3 baseCenter;

        public override IntVec3 FlagLoc => baseCenter;

        public LordToil_RIMDAYDefendTheBank(IntVec3 baseCenter)
        {
            this.baseCenter = baseCenter;
        }

        public override void UpdateAllDuties()
        {
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                lord.ownedPawns[i].mindState.duty = new PawnDuty(DutyDefOf.Defend, baseCenter);
            }
        }
    }

    public class LordToil_RIMDAYRadioForHelp : LordToil
    {
        public override void UpdateAllDuties()
        {
            foreach (var pawn in lord.ownedPawns)
            {
                //pawn.mindState.duty = new PawnDuty(RIMDAY_DutyDefOf.RIMDAY_PageCommand);
            }
        }
    }

    public class LordToil_RIMDAYAssault : LordToil
    {
        public override void UpdateAllDuties()
        {
            foreach (Pawn pawn in lord.ownedPawns)
            {
                pawn.mindState.duty = new PawnDuty(DutyDefOf.AssaultColony);
            }
        }
    }
}
