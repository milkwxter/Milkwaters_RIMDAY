using Verse;

namespace RIMDAY
{
    public class CompProperties_BankGuard : CompProperties
    {
        public CompProperties_BankGuard()
        {
            this.compClass = typeof(CompBankGuard);
        }
    }

    public class CompBankGuard : ThingComp
    {
        public bool IsBankGuard = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref IsBankGuard, "IsBankGuard", false);
        }
    }
}
