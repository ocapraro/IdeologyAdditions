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

        private void AssignMoodBuffs()
        {
            int prayerWitnesses = 0;
            foreach (Pawn colonist in pawn.Map.mapPawns.FreeColonists)
            {
                if ((colonist == null || colonist.Dead) || 
                    (colonist.Equals(pawn))
                ) continue;
                
                // mood boost for all colonists of the same religion
                if (colonist.Ideo.Equals(pawn.Ideo))
                    colonist.needs.mood.thoughts.memories.TryGainMemory(IdeologyAdditionsDefOf.IdeologyAdditions_ActivePrayer);
                
                // mood boost for nearby pawns
                if (pawn.GetRoom().Equals(colonist.GetRoom()))
                {
                    colonist.needs.mood.thoughts.memories.TryGainMemory(IdeologyAdditionsDefOf.IdeologyAdditions_WitnessedPrayer);
                    prayerWitnesses++;
                }
            }

            AssignMoodBuffCaster(prayerWitnesses, pawn);
        }

        private static void AssignMoodBuffCaster(int witnessCount, Pawn caster)
        {
            List<ThoughtDef> moodBuffs = new List<ThoughtDef>
            {
                IdeologyAdditionsDefOf.IdeologyAdditions_PrayerCasterNoOne,
                IdeologyAdditionsDefOf.IdeologyAdditions_PrayerCasterSmallGroup,
                IdeologyAdditionsDefOf.IdeologyAdditions_PrayerCasterMediumGroup,
                IdeologyAdditionsDefOf.IdeologyAdditions_PrayerCasterLargeGroup
            };
            int memoryIndex;
            if (witnessCount < 1)
            {
                memoryIndex = 0;
            } 
            else  if (witnessCount < 4)
            {
                memoryIndex = 1;
            } else if (witnessCount < 11)
            {
                memoryIndex = 2;
            }
            else
            {
                memoryIndex = 3;
            }
            int curMemoryIndex = -1;
            List<Thought_Memory> memories = caster.needs.mood.thoughts.memories.Memories;
            foreach (var memory in memories)
            {
                if (!moodBuffs.Contains(memory.def)) continue;
                curMemoryIndex = moodBuffs.IndexOf(memory.def);
                break;
            }
            if(curMemoryIndex > memoryIndex) return;
            for (int i = memoryIndex-1; i >= 0; i--)
            {
                caster.needs.mood.thoughts.memories.RemoveMemoriesOfDef(moodBuffs[i]);
            }
            caster.needs.mood.thoughts.memories.TryGainMemory(moodBuffs[memoryIndex]);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(Lectern), job);
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
            
            Toil buffMood = Toils_General.Do(AssignMoodBuffs);
            
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