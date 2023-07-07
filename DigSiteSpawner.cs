using System;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_DigSiteSpawner : IPart
  {
    public override bool WantEvent(int ID, int cascade) {
      return base.WantEvent(ID, cascade)
      || ID == ZoneActivatedEvent.ID
      || ID == EndTurnEvent.ID;
    }

    public void checkSpawn()
    {
      // Don't spawn the cryptogeologist and camp if he's already here or has
      // been killed
      if (
        this.ParentObject.CurrentCell.ParentZone.FindObject("CRYPTOGEOLOGY_Cryptogeologist") != null
        || The.Game.HasGameState("CRYPTOGEOLOGY_CryptogeologistKilled")
      )
        return;

      // Spawn if it's been some time since the excavator was fixed and it hasn't
      // been too long since the dig was finished
      if (
        The.Game.HasFinishedQuest("Retrieve Yet Another Spare Part for the Excavator")
        && Calendar.TotalTimeTicks - The.Game.GetQuestFinishTime("Retrieve Yet Another Spare Part for the Excavator") > 100L
        && !(
        The.Game.HasFinishedQuest("The Dig")
        && Calendar.TotalTimeTicks - The.Game.GetQuestFinishTime("The Dig") > 100L
        )
      ) {
        // Spawn the cryptogeologist, camp and mark that he moved
        this.currentCell.ParentZone.GetCell(45, 16).AddObject("CRYPTOGEOLOGY_Cryptogeologist").SetStringProperty("CRYPTOGEOLOGY_RealCryptogeologist", "1");
        this.currentCell.ParentZone.GetCell(46, 16).AddObject("CRYPTOGEOLOGY_ExactingExcavator");
        this.currentCell.ParentZone.GetCell(46, 17).AddObject("Campfire");
        The.Game.SetStringGameState("CRYPTOGEOLOGY_CryptogeologistMoved", "1");
      }
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
