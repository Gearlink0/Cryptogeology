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
      this.IsBreakageSensitive = true;
      this.WorksOnWearer = true;
    }

		public override bool WantEvent(int ID, int cascade) {
      return base.WantEvent(ID, cascade)
      || ID == GetCooldownEvent.ID
      || ID == EquippedEvent.ID
      || ID == UnequippedEvent.ID
      || ID == GetShortDescriptionEvent.ID;
    }

    public override bool FireEvent(Event E)
    {
      if (
        this.IsReady()
        && (E.ID == "DefenderHit" || E.ID == "DefenderMissileWeaponHit")
        && E.GetIntParameter("Penetrations") <= 0
        && Stat.Random(1, 100) <= this.FearOnPenetrationChance
      )
      {
        XRL.Messages.MessageQueue.AddPlayerMessage("You do not flinch at the attack.");
        GameObject Attacker = E.GetGameObjectParameter("Attacker");
        GameObject Subject = this.GetActivePartFirstSubject();
        // Use an instance of MenacingStare to access it's default RatingBase
        // and RatingOffset
        Persuasion_MenacingStare MenacingStare = new Persuasion_MenacingStare();
        this.PerformMentalAttack(
          new Mental.Attack(Terrified.OfAttacker),
          Subject,
          Attacker,
          Command: "Terrify MenacingStare",
          Dice: MenacingStare.RatingBase,
          Type: 8388612,
          Magnitude: "6d4".RollCached(),
          AttackModifier: (Subject.StatMod("Ego") + MenacingStare.RatingOffset)
        );
      }
      return base.FireEvent(E);
    }

		public override bool HandleEvent(GetCooldownEvent E)
    {
      if (
        this.IncludeAbilities.CachedCommaExpansion().Contains(E.Ability.DisplayName)
        && this.IsObjectActivePartSubject(E.Actor)
      )
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
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Provides ").Append(PercentageReduction).Append("% reduction in the ");
      List<string> words = this.IncludeAbilities.CachedCommaExpansion();
      stringBuilder.Append(words.Count == 1 ? "cooldown of the ability " : "cooldowns of the abilities ");
      stringBuilder.Append(ColorUtility.StripFormatting(words.Count == 1 ? words[0] : Grammar.MakeAndList(words)));
      stringBuilder.Append('.');
      stringBuilder.Append("\nWhenever an attack hits you but does not penetrate, there is a ");
      stringBuilder.Append(this.FearOnPenetrationChance.ToString());
      stringBuilder.Append("% chance you stare down the attacker.");
      E.Postfix.AppendRules( stringBuilder.ToString() );
      return base.HandleEvent(E);
    }
  }
}
