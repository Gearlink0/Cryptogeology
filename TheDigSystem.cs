using Qud.API;
using System;

namespace XRL.World.Quests
{
  [Serializable]
  public class CRYPTOGEOLOGY_TheDigSystem : IQuestSystem
  {
		public string secretId = "$CRYPTOGEOLOGY_digsite";

    public override void Start()
    {
      if (this.secretId != null)
      {
	      JournalMapNote mapNote = JournalAPI.GetMapNote(this.secretId);
	      if (mapNote != null && !mapNote.Revealed)
	        mapNote.Reveal((string) null, false);
			}
    }
  }
}
