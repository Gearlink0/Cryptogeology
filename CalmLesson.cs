using ConsoleLib.Console;
using System;
using System.Collections.Generic;
using System.Text;
using XRL.Language;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_CalmLesson : IActivePart
  {
    public string PercentageReduction = "50";
		public string IncludeAbilities = "Menacing Stare,Intimidate";

    public CRYPTOGEOLOGY_CalmLesson()
    {
      WorksOnWearer = true;
    }

		public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == GetCooldownEvent.ID || ID == GetShortDescriptionEvent.ID;

		public override bool HandleEvent(GetCooldownEvent E)
    {
      if (this.Applicable(E.Ability) && this.IsObjectActivePartSubject(E.Actor))
      {
        if (!string.IsNullOrEmpty(this.PercentageReduction))
				{
          E.PercentageReduction += this.PercentageReduction.RollCached();
				}
			}
      return base.HandleEvent(E);
    }

		private bool Applicable(ActivatedAbilityEntry Ability)
		{
			return( this.IncludeAbilities.CachedCommaExpansion().Contains(Ability.DisplayName) );
		}

    public override bool HandleEvent(GetShortDescriptionEvent E)
    {
      if ((this.WorksOnEquipper || this.WorksOnWearer || this.WorksOnHolder || this.WorksOnCarrier || this.WorksOnImplantee) && (!string.IsNullOrEmpty(this.PercentageReduction)))
      {
        StringBuilder postfix = E.Postfix;
        postfix.Append("\n{{rules|");
        bool flag1 = false;
        bool flag2 = false;
        if (!string.IsNullOrEmpty(this.PercentageReduction))
        {
          int num1 = this.PercentageReduction.RollMin();
          int num2 = this.PercentageReduction.RollMax();
          if (num1 >= 0 && num2 > 0)
          {
						postfix.Append("Provides ").Append(num1);
            if (num1 != num2)
              postfix.Append('-').Append(num2);
            postfix.Append('%');
            flag1 = true;
          }
          else if (num1 < 0 && num2 <= 0)
          {
						postfix.Append("Causes ").Append(-num2).Append('%');
            if (num1 != num2)
              postfix.Append(" to ").Append(-num1).Append('%');
            flag2 = true;
          }
          else if (num1 != 0 || num2 != 0)
          {
						postfix.Append("Confers ").Append(num1).Append('%');
            if (num1 != num2)
              postfix.Append(" to ").Append(num2).Append('%');
            flag1 = true;
            flag2 = true;
          }
        }
        if (flag1 & flag2)
        {
          postfix.Append(" reduction/increase");
        }
        else
        {
          if (flag1)
            postfix.Append(" reduction");
          if (flag2)
            postfix.Append(" increase");
        }
        if (flag1 | flag2)
        {
          postfix.Append(" in the ");
          if (!string.IsNullOrEmpty(this.IncludeAbilities))
          {
            List<string> words = this.IncludeAbilities.CachedCommaExpansion();
            postfix.Append(words.Count == 1 ? "cooldown of the ability " : "cooldowns of the abilities ").Append(ColorUtility.StripFormatting(words.Count == 1 ? words[0] : Grammar.MakeAndList(words)));
          }
          else
            postfix.Append("cooldowns of activated abilities");
          postfix.Append('.');
        }
        this.AddStatusSummary(postfix);
        postfix.Append("}}");
      }
      return base.HandleEvent(E);
    }
  }
}
