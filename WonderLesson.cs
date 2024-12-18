using Qud.API;
using System;
using System.Collections.Generic;
using System.Text;
using XRL.Language;
using XRL.Rules;
using XRL.UI;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_WonderLesson : IActivePart
  {
    public int Chance = 10;
    [NonSerialized]
    public Dictionary<string, bool> Visited = new Dictionary<string, bool>();

    public CRYPTOGEOLOGY_WonderLesson()
    {
      this.IsBreakageSensitive = true;
      this.WorksOnWearer = true;
    }

    public override void Read(GameObject Basis, SerializationReader Reader)
    {
      this.Visited = Reader.ReadDictionary<string, bool>();
      base.Read(Basis, Reader);
    }

    public override void Write(GameObject Basis, SerializationWriter Writer)
    {
      Writer.Write<string, bool>(this.Visited);
      base.Write(Basis, Writer);
    }

    public override bool AllowStaticRegistration() => false;

    public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == GetShortDescriptionEvent.ID;

    public override void Register(GameObject Object, IEventRegistrar Registrar)
    {
      Registrar.Register("EnteredCell");
      Registrar.Register("Equipped");
      Registrar.Register("Unequipped");
      base.Register(Object, Registrar);
    }

    public override bool FireEvent(Event E)
    {
      if (E.ID == "EnteredCell")
      {
        GameObject Subject = this.GetActivePartFirstSubject();
        if (Subject != null && Subject.IsPlayer())
        {
          Zone CurrentZone = Subject.CurrentCell.ParentZone;
          Stomach Stomach = (Stomach) Subject.GetPart("Stomach");
          if (!CurrentZone.IsWorldMap() && Stomach.HungerLevel == 0)
          {
            if (this.Visited.ContainsKey(CurrentZone.ZoneID))
              return true;
            this.Visited.Add(CurrentZone.ZoneID, true);
            if (Stat.Random(0, 100) <= this.Chance)
            {
              IBaseJournalEntry RandomUnrevealedNote = JournalAPI.GetRandomUnrevealedNote();
              StringBuilder stringBuilder = new StringBuilder();
              stringBuilder.Append("Your head throbs with the wonder of ancient, squirming, things and you piece together a truth:\n\n");
              if( RandomUnrevealedNote as JournalMapNote == null )
                stringBuilder.Append( RandomUnrevealedNote.Text );
              else
                stringBuilder.Append( "The location of " ).Append( Grammar.InitLowerIfArticle(RandomUnrevealedNote.Text) );
              Popup.Show(stringBuilder.ToString());
              RandomUnrevealedNote.Reveal();
            }
          }
        }
      }
      else if (E.ID == "Equipped")
        E.GetGameObjectParameter("EquippingObject").RegisterPartEvent((IPart) this, "EnteredCell");
      else if (E.ID == "Unequipped")
        E.GetGameObjectParameter("UnequippingObject").UnregisterPartEvent((IPart) this, "EnteredCell");
      return base.FireEvent(E);
    }

    public override bool HandleEvent(GetShortDescriptionEvent E)
    {
      E.Postfix.AppendRules("On entering a new zone while sated, learn a secret " + (this.Chance).ToString() + " % of the time.");
      return base.HandleEvent(E);
    }
  }
}
