using System;
using System.Collections.Generic;
using XRL.Core;
using XRL.Rules;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_ExactingExcavator : IPart
  {
		public int QuestTurnsDigging = 0;
    private int lastSparkFrame;

		public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade)
      || ID == EndTurnEvent.ID
      || ID == TookDamageEvent.ID;
    }

    public override bool HandleEvent(EndTurnEvent E)
    {
      // If the excavator is at the dig site, has been started, but hasn't found
      // the neurolith, keep digging
			if(
        this.ParentObject.CurrentCell.ParentZone.FindObject("CRYPTOGEOLOGY_DigSiteSpawner")!=null
        && The.Game.HasGameState("CRYPTOGEOLOGY_ExcavatorStarted")
        && !The.Game.HasGameState("CRYPTOGEOLOGY_NeurolithSpawned")
      ) {
				if( QuestTurnsDigging < 5 ) {
					QuestTurnsDigging += 1;
				}
				else {
          List<Cell> emptyAdjacentCells = this.ParentObject.CurrentCell.GetEmptyAdjacentCells(1, 1);
          emptyAdjacentCells.RemoveRandomElement<Cell>()?.AddObject("CRYPTOGEOLOGY_Neurolith");
          The.Game.SetStringGameState("CRYPTOGEOLOGY_NeurolithSpawned", "1");
				}
        this.ParentObject.DustPuff();
			}
			return true;
    }

    public override bool HandleEvent(TookDamageEvent E)
    {
      if (E.Actor != this.ParentObject)
        this.ParentObject.pPhysics.BroadcastForHelp(E.Actor);
      return true;
    }

    public override bool Render(RenderEvent E)
    {
      // If the excavator hasn't been fixed yet, it should spark
      if (
        The.Game.GetQuestFinishTime("Retrieve Yet Another Spare Part for the Excavator") <= 0L
        && this.lastSparkFrame != XRLCore.CurrentFrame
      )
      {
        this.lastSparkFrame = XRLCore.CurrentFrame;
        if (Stat.RandomCosmetic(1, 120) <= 2)
        {
          for (int index = 0; index < 2; ++index)
            this.ParentObject.ParticleText("&Y" + ((char) Stat.RandomCosmetic(191, 198)).ToString(), 0.2f, 20);
          for (int index = 0; index < 2; ++index)
            this.ParentObject.ParticleText("&W\u000F", 0.02f, 10);
          this.PlayWorldSound("Electric", 0.35f, 0.35f);
        }
      }
      return true;
    }
  }
}
