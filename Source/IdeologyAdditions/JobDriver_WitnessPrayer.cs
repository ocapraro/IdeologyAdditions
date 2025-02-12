using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace IdeologyAdditions
{
    public class JobDriver_WitnessPrayer : JobDriver
    {
        private const TargetIndex RitualSeat = TargetIndex.B;
        private const TargetIndex Caster = TargetIndex.A;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(RitualSeat), job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(RitualSeat);
            
            Toil goToRitualSeat = Toils_Goto.GotoThing(RitualSeat, PathEndMode.OnCell);
            Toil witnessPrayer = Toils_General.Wait(job.def.joyDuration)
                .WithEffect(EffecterDefOf.Birthday, RitualSeat)
                .FailOn(() => !IdeologyAdditions.PawnPraying(job.GetTarget(Caster).Pawn));
            
            
            yield return goToRitualSeat;
            yield return witnessPrayer;
        }
    }
}