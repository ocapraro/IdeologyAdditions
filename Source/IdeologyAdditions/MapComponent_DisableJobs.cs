using System.Collections.Generic;
using Verse;

namespace IdeologyAdditions
{
    public class MapComponent_DisableJobs : MapComponent
    {
        public MapComponent_DisableJobs(Map map) : base(map)
        {
            
        }

        private void disablePrayerForNonCasters()
        {
            foreach (Pawn colonist in map.mapPawns.FreeColonists)
            {
                List<WorkTypeDef> disabledWorkTypes = colonist.GetDisabledWorkTypes();
                if(IdeologyAdditions.ValidPrayerCaster(colonist))
                {
                    if (disabledWorkTypes.Contains(IdeologyAdditionsDefOf.IdeologyAdditions_Praying)) 
                        disabledWorkTypes.Remove(IdeologyAdditionsDefOf.IdeologyAdditions_Praying);
                    continue;
                };
                if(!disabledWorkTypes.Contains(IdeologyAdditionsDefOf.IdeologyAdditions_Praying))
                    disabledWorkTypes.Add(IdeologyAdditionsDefOf.IdeologyAdditions_Praying);
            }
        }

        public override void MapComponentTick()
        {
            if (!map.IsHashIntervalTick(100)) return;
            disablePrayerForNonCasters();
        }

        public override void FinalizeInit()
        {
            disablePrayerForNonCasters();
        }
    }
}