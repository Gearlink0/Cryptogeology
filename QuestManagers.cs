using Qud.API;
using System;
using XRL.World.WorldBuilders;

namespace XRL.World.QuestManagers
{
  [Serializable]
  public class CRYPTOGEOLOGY_TheDigManager : QuestManager
  {
		public string secretId = "$CRYPTOGEOLOGY_digsite";

    public override void OnQuestAdded()
		{
			if (this.secretId != null)
      {
	      JournalMapNote mapNote = JournalAPI.GetMapNote(this.secretId);
	      if (mapNote != null && !mapNote.revealed)
	        mapNote.Reveal(false);
			}
		}
  }
}
