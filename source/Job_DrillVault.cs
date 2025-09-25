using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RIMDAY
{
    public class JobDriver_UseDrill : JobDriver
    {
        public Building_VaultDoor door => job.GetTarget(TargetIndex.A).Thing as Building_VaultDoor;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (door != null)
            {
                if (!pawn.Reserve(door, job, 1, 0, null, errorOnFailed))
                {
                    return false;
                }
            }
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);

            // make pawn go to door
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            // make our toil
            Toil toil = ToilMaker.MakeToil("MakeNewToils");

            // initial action and setup
            toil.initAction = delegate
            {
                // ensure the pawn is at the interaction cell
                pawn.pather.StartPath(door.InteractionCell, PathEndMode.OnCell);
            };
            toil.defaultCompleteMode = ToilCompleteMode.Never;

            // final action
            toil.AddFinishAction(delegate
            {
                if (pawn.inventory != null)
                {
                    Thing drill = pawn.inventory.innerContainer.FirstOrDefault(
                        t => t.def.defName == "m_Drill"
                    );
                    if (drill != null)
                    {
                        pawn.inventory.innerContainer.Remove(drill);
                        drill.Destroy();
                        door.StartDrilling();
                    }
                }
            });
            yield return toil;
        }
    }
}
