using Qud.API;

namespace XRL.World.Conversations.Parts
{
  public class CRYPTOGEOLOGY_StartTheExcavator : IConversationPart
  {
    public override bool WantEvent(int ID, int Propagation) => base.WantEvent(ID, Propagation) || ID == GetTargetElementEvent.ID;

    public override bool HandleEvent(GetTargetElementEvent E)
    {
			The.Game.SetStringGameState("CRYPTOGEOLOGY_ExcavatorStarted", "1");
      return base.HandleEvent(E);
    }
  }
}
