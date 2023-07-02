using XRL;
using XRL.World;
using XRL.World.Parts;

[PlayerMutator]
[HasCallAfterGameLoadedAttribute]
public class CRYPTOGEOLOGY_AddCountWaterRitualsToPlayer : IPlayerMutator
{
	public void mutate(GameObject player)
	{
    player.AddPart<CRYPTOGEOLOGY_CountWaterRituals>();
	}

	[CallAfterGameLoadedAttribute]
  public static void AfterLoadGameCallback()
  {
    GameObject player = The.Player;
    if (player != null)
		{
      player.RequirePart<CRYPTOGEOLOGY_CountWaterRituals>();
		}
  }
}
