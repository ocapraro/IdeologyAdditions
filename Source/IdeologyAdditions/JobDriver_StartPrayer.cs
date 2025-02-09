using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace IdeologyAdditions
{
    class JobDriver_StartPrayer : JobDriver
    {
        private const TargetIndex Lectern = TargetIndex.A;
        private const int PrayerDuration = 5000;

        private void AssignMoodBuff()
        {
            foreach (Pawn colonist in pawn.Map.mapPawns.FreeColonists)
            {
                if ((colonist == null || colonist.Dead) || 
                    (colonist.Equals(pawn) || !colonist.Ideo.Equals(pawn.Ideo))
                ) continue;
                colonist.needs.mood.thoughts.memories.TryGainMemory(IdeologyAdditionsDefOf.IdeologyAdditions_ActivePrayer);
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(Lectern), job, 1, -1, null);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // fail the toil if the lectern is destroyed
            this.FailOnDestroyedNullOrForbidden(Lectern);
            
            
            // go to lectern
            Toil goToLectern = Toils_Goto.GotoThing(Lectern, PathEndMode.InteractionCell);


            // assign voice by gender
            SoundDef voice = pawn.gender == Gender.Female
                ? SoundDefOf.Speech_Leader_Female
                : SoundDefOf.Speech_Leader_Male;
            
            Toil buffMood = Toils_General.Do(AssignMoodBuff);
            
            // wait until the prayer is over
            Toil pray = Toils_General
                .WaitWith(Lectern,  PrayerDuration, true, true)
                .FailOn(() =>
                {
                    Thing lectern = job.GetTarget(Lectern).Thing;
                    return !IdeologyAdditions.ValidLectern(lectern, pawn);
                })
                .WithEffect(EffecterDefOf.ActivitySuppression, Lectern)
                .PlaySustainerOrSound(voice);
            pray.tickAction = () =>
            {
                if(pawn.IsHashIntervalTick(100)) FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, FleckDefOf.Meditating);
                
            };

            // gain xp
            pawn.skills.Learn(SkillDefOf.Social, 1);
            
            yield return goToLectern;
            yield return pray;
            yield return buffMood;
            
            
        }
    }
}