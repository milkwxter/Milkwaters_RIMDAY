using KCSG;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;
using static Verse.AI.ThingCountTracker;

namespace RIMDAY
{
    public class GenStep_SpawnMBankLayout : GenStep
    {
        public override int SeedPart => 901238893;

        // create list of pawns to defend that room, immersive
        List<Pawn> pawnsDefendingBank = new List<Pawn>();

        public override void Generate(Map map, GenStepParams parms)
        {
            // choose which layout to spawn
            var allLayouts = DefDatabase<KCSG.StructureLayoutDef>.AllDefsListForReading;
            var bankLayouts = allLayouts.Where(def => def.tags != null && def.tags.Contains("m_Bank")).ToList();
            KCSG.StructureLayoutDef layoutDef = bankLayouts.RandomElement();

            // spawn the layout
            Faction parentFaction = map.ParentFaction;
            IntVec3 center = map.Center;
            IntVec2 sizes = layoutDef.Sizes;
            CellRect cellRect = CellRect.CenteredOn(center, sizes);
            GenOption.GetAllMineableIn(cellRect, map);
            layoutDef.Generate(cellRect, map, parentFaction);

            // save the rooms so we can play with them
            List<Room> uniqueRooms = new List<Room>();
            foreach (IntVec3 cell in cellRect)
            {
                Room room = cell.GetRoom(map);
                if (room != null && !uniqueRooms.Contains(room))
                {
                    uniqueRooms.Add(room);
                }
            }

            // spawn stuff in the room
            foreach (Room room in uniqueRooms)
            {
                // error handling
                if (room == null || room.TouchesMapEdge) continue;

                // spawn simple guards
                SpawnGuards(room);
            }

            // lord job so pawns guard the room they spawned in
            if (pawnsDefendingBank.Count > 0)
            {
                Lord guardYourRoomLord = LordMaker.MakeNewLord(
                    map.ParentFaction,
                    new LordJob_RIMDAYDefendTheBank(map.ParentFaction, center),
                    map,
                    pawnsDefendingBank
                    );
            }
        }

        private void SpawnGuards(Room room)
        {
            // set vars
            int roomSize = room.CellCount;

            // spawn pawns according to size of room
            int pawnCount = Math.Min(roomSize / 30, 3);
            for (int i = 0; i < pawnCount; i++)
            {
                // request a new pawn from the game with params
                PawnGenerationRequest req = new PawnGenerationRequest(
                    kind: PawnKindDef.Named("Town_Guard"),
                    faction: room.Map.ParentFaction,
                    context: PawnGenerationContext.NonPlayer,
                    tile: room.Map.Tile,
                    forceGenerateNewPawn: true,
                    allowDead: false
                );

                // generate him
                Pawn pawn = PawnGenerator.GeneratePawn(req);

                // spawn him and add to list for lord use
                IntVec3 spawnCell = room.Cells.Where(c => c.Standable(room.Map)).InRandomOrder().FirstOrDefault();
                if (spawnCell != IntVec3.Invalid)
                {
                    GenSpawn.Spawn(pawn, spawnCell, room.Map);
                    pawnsDefendingBank.Add(pawn);
                }

                // give him the bank guard comp
                var comp = new CompBankGuard();
                comp.parent = pawn;
                comp.IsBankGuard = true;
                pawn.AllComps.Add(comp);
            }
        }
    }
}
