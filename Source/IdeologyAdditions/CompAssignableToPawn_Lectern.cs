using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace IdeologyAdditions
{
    public class CompAssignableToPawn_Lectern : CompAssignableToPawn
    {
        public override IEnumerable<Pawn> AssigningCandidates
        {
            get
            {
                if (!parent.Spawned)
                {
                    return Enumerable.Empty<Pawn>();
                }
                return from p in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists
                    where (p.Ideo.GetRole(p) != null) && (p.Ideo.GetRole(p).def.Equals(PreceptDefOf.IdeoRole_Moralist))
                    orderby CanAssignTo(p).Accepted descending
                    select p;
            }
        }
        
        protected override bool CanSetUninstallAssignedPawn(Pawn pawn)
        {
            if (pawn != null && !AssignedAnything(pawn) && CanAssignTo(pawn))
            {
                return pawn.IsColonist;
            }
            return false;
        }
        
        public override string CompInspectStringExtra()
        {
            switch (AssignedPawnsForReading.Count)
            {
                case 0:
                    return "Owner".Translate() + ": " + "Nobody".Translate();
                case 1:
                    return "Owner".Translate() + ": " + AssignedPawnsForReading[0].Label;
                default:
                    return "";
            }
        }
        
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!base.ShouldShowAssignmentGizmo()) yield break;
            Command_Action command_Action = new Command_Action
            {
                defaultLabel = "Assign Casters",
                icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner"),
                defaultDesc = GetAssignmentGizmoDesc(),
                action = () => Find.WindowStack.Add(new Dialog_AssignLectern(this)),
                hotKey = KeyBindingDefOf.Misc4
            };
            if (!Props.noAssignablePawnsDesc.NullOrEmpty() && !base.AssigningCandidates.Any())
                (command_Action).Disable(Props.noAssignablePawnsDesc);
            yield return command_Action;
        }
        
        
        
    }
}