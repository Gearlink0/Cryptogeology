using System;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_Cryptogeologist : IPart
  {
    public override bool SameAs(IPart p) => false;

    public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == ZoneActivatedEvent.ID;

    public override bool HandleEvent(ZoneActivatedEvent E)
    {
      // If the cryptogeologist moved to the dig site, but this instance is in the
      // stilt, delete this instance
      if (
        The.Game.HasGameState("CRYPTOGEOLOGY_CryptogeologistMoved")
        && this.ParentObject.CurrentCell.ParentZone.FindObject("CRYPTOGEOLOGY_StiltSpawner") != null
      )
      {
        this.ParentObject.CurrentCell.ParentZone.FindObject("CRYPTOGEOLOGY_ExactingExcavator")?.Destroy();
        this.ParentObject.Destroy();
      }

      // If the dig has been completed, but this instance is in the dig site,
      // delete this instance
      if (
        The.Game.HasFinishedQuest("The Dig")
        && this.ParentObject.CurrentCell.ParentZone.FindObject("CRYPTOGEOLOGY_DigSiteSpawner") != null
      )
      {
        this.ParentObject.CurrentCell.ParentZone.FindObject("CRYPTOGEOLOGY_ExactingExcavator")?.Destroy();
        this.ParentObject.Destroy();
      }

      return base.HandleEvent(E);
    }
  }
}
