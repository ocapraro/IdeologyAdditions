using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace IdeologyAdditions
{
    public class Dialog_AssignLectern : Window
    { 
      private readonly CompAssignableToPawn_Lectern assignable;
      private Vector2 scrollPosition;
      private static readonly List<Pawn> tmpPawnSorted = new List<Pawn>(16);

      public override Vector2 InitialSize => new Vector2(650f, 500f);

      public Dialog_AssignLectern(CompAssignableToPawn_Lectern assignable)
      {
        this.assignable = assignable;
        this.doCloseButton = true;
        this.doCloseX = true;
        this.closeOnClickedOutside = true;
        this.absorbInputAroundWindow = true;
      }

      public override void DoWindowContents(Rect inRect)
      {
        Text.Font = (GameFont) 1;
        Rect rect1 = new Rect(inRect);
        ref Rect local1 = ref rect1;
        local1.yMin += 20f;
        ref Rect local2 = ref rect1;
        local2.yMax -= 40f;
        float height = (assignable.AssignedPawnsForReading.Count + assignable.AssigningCandidates.Count()) * 35f + 7f;
        Rect viewRect = new Rect(0, 0, rect1.width, height);
        
        Widgets.AdjustRectsForScrollView(inRect, ref rect1, ref viewRect);
        Widgets.BeginScrollView(rect1, ref this.scrollPosition, viewRect);
        SortTmpList(assignable.AssignedPawnsForReading);
        float y = 0.0f;
        for (int index = 0; index < tmpPawnSorted.Count; ++index)
          DrawAssignedRow(tmpPawnSorted[index], ref y, viewRect, index);
        if (assignable.AssignedPawnsForReading.Count > 0)
        {
          Rect rect2 = new Rect(0.0f, y, viewRect.width, 7f);
          y += 7f;
          TextBlock textBlock = new TextBlock(Widgets.SeparatorLineColor);
          try
          {
            Widgets.DrawLineHorizontal(rect2.x, rect2.y + rect2.height / 2f, rect2.width);
          }
          finally
          {
            textBlock.Dispose();
          }
        }
        SortTmpList(assignable.AssigningCandidates);
        for (int index = 0; index < Dialog_AssignLectern.tmpPawnSorted.Count; ++index)
          this.DrawUnassignedRow(Dialog_AssignLectern.tmpPawnSorted[index], ref y, viewRect, index);
        Dialog_AssignLectern.tmpPawnSorted.Clear();
        Widgets.EndScrollView();
      }

      private static void SortTmpList(IEnumerable<Pawn> collection)
      {
        tmpPawnSorted.Clear();
        tmpPawnSorted.AddRange(collection);
        tmpPawnSorted.SortBy(x=>x.LabelShort);
      }

      private void DrawAssignedRow(Pawn pawn, ref float y, Rect viewRect, int i)
      {
        Rect rect1 = new Rect(0.0f, y, viewRect.width, 35f);
        y += 35f;
        if (i % 2 == 1)
          Widgets.DrawLightHighlight(rect1);
        Rect rect2 = rect1;
        rect2.width = rect1.height;
        Widgets.ThingIcon(rect2, pawn);
        Rect rect3 = rect1;
        rect3.xMin = (float) (rect1.xMax - 165.0 - 10.0);
        Rect rect4 = rect3.ContractedBy(2f);
        if (Widgets.ButtonText(rect4, new TaggedString("AssignableLectern_LecternUnassign".Translate())))
        {
          assignable.TryUnassignPawn(pawn);
          SoundDefOf.Click.PlayOneShotOnCamera();
        }
        Rect rect5 = rect1;
        rect5.xMin = rect2.xMax + 10f;
        rect5.xMax = rect4.xMin - 10f;
        TextBlock textBlock = new TextBlock((TextAnchor) 3);
        try
        {
          Widgets.LabelEllipses(rect5, pawn.LabelCap);
        }
        finally
        {
          textBlock.Dispose();
        }
        Rect rect6 = rect1;
        rect6.xMin = rect4.xMin - 180f;
        rect6.xMax = rect4.xMin - 10f;
        DrawAssignableWorkRow(pawn, rect6);
        DrawToolTipRow(pawn, rect5);
      }

      private void DrawUnassignedRow(Pawn pawn, ref float y, Rect viewRect, int i)
      {
        if (assignable.AssignedPawnsForReading.Contains(pawn))
          return;
        AcceptanceReport acceptanceReport = assignable.CanAssignTo(pawn);
        bool accepted = acceptanceReport.Accepted;
        Rect rect1 = new Rect(0.0f, y, viewRect.width, 35f);
        y += 35f;
        if (i % 2 == 1)
          Widgets.DrawLightHighlight(rect1);
        if (!accepted)
          GUI.color = Color.gray;
        Rect rect2 = rect1;
        rect2.width = rect1.height;
        Widgets.ThingIcon(rect2, pawn);
        Rect rect3 = rect1;
        rect3.xMin = (float) (rect1.xMax - 165.0 - 10.0);
        rect3 = rect3.ContractedBy(2f);
        if (!Find.IdeoManager.classicMode & accepted && assignable.IdeoligionForbids(pawn))
        {
          Rect rect4 = rect1;
          rect4.width = rect1.height;
          rect4.x = rect1.xMax - rect1.height;
          rect4 = rect4.ContractedBy(2f);
          TextBlock textBlock = new TextBlock((TextAnchor) 3);
          try
          {
            Widgets.Label(rect3, "IdeoligionForbids".Translate());
          }
          finally
          {
            textBlock.Dispose();
          }
          IdeoUIUtility.DoIdeoIcon(rect4, pawn.ideo.Ideo, true, (() =>
          {
            IdeoUIUtility.OpenIdeoInfo(pawn.ideo.Ideo);
            Close();
          }));
        }
        else if (accepted && Widgets.ButtonText(rect3, new TaggedString("AssignableLectern_LecternAssign".Translate())))
        {
          assignable.TryAssignPawn(pawn);
          SoundDefOf.Click.PlayOneShotOnCamera();
        }
        Rect rect5 = rect1;
        rect5.xMin = rect2.xMax + 10f;
        rect5.xMax = rect3.xMin - 10f;
        string str = pawn.LabelCap + (accepted ? "" : " (" + acceptanceReport.Reason.StripTags() + ")");
        TextBlock textBlock1 = new TextBlock((TextAnchor) 3);
        try
        {
          Widgets.LabelEllipses(rect5, str);
        }
        finally
        {
          textBlock1.Dispose();
        }
        Rect rect6 = rect1;
        rect6.xMin = rect3.xMin - 180f;
        rect6.xMax = rect3.xMin - 10f;
        DrawAssignableWorkRow(pawn, rect6);
        DrawToolTipRow(pawn, rect5);
      }

      private void DrawAssignableWorkRow(Pawn pawn, Rect rect)
      {
        TextBlock textBlock = new TextBlock((TextAnchor) 3);
        try
        {
          string str1 = "";
          if (pawn.workSettings.GetPriority(IdeologyAdditionsDefOf.IdeologyAdditions_Praying) > 0)
          {
            string str2 = str1;
            TaggedString taggedString1 = "AssignableLectern_Pray".Translate();
            TaggedString taggedString2 = taggedString1.CapitalizeFirst();
            str1 = new TaggedString(str2 + taggedString2);
          }
          Widgets.LabelEllipses(rect, str1.Colorize(ColorLibrary.Green));
        }
        finally
        {
          textBlock.Dispose();
        }
      }

      private void DrawToolTipRow(Pawn pawn, Rect rect)
      {
        string labelCap = pawn.LabelCap;
        SkillDef social = SkillDefOf.Social;
        string str1 = new TaggedString(labelCap + "\n" + social.LabelCap + ": ");
        string str2;
        if (pawn.skills.GetSkill(social).TotallyDisabled)
        {
          str2 = new TaggedString(str1 + "AssignableLectern_SkillDisabled".Translate());
        }
        else
        {
          str2 = str1 + pawn.skills.GetSkill(social).GetLevel();
          if (pawn.workSettings.GetPriority(IdeologyAdditionsDefOf.IdeologyAdditions_Praying) <= 0)
          {
            string str3 = str2;
            var taggedString1 = "NotAssignedToWorkType".Translate(IdeologyAdditionsDefOf.IdeologyAdditions_Praying.labelShort);
            TaggedString taggedString2 = new TaggedString(" (" + taggedString1.CapitalizeFirst() + ")");
            str2 = new TaggedString(str3 + taggedString2);
          }
        }
        if (!Mouse.IsOver(rect))
          return;
        TooltipHandler.TipRegion(rect, new TipSignal(str2));
      }
    }
}