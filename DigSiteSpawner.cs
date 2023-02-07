using System;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_DigSiteSpawner : IPart
  {
    public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == ZoneActivatedEvent.ID || ID == EndTurnEvent.ID;

    public void checkSpawn()
    {
      if ( this.ParentObject.CurrentCell.ParentZone.FindObject("CRYPTOGEOLOGY_Cryptogeologist")!=null
        || The.Game.HasGameState("CRYPTOGEOLOGY_CryptogeologistKilled")
        || (
          The.Game.GetQuestFinishTime("Retrieve Yet Another Spare Part for the Excavator") <= 0L
          || Calendar.TotalTimeTicks - The.Game.GetQuestFinishTime("Retrieve Yet Another Spare Part for the Excavator") <= 100L
        )
      )
        return;

      this.currentCell.ParentZone.GetCell(45, 16).AddObject("CRYPTOGEOLOGY_Cryptogeologist").SetStringProperty("CRYPTOGEOLOGY_RealCryptogeologist", "1");
      The.Game.SetStringGameState("CRYPTOGEOLOGY_CryptogeologistMoved", "1");
      this.currentCell.ParentZone.GetCell(46, 16).AddObject("CRYPTOGEOLOGY_ExactingExcavator");
      this.currentCell.ParentZone.GetCell(46, 17).AddObject("Campfire");
    }

    public override bool HandleEvent(EndTurnEvent E)
    {
      this.checkSpawn();
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(ZoneActivatedEvent E)
    {
      this.checkSpawn();
      return base.HandleEvent(E);
    }
  }
}
