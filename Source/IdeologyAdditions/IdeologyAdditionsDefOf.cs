using RimWorld;
using Verse;

namespace IdeologyAdditions
{
    [DefOf]
    public class IdeologyAdditionsDefOf
    {
        public static WorkGiverDef IdeologyAdditions_Pray;
        public static WorkTypeDef IdeologyAdditions_Praying;
        public static JobDef IdeologyAdditions_StartPrayer;
        public static JobDef IdeologyAdditions_WitnessPrayer;
        public static ThoughtDef IdeologyAdditions_PrayerCasterNoOne;
        public static ThoughtDef IdeologyAdditions_PrayerCasterSmallGroup;
        public static ThoughtDef IdeologyAdditions_PrayerCasterMediumGroup;
        public static ThoughtDef IdeologyAdditions_PrayerCasterLargeGroup;
        public static ThoughtDef IdeologyAdditions_ActivePrayer;
        public static ThoughtDef IdeologyAdditions_WitnessedPrayer;

        static IdeologyAdditionsDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(IdeologyAdditionsDefOf));
        }
    }
}