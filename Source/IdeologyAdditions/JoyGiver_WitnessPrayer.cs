using RimWorld;
using Verse;
using Verse.AI;

namespace IdeologyAdditions
{
    public class JoyGiver_WitnessPrayer : JoyGiver
    {
        private Building seat;
        private Pawn caster;

        private static Building GetSeatFromPawnRoom(Pawn roomPawn, Pawn pawn)
        {
            foreach (Building building in roomPawn.GetRoom().ContainedThings<Building>())
            {
                if(
                    !building.def.Equals(roomPawn.Ideo.RitualSeatDef) || 
                    !pawn.CanReserve(building)
                ) continue;
                return building;
            }
            return null;
        }
        private void ActivePrayer(Pawn pawn)
        {
            Building bestSeat = null;
            Pawn bestCaster = null;
            foreach (Pawn colonist in pawn.Map.mapPawns.FreeColonists)
            {
                if (!IdeologyAdditions.PawnPraying(colonist) || colonist.Equals(pawn)) continue;
                Building seatOption = GetSeatFromPawnRoom(colonist, pawn);
                if (
                    seatOption == null ||
                    (bestSeat != null &&
                     pawn.Position.DistanceTo(bestSeat.Position) <= pawn.Position.DistanceTo(seatOption.Position))
                ) continue;
                bestSeat = seatOption;
                bestCaster = colonist;
            }
            seat = bestSeat;
            caster = bestCaster;
            
        }
        public override Job TryGiveJob(Pawn pawn)
        {
            ActivePrayer(pawn);
            if (seat == null) return null;
            return JobMaker.MakeJob(
                IdeologyAdditionsDefOf.IdeologyAdditions_WitnessPrayer, 
                caster,
                seat
            );
        }
    }
}