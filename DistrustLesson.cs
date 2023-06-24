using System;
using UnityEngine;
using XRL.World.Effects;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_DistrustLesson : IPart
  {
    public int Bonus = 20;
    public int MaxDuration = 20;
    public int Duration = 0;
    public string ActivatedAbilityName = "Grow wary";
    public string ActivatedAbilityCommandNamePrefix = "ActivateDistrustLesson";
    [FieldSaveVersion(236)]
    public string ActivatedAbilityClass;
    [FieldSaveVersion(236)]
    public string ActivatedAbilityIcon = "û";
    [FieldSaveVersion(236)]
    public Guid ActivatedAbilityID;


    public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade) || ID == CommandEvent.ID || ID == EquippedEvent.ID  || ID == UnequippedEvent.ID || ID == GetShortDescriptionEvent.ID;
    }

    public override bool FireEvent(Event E)
    {
      // TODO: Find a good way to verify that the actor desecrating the shrine is
      // the one equipping the lesson
      if (E.ID == "Desecrated")
      {
        if (E.GetGameObjectParameter("Object") != null && E.GetGameObjectParameter("Object").HasPart("SultanShrine"))
        {
          if (this.ParentObject.Equipped.IsPlayer())
            XRL.Messages.MessageQueue.AddPlayerMessage("You remember the source of your distrust");
          ActivatedAbilityEntry ActivatedAbilityEntry = GetMyActivatedAbilities(this.ParentObject.Equipped).GetAbility(this.ActivatedAbilityID);
          if (ActivatedAbilityEntry.Cooldown > 0)
            ActivatedAbilityEntry.Cooldown = 0;
        }
      }
      return base.FireEvent(E);
    }

    public override bool HandleEvent(CommandEvent E)
    {
      if (E.Command == this.GetActivatedAbilityCommandName() && E.Actor == this.ParentObject.Equipped)
      {
        XRL.Messages.MessageQueue.AddPlayerMessage("The lesson whispers warriness");
        E.Actor.ApplyEffect((Effect) new CRYPTOGEOLOGY_DistrustEffect(MaxDuration, Bonus, this.ParentObject));
        E.Actor.CooldownActivatedAbility(this.ActivatedAbilityID, 1200);
      }
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(EquippedEvent E)
    {
      E.Actor.RegisterPartEvent((IPart) this, "Desecrated");
      E.Actor.RegisterPartEvent((IPart) this, this.GetActivatedAbilityCommandName());
      this.SetUpActivatedAbility(E.Actor);
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(UnequippedEvent E)
    {
      E.Actor.UnregisterPartEvent((IPart) this, "Desecrated");
      E.Actor.UnregisterPartEvent((IPart) this, this.GetActivatedAbilityCommandName());
      E.Actor.RemoveActivatedAbility(ref this.ActivatedAbilityID);
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(GetShortDescriptionEvent E)
    {
      E.Postfix.AppendRules("You can grow wary, gaining +" + this.Bonus.ToString() + " Quickness for " + this.Duration.ToString() + " rounds. Desecrating a sulten shrine refreshes this ability.");
      return base.HandleEvent(E);
    }

    public void SetUpActivatedAbility(GameObject Who = null)
    {
      if (Who == null)
        Who = this.ParentObject.Equipped;
      if (Who == null)
        return;
      if (this.ActivatedAbilityID == Guid.Empty)
        this.ActivatedAbilityID = Who.AddActivatedAbility(this.ActivatedAbilityName, this.GetActivatedAbilityCommandName(), this.ActivatedAbilityClass ?? (Who == this.ParentObject ? "Maneuvers" : "Items"), Icon: this.ActivatedAbilityIcon);
      else
        this.SyncActivatedAbilityName(Who);
    }

    public void SyncActivatedAbilityName(GameObject Who = null)
    {
      if (this.ActivatedAbilityID == Guid.Empty)
        return;
      if (Who == null)
        Who = this.ParentObject.Equipped;
      Who.SetActivatedAbilityDisplayName(this.ActivatedAbilityID, this.ActivatedAbilityName);
    }

    public string GetActivatedAbilityCommandName() => this.ActivatedAbilityCommandNamePrefix + this.ParentObject.id;
  }
}
