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
			XRL.Messages.MessageQueue.AddPlayerMessage("OnQuestAdded");
			if (this.secretId != null)
      {
				XRL.Messages.MessageQueue.AddPlayerMessage("secret id not null");
	      JournalMapNote mapNote = JournalAPI.GetMapNote(this.secretId);
	      if (mapNote != null && !mapNote.revealed)
				{
					XRL.Messages.MessageQueue.AddPlayerMessage("map note not null");
	        mapNote.Reveal(false);
				}
			}
		}
  }
}
