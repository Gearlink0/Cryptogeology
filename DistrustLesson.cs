using System;
using UnityEngine;
// using XRL.World;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_DistrustLesson : IPart
  {
    // When you desecrate a sultan shrine gain +2 DV, +2 MA, and +10 quickness for 2 hours." />
    public int Bonus = 20;
    public int MaxDuration = 2;
    public int Duration = 0;

    public override bool WantEvent(int ID, int cascade)
    {
      if( this.Duration > 0 && ID == EndTurnEvent.ID )
        return true;
      return base.WantEvent(ID, cascade) || ID == EquippedEvent.ID  || ID == UnequippedEvent.ID || ID == GetShortDescriptionEvent.ID;
    }

    public override bool FireEvent(Event E)
    {
      if (E.ID == "Desecrated")
      {
        if (E.GetGameObjectParameter("Object") != null && E.GetGameObjectParameter("Object").HasPart("SultanShrine") )
        {
          this.Duration = this.MaxDuration * Calendar.turnsPerHour;
          GameObject actor = this.ParentObject.Equipped;
          this.UpdateStatShifts(actor);
          if( actor.IsPlayer() ){
            XRL.Messages.MessageQueue.AddPlayerMessage("The lesson whispers warriness, and your senses sharpen");
          }
        }
      }
      return base.FireEvent(E);
    }

    public override bool HandleEvent(EndTurnEvent E)
    {
      if (this.Duration > 0)
      {
        --this.Duration;
        if (this.Duration == 0)
        {
          GameObject actor = this.ParentObject.Equipped;
          this.UpdateStatShifts(actor);
          if( actor.IsPlayer() ){
            XRL.Messages.MessageQueue.AddPlayerMessage("Your heightened warriness subsides");
          }
        }
      }
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(EquippedEvent E)
    {
      GameObject actor = E.Actor;
      actor.RegisterPartEvent((IPart) this, "Desecrated");
      this.UpdateStatShifts(actor);
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(UnequippedEvent E)
    {
      GameObject actor = E.Actor;
      actor.UnregisterPartEvent((IPart) this, "Desecrated");
      this.StatShifter.RemoveStatShifts();
      return base.HandleEvent(E);
    }

    public bool UpdateStatShifts(GameObject who = null)
    {
      if (who == null)
      {
        who = this.ParentObject.Equipped;
        if (who == null)
        {
          this.StatShifter.RemoveStatShifts();
          return false;
        }
      }
      if (!this.ParentObject.IsWorn())
      {
        this.StatShifter.RemoveStatShifts();
        return false;
      }
      int bonus = 0;
      if (this.Duration > 0)
        bonus = this.Bonus;
      this.StatShifter.SetStatShift(who, "Speed", bonus);
      return true;
    }

    public override bool HandleEvent(GetShortDescriptionEvent E)
    {
      E.Postfix.AppendRules("When you desecrate a sultan shrine, +" + this.Bonus.ToString() + " Quickness for " + this.Duration.ToString() + " hours");
      return base.HandleEvent(E);
    }
  }
}
