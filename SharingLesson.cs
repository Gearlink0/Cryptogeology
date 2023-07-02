using System;
using UnityEngine;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_SharingLesson : IPart
  {
    public int HPPerRitual = 1;
    public int Bonus = 0;

    public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade)
      || ID == GetWaterRitualReputationAmountEvent.ID
      || ID == WaterRitualStartEvent.ID
      || ID == EquippedEvent.ID
      || ID == UnequippedEvent.ID
      || ID == GetShortDescriptionEvent.ID;
    }

    public override bool HandleEvent(WaterRitualStartEvent E)
    {
      if (E.Actor.IsPlayer())
      {
        int OldBonus = this.Bonus;
        this.UpdateStatShifts(E.Actor);
        if( this.Bonus > OldBonus )
          XRL.Messages.MessageQueue.AddPlayerMessage("The lesson resonates as you share a new circle");
      }
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(GetWaterRitualReputationAmountEvent E)
    {
      if (E.Actor.IsPlayer())
      {
        int OldBonus = this.Bonus;
        this.UpdateStatShifts(E.Actor);
        if( this.Bonus > OldBonus )
          XRL.Messages.MessageQueue.AddPlayerMessage("The lesson resonates as you share a new circle");
      }
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(EquippedEvent E)
    {
      this.UpdateStatShifts(E.Actor);
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(UnequippedEvent E)
    {
      this.StatShifter.RemoveStatShifts(E.Actor);
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
        return false;
      }
      this.Bonus = this.CalculateBonus();
      this.StatShifter.SetStatShift(who, "Hitpoints", this.Bonus, true);
      return true;
    }

    public override bool HandleEvent(GetShortDescriptionEvent E)
    {
      this.Bonus = this.CalculateBonus();
      E.Postfix.AppendRules("+" + (this.Bonus).ToString() + " max HP");
      return base.HandleEvent(E);
    }

    public int CalculateBonus()
    {
      if( The.Player.HasPart("CRYPTOGEOLOGY_CountWaterRituals") )
      {
        CRYPTOGEOLOGY_CountWaterRituals Counter = (CRYPTOGEOLOGY_CountWaterRituals) The.Player.GetPart("CRYPTOGEOLOGY_CountWaterRituals");
        return Counter.WaterRitualsPerformed * this.HPPerRitual;
      }
      return 0;
    }
  }
}
