using System;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_StiltSpawner : IPart
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

      // Spawn if the cryptogeologst hasn't moved to the dig site, or its been
      // some time since the dig was finished
      if (
        !The.Game.HasGameState("CRYPTOGEOLOGY_CryptogeologistMoved")
        || (
          The.Game.HasFinishedQuest("The Dig")
          && Calendar.TotalTimeTicks - The.Game.GetQuestFinishTime("The Dig") > 100L
        )
      ) {
        this.currentCell.ParentZone.GetCell(44, 16).AddObject("CRYPTOGEOLOGY_Cryptogeologist");
        this.currentCell.ParentZone.GetCell(45, 16).AddObject("CRYPTOGEOLOGY_ExactingExcavator");
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
