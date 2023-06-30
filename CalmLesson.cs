using ConsoleLib.Console;
using System;
using System.Collections.Generic;
using System.Text;
using XRL.Language;
using XRL.Rules;
using XRL.World.Capabilities;
using XRL.World.Effects;
using XRL.World.Parts.Skill;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_CalmLesson : IActivePart
  {
    public string PercentageReduction = "50";
		public string IncludeAbilities = "Menacing Stare,Intimidate";
    public int FearOnPenetrationChance = 2;

    public CRYPTOGEOLOGY_CalmLesson()
    {
      WorksOnWearer = true;
    }

		public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == GetCooldownEvent.ID || ID == EquippedEvent.ID  || ID == UnequippedEvent.ID || ID == GetShortDescriptionEvent.ID;

    public override bool FireEvent(Event E)
    {
      if ((E.ID == "DefenderHit" || E.ID == "DefenderMissileWeaponHit") && E.GetIntParameter("Penetrations") <= 0 && Stat.Random(1, 100) <= this.FearOnPenetrationChance)
      {
        XRL.Messages.MessageQueue.AddPlayerMessage("You do not flinch at the attack");
        GameObject Attacker = E.GetGameObjectParameter("Attacker");
        // TODO: Terrible hack, find a better way
        Persuasion_MenacingStare test = new Persuasion_MenacingStare();
        this.PerformMentalAttack(
          new Mental.Attack(Terrified.OfAttacker),
          this.ParentObject.Equipped,
          Attacker,
          Command: "Terrify MenacingStare",
          Dice: test.RatingBase,
          Type: 8388612,
          Magnitude: "6d4".RollCached(),
          AttackModifier: (this.ParentObject.Equipped.StatMod("Ego") + test.RatingOffset)
        );
      }
      return base.FireEvent(E);
    }

		public override bool HandleEvent(GetCooldownEvent E)
    {
      if (this.IncludeAbilities.CachedCommaExpansion().Contains(E.Ability.DisplayName) && this.IsObjectActivePartSubject(E.Actor))
      {
        if (!string.IsNullOrEmpty(this.PercentageReduction))
				{
          E.PercentageReduction += this.PercentageReduction.RollCached();
				}
			}
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(EquippedEvent E)
    {
      E.Actor.RegisterPartEvent((IPart) this, "DefenderHit");
      E.Actor.RegisterPartEvent((IPart) this, "DefenderMissileWeaponHit");
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(UnequippedEvent E)
    {
      E.Actor.UnregisterPartEvent((IPart) this, "DefenderHit");
      E.Actor.RegisterPartEvent((IPart) this, "DefenderMissileWeaponHit");
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(GetShortDescriptionEvent E)
    {
      if ((this.WorksOnEquipper || this.WorksOnWearer || this.WorksOnHolder || this.WorksOnCarrier || this.WorksOnImplantee) && (!string.IsNullOrEmpty(this.PercentageReduction)))
      {
        StringBuilder postfix = E.Postfix;
        postfix.Append("\n{{rules|");
        postfix.Append("Provides ").Append(PercentageReduction).Append("% reduction in the ");

        postfix.Append(" in the ");
        List<string> words = this.IncludeAbilities.CachedCommaExpansion();
        postfix.Append(words.Count == 1 ? "cooldown of the ability " : "cooldowns of the abilities ");
        postfix.Append(ColorUtility.StripFormatting(words.Count == 1 ? words[0] : Grammar.MakeAndList(words)));
        postfix.Append('.');

        postfix.Append("\nWhenever an attack hits you but does not penetrate, there is a ");
        postfix.Append(this.FearOnPenetrationChance.ToString());
        postfix.Append("% chance you stare down the attacker.");
        this.AddStatusSummary(postfix);
        postfix.Append("}}");
      }
      return base.HandleEvent(E);
    }
  }
}
