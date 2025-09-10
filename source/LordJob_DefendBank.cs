using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace RIMDAY
{
    public class LordJob_DefendBank : LordJob
    {
	    private Faction faction;

        private IntVec3 baseCenter;

        private bool attackWhenPlayerBecameEnemy;

        private int delayBeforeAssault;

        public LordJob_DefendBank()
        {
        }

        public LordJob_DefendBank(Faction faction, IntVec3 baseCenter, int delayBeforeAssault, bool attackWhenPlayerBecameEnemy = false)
        {
            this.faction = faction;
            this.baseCenter = baseCenter;
            this.attackWhenPlayerBecameEnemy = attackWhenPlayerBecameEnemy;
            this.delayBeforeAssault = delayBeforeAssault;
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();

            // defend center of base
            LordToil_DefendBase lordToil_DefendBase = (LordToil_DefendBase)(stateGraph.StartingToil = new LordToil_DefendBase(baseCenter));

            // continuously defend base
            LordToil_DefendBase lordToil_DefendBase2 = new LordToil_DefendBase(baseCenter);
            stateGraph.AddToil(lordToil_DefendBase2);

            // attacking players guys state
            LordToil_AssaultColony lordToil_AssaultColony = new LordToil_AssaultColony(attackDownedIfStarving: true)
            {
                useAvoidGrid = true
            };

            // if we become friendly with player, stop attacking him
            stateGraph.AddToil(lordToil_AssaultColony);
            Transition transition = new Transition(lordToil_DefendBase, lordToil_DefendBase2);
            transition.AddSource(lordToil_AssaultColony);
            transition.AddTrigger(new Trigger_BecameNonHostileToPlayer());
            stateGraph.AddTransition(transition);

            // if we become hostile to player, start attacking him
            Transition transition2 = new Transition(lordToil_DefendBase2, attackWhenPlayerBecameEnemy ? ((LordToil)lordToil_AssaultColony) : ((LordToil)lordToil_DefendBase));
            if (attackWhenPlayerBecameEnemy)
            {
                transition2.AddSource(lordToil_DefendBase);
            }
            transition2.AddTrigger(new Trigger_BecamePlayerEnemy());
            stateGraph.AddTransition(transition2);

            // based on a ton of misc triggers, become hostile to the player
            Transition transition3 = new Transition(lordToil_DefendBase, lordToil_AssaultColony);
            // too many guards die
            transition3.AddTrigger(new Trigger_FractionPawnsLost(0.2f));
            // guards get suspicious over time
            transition3.AddTrigger(new Trigger_TicksPassed(delayBeforeAssault));
            // too much time passes, guards are out of food
            transition3.AddTrigger(new Trigger_UrgentlyHungry());
            // a ability is casted
            transition3.AddTrigger(new Trigger_OnClamor(ClamorDefOf.Ability));
            // a gunshot is heard
            transition3.AddTrigger(new Trigger_LoudGunshotClamor());

            // wake up sleepers when misc trigger goes off
            transition3.AddPostAction(new TransitionAction_WakeAll());

            // show message to player when the transition starts
            TaggedString taggedString = faction.def.messageDefendersAttacking.Formatted(faction.def.pawnsPlural, faction.Name, Faction.OfPlayer.def.pawnsPlural).CapitalizeFirst();
            transition3.AddPreAction(new TransitionAction_Message(taggedString, MessageTypeDefOf.ThreatBig));

            // add the transition
            stateGraph.AddTransition(transition3);

            // epic return statement
            return stateGraph;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref faction, "faction");
            Scribe_Values.Look(ref baseCenter, "baseCenter");
            Scribe_Values.Look(ref attackWhenPlayerBecameEnemy, "attackWhenPlayerBecameEnemy", defaultValue: false);
            Scribe_Values.Look(ref delayBeforeAssault, "delayBeforeAssault", 25000);
        }
    }
}
