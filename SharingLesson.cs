using System;
using UnityEngine;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_SharingLesson : IPart
  {
    public int PercentPerRitual = 1;
    public int WaterRitualsPerformed = 0;
    public int Bonus = 0;
    public bool BonusApplied;

    public CRYPTOGEOLOGY_SharingLesson()
    {
      WaterRitualsPerformed = 0;
      Bonus = 0;
      BonusApplied = false;
    }

    public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == GetWaterRitualReputationAmountEvent.ID || ID == WaterRitualStartEvent.ID  || ID == EquippedEvent.ID  || ID == UnequippedEvent.ID || ID == GetShortDescriptionEvent.ID;

    public override bool HandleEvent(WaterRitualStartEvent E)
    {
      if (E.Actor == this.ParentObject.Equipped && !E.Record.Has("CRYPTOGEOLOGY_usedSharingLesson") && E.Actor.IsPlayer())
      {
        E.Record.attributes.Add("CRYPTOGEOLOGY_usedSharingLesson");
				// Increment item's bonus
        this.WaterRitualsPerformed += 1;
        this.UpdateStatShifts(E.Actor);
      }
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(GetWaterRitualReputationAmountEvent E)
    {
      if (E.Actor == this.ParentObject.Equipped && !E.Record.Has("CRYPTOGEOLOGY_usedSharingLesson") && E.Actor.IsPlayer())
      {
        E.Record.attributes.Add("CRYPTOGEOLOGY_usedSharingLesson");
        // Increment item's bonus
        this.WaterRitualsPerformed += 1;
        this.UpdateStatShifts(E.Actor);
      }
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(EquippedEvent E)
    {
      GameObject actor = E.Actor;
      this.UpdateStatShifts(actor);
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(UnequippedEvent E)
    {
      GameObject actor = E.Actor;
      if (this.BonusApplied)
      {
        this.BonusApplied = false;
        this.StatShifter.RemoveStatShifts(actor);
      }
      return base.HandleEvent(E);
    }

    public bool UpdateStatShifts(GameObject who = null)
    {
      if (who == null)
      {
        who = this.ParentObject.Equipped;
        if (who == null)
          return false;
      }
      if (!this.ParentObject.IsWorn())
      {
        this.StatShifter.RemoveStatShifts();
        this.BonusApplied = false;
        return false;
      }
      this.Bonus = (int) Math.Ceiling((double) who.Statistics["Hitpoints"].BaseValue * ((double) this.WaterRitualsPerformed * this.PercentPerRitual * 0.0099999997764825821));
      this.StatShifter.SetStatShift(who, "Hitpoints", this.Bonus, true);
      this.BonusApplied = true;
      return true;
    }

    public override bool HandleEvent(GetShortDescriptionEvent E)
    {
      E.Postfix.AppendRules("+" + (this.WaterRitualsPerformed * this.PercentPerRitual).ToString() + " % max HP");
      return base.HandleEvent(E);
    }
  }
}
