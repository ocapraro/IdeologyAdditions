using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace IdeologyAdditions
{
    public static class IdeologyAdditions
    {
        /// <summary>
        /// Checks to see if the pawn can start a prayer.
        /// </summary>
        public static bool ValidPrayerCaster(Pawn pawn, bool forced = false)
        {
            return pawn.Ideo.GetRole(pawn) != null && pawn.Ideo.GetRole(pawn).def.Equals(PreceptDefOf.IdeoRole_Moralist);
        }
        
        /// <summary>
        /// Checks to see if the pawn is praying.
        /// </summary>
        public static bool PawnPraying(Pawn pawn)
        {
            return !(
                pawn == null ||
                pawn.Dead ||
                pawn.Map == null ||
                pawn.CurJob == null ||
                !pawn.CurJob.def.Equals(IdeologyAdditionsDefOf.IdeologyAdditions_StartPrayer)
            );
        }

        /// <summary>
        /// Checks to see if the pawn can start a prayer at the lectern.
        /// </summary>
        public static bool ValidLectern(Thing lectern, Pawn pawn)
        {
            // if the lectern isn't actually a lectern.
            if (!lectern.def.Equals(ThingDefOf.Lectern)) return false;
            CompAssignableToPawn_Lectern comp = lectern.TryGetComp<CompAssignableToPawn_Lectern>();
            // if the lectern has an assignment not including the attempting caster.
            if(
                comp != null && 
                comp.AssignedPawnsForReading.Count != 0 && 
                !comp.AssignedPawns.Contains(pawn)
            ) return false;
            return true;
        }
    }
}