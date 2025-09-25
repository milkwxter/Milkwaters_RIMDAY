using RimWorld;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RIMDAY
{
    public class Building_VaultDoor : Building_Door
    {
        protected bool IsLocked = true;
        protected bool IsBeingDrilled = false;
        protected float ticksToFinish = 1000;
        protected float currentTicks = 0;

        public void StartDrilling()
        {
            if (IsLocked)
            {
                IsBeingDrilled = true;
            }
        }

        private void UnlockVaultDoor()
        {
            IsLocked = false;
            this.Map.pathing.RecalculatePerceivedPathCostAt(this.Position);
        }

        public override bool PawnCanOpen(Pawn p)
        {
            if (IsLocked)
                return false;

            return base.PawnCanOpen(p);
        }

        public override bool BlocksPawn(Pawn p)
        {
            if (IsLocked)
                return true;

            return base.BlocksPawn(p);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            if (IsLocked && !IsBeingDrilled)
            {
                if (PawnHasDrill(myPawn))
                {
                    yield return new FloatMenuOption("Drill vault door", () =>
                    {
                        Job job = JobMaker.MakeJob(RIMDAY_JobDefOf.DrillVaultDoor, this);
                        myPawn.jobs.TryTakeOrderedJob(job);
                    });
                }
                else
                {
                    yield return new FloatMenuOption("Requires portable drill to breach", null);
                }
            }
            else
            {
                foreach (var opt in base.GetFloatMenuOptions(myPawn))
                    yield return opt;
            }
        }

        private bool PawnHasDrill(Pawn pawn)
        {
            if (pawn.inventory != null)
            {
                foreach (Thing thing in pawn.inventory.innerContainer)
                {
                    if (thing.def.defName == "m_Drill")
                        return true;
                }
            }

            return false;
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            // do the basic draw stuff
            base.DrawAt(drawLoc, flip);

            // draw progress bar
            if (IsBeingDrilled)
            {
                Vector3 barLoc = drawLoc;
                barLoc.z -= 1f;
                GenDraw.FillableBarRequest r = new GenDraw.FillableBarRequest
                {
                    center = barLoc,
                    size = new Vector2(1f, 0.2f),
                    fillPercent = currentTicks / ticksToFinish,
                    filledMat = SolidColorMaterials.SimpleSolidColorMaterial(new UnityEngine.Color(0.9f, 0.85f, 0.2f)),
                    unfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new UnityEngine.Color(0.3f, 0.3f, 0.3f)),
                    margin = 0.1f
                };
                GenDraw.DrawFillableBar(r);
            }
        }

        protected override void Tick()
        {
            // do the basic ticking stuff
            base.Tick();

            // custom ticking stuff
            if (IsBeingDrilled)
            {
                // progress the drill
                currentTicks += 1;

                // effects
                if (currentTicks % 30 == 0)
                {
                    FleckMaker.ThrowMicroSparks(
                        this.DrawPos,
                        this.Map
                    );
                }

                // open door and reset vars
                if (currentTicks > ticksToFinish)
                {
                    IsBeingDrilled = false;
                    currentTicks = 0;
                    UnlockVaultDoor();
                }
            }
        }
    }
}
