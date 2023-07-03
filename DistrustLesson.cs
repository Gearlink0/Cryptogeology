using System;
using System.Text;
using UnityEngine;
using XRL.World.Effects;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_DistrustLesson : IActivePart
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

    public CRYPTOGEOLOGY_DistrustLesson()
    {
      this.IsBreakageSensitive = true;
      this.WorksOnWearer = true;
    }

    public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade)
      || ID == CommandEvent.ID
      || ID == EquippedEvent.ID
      || ID == UnequippedEvent.ID
      || ID == GetShortDescriptionEvent.ID;
    }

    public override bool FireEvent(Event E)
    {
      if (E.ID == "Desecrated")
      {
        if (E.GetGameObjectParameter("Object") != null && E.GetGameObjectParameter("Object").HasPart("SultanShrine"))
        {
          GameObject Subject = this.GetActivePartFirstSubject();
          if (Subject.IsPlayer())
            XRL.Messages.MessageQueue.AddPlayerMessage("You remember the source of your distrust.");
          ActivatedAbilityEntry ActivatedAbilityEntry = GetMyActivatedAbilities(Subject).GetAbility(this.ActivatedAbilityID);
          if (ActivatedAbilityEntry.Cooldown > 0)
            ActivatedAbilityEntry.Cooldown = 0;
        }
      }
      return base.FireEvent(E);
    }

    public override bool HandleEvent(CommandEvent E)
    {
      if (E.Command == this.GetActivatedAbilityCommandName() && E.Actor == this.GetActivePartFirstSubject())
      {
        XRL.Messages.MessageQueue.AddPlayerMessage("The lesson whispers warriness.");
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
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("You can grow wary, gaining +").Append( this.Bonus.ToString() );
      stringBuilder.Append(" Quickness for ").Append( this.MaxDuration.ToString() );
      stringBuilder.Append(" rounds. Desecrating a sultan shrine refreshes this ability.");
      E.Postfix.AppendRules( stringBuilder.ToString() );
      return base.HandleEvent(E);
    }

    public void SetUpActivatedAbility(GameObject Who = null)
    {
      if (Who == null)
        Who = this.GetActivePartFirstSubject();
      if (Who == null)
        return;
      if (this.ActivatedAbilityID == Guid.Empty)
        this.ActivatedAbilityID = Who.AddActivatedAbility(
          this.ActivatedAbilityName,
          this.GetActivatedAbilityCommandName(),
          this.ActivatedAbilityClass ?? (Who == this.ParentObject ? "Maneuvers" : "Items"),
          Icon: this.ActivatedAbilityIcon
        );
      else
        this.SyncActivatedAbilityName(Who);
    }

    public void SyncActivatedAbilityName(GameObject Who = null)
    {
      if (this.ActivatedAbilityID == Guid.Empty)
        return;
      if (Who == null)
        Who = this.GetActivePartFirstSubject();
      if (Who == null)
        return;
      Who.SetActivatedAbilityDisplayName(this.ActivatedAbilityID, this.ActivatedAbilityName);
    }

    public string GetActivatedAbilityCommandName() => this.ActivatedAbilityCommandNamePrefix + this.ParentObject.id;
  }
}
