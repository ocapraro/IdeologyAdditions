using RimWorld;
using Verse;

namespace IdeologyAdditions
{
    public class ThoughtWorker_ActivePrayerCaster : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            return p.jobs.curJob.def.Equals(IdeologyAdditionsDefOf.IdeologyAdditions_StartPrayer);
        }
    }
}