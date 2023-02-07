using System;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_Neurolith : IPart
  {
    public override bool SameAs(IPart p) => false;

    public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == AfterConversationEvent.ID;

    public override bool HandleEvent(AfterConversationEvent E)
    {
			if( The.Game.FinishedQuestStep("The Dig~Attend the Dig") ){
			  this.ParentObject.DustPuff();
        this.ParentObject.Destroy();
      }
      return base.HandleEvent(E);
    }
  }
}
