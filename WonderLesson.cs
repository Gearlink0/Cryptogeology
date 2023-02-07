using Qud.API;
using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.Rules;
using XRL.UI;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_WonderLesson : IPart
  {
    public int Chance = 10;
    [NonSerialized]
    public Dictionary<string, bool> Visited = new Dictionary<string, bool>();

    public override void LoadData(SerializationReader Reader)
    {
      this.Visited = Reader.ReadDictionary<string, bool>();
      base.LoadData(Reader);
    }

    public override void SaveData(SerializationWriter Writer)
    {
      Writer.Write<string, bool>(this.Visited);
      base.SaveData(Writer);
    }

    public override bool AllowStaticRegistration() => false;

    public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == GetShortDescriptionEvent.ID;

    public override void Register(GameObject Object)
    {
      Object.RegisterPartEvent((IPart) this, "EnteredCell");
      Object.RegisterPartEvent((IPart) this, "Equipped");
      Object.RegisterPartEvent((IPart) this, "Unequipped");
      base.Register(Object);
    }

    public override bool FireEvent(Event E)
    {
      if (E.ID == "EnteredCell")
      {
        if (this.ParentObject.pPhysics.Equipped != null && this.ParentObject.pPhysics.Equipped.IsPlayer())
        {
          string zoneId = IComponent<GameObject>.ThePlayer.pPhysics.CurrentCell.ParentZone.ZoneID;
          Stomach stomach = IComponent<GameObject>.ThePlayer.GetPart("Stomach") as Stomach;
          if (!IComponent<GameObject>.ThePlayer.pPhysics.CurrentCell.ParentZone.IsWorldMap() && stomach.HungerLevel == 0)
          {
            if (this.Visited.ContainsKey(zoneId))
              return true;
            this.Visited.Add(zoneId, true);
            if (Stat.Random(0, 100) <= this.Chance)
            {
              IBaseJournalEntry randomUnrevealedNote = JournalAPI.GetRandomUnrevealedNote();
              Popup.Show("Your head throbs with the wonder of ancient, squirming, things and you piece together a truth:\n\n" + (randomUnrevealedNote as JournalMapNote == null ? randomUnrevealedNote.text : "The location of " + Grammar.InitLowerIfArticle(randomUnrevealedNote.text)));
              randomUnrevealedNote.Reveal();
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
