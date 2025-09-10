using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace RIMDAY
{
    class QuestNode_Root_MBank : QuestNode
    {
        private bool TryFindSiteTile(out PlanetTile tile, bool exitOnFirstTileFound = false)
        {
            return TileFinder.TryFindNewSiteTile(
                out tile,
                minDist: 5,
                maxDist: 8,
                allowedLandmarks: null,
                selectLandmarkChance: 0f,
                canSelectComboLandmarks: false,
                tileFinderMode: TileFinderMode.Near,
                exitOnFirstTileFound: true,
                canBeSpace: false,
                validator: x => Find.WorldGrid[x].hilliness == Hilliness.Flat
            );
        }

        private Site GenerateSite(Quest quest, Slate slate)
        {
            // what is the enemy faction
            Faction bankFaction = Find.FactionManager.AllFactions.FirstOrDefault(f => f.def == FactionDef.Named("OutlanderCivil"));

            // find my sitepart
            SitePartDef sitePartDef = DefDatabase<SitePartDef>.GetNamed("m_Bank");

            // find a suitable tile
            TryFindSiteTile(out PlanetTile tile);

            // create the parameters
            SitePartParams sitePartParams = new SitePartParams
            {
                threatPoints = 1000f
            };

            // create the site
            Site site = QuestGen_Sites.GenerateSite(new List<SitePartDefWithParams>
            {
                new SitePartDefWithParams(sitePartDef, sitePartParams)
            }, tile, bankFaction, false, null);
            site.doorsAlwaysOpenForPlayerPawns = true;

            // spawn the world object we created
            quest.SpawnWorldObject(site);

            // return our beautiful site so we can access it again
            return site;
        }

        protected override void RunInt()
        {
            // create the most important variables
            Slate slate = QuestGen.slate;
            Quest quest = QuestGen.quest;

            // create the site
            Site site = GenerateSite(quest, slate);

            // update slate
            slate.Set("playerFaction", Faction.OfPlayer);
            slate.Set("map", QuestGen_Get.GetMap(false, null));
            slate.Set("bank", site);

            // custom part that watches the map for success and fail conditions
            var part = new QuestPart_MBankUtilities
            {
                mapParent = site,
                inSignalEnable = QuestGenUtility.HardcodedSignalWithQuestID("bank.MapGenerated"),
                quest = quest
            };
            quest.AddPart(part);
        }

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
    }
}
