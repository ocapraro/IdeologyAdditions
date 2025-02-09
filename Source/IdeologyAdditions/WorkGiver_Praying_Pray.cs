using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace IdeologyAdditions
{
    public class WorkGiver_Praying_Pray : WorkGiver_Scanner
    {
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !IdeologyAdditions.ValidPrayerCaster(pawn, forced);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!pawn.CanReserve(t, 1, -1, null, forced)) return null;
            
            return new Job(IdeologyAdditionsDefOf.IdeologyAdditions_StartPrayer, t);
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            foreach (Thing l in pawn.Map.listerBuildings.allBuildingsColonist)
            {
                // CompProperties_AssignableToPawn a1 = new CompProperties_AssignableToPawn();
                // a1.compClass = typeof(CompAssignableToPawn_Lectern);
                
                if (!IdeologyAdditions.ValidLectern(l,pawn)) continue;
                yield return l;
            }
        }
    }
}