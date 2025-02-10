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
        public static ThoughtDef IdeologyAdditions_ActivePrayerCaster;
        public static ThoughtDef IdeologyAdditions_ActivePrayer;
        public static ThoughtDef IdeologyAdditions_WitnessedPrayer;

        static IdeologyAdditionsDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(IdeologyAdditionsDefOf));
        }
    }
}