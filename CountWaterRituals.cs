using Qud.API;
using System;
using UnityEngine;

namespace XRL.World.Parts
{
  [Serializable]
  public class CRYPTOGEOLOGY_CountWaterRituals : IPart
  {
    public int WaterRitualsPerformed = 0;

	  public override bool WantEvent(int ID, int cascade) {
	    return base.WantEvent(ID, cascade)
			|| ID == AfterPlayerBodyChangeEvent.ID
	    || ID == GetWaterRitualReputationAmountEvent.ID
	    || ID == WaterRitualStartEvent.ID;
	  }

		public override bool HandleEvent(AfterPlayerBodyChangeEvent E)
	  {
			if( E.OldBody != null && E.NewBody != null ){
				E.OldBody.RemovePart(this);
		    E.NewBody.AddPart(this);
			}
	    return base.HandleEvent(E);
	  }

	  public override bool HandleEvent(WaterRitualStartEvent E)
	  {
	    if (E.Actor.IsPlayer() && !E.Record.Has("CRYPTOGEOLOGY_CountedWaterRitual"))
	    {
	      E.Record.attributes.Add("CRYPTOGEOLOGY_CountedWaterRitual");
	      this.WaterRitualsPerformed += 1;
	    }
	    return base.HandleEvent(E);
	  }

	  public override bool HandleEvent(GetWaterRitualReputationAmountEvent E)
	  {
	    if (E.Actor.IsPlayer() && !E.Record.Has("CRYPTOGEOLOGY_CountedWaterRitual"))
	    {
	      E.Record.attributes.Add("CRYPTOGEOLOGY_CountedWaterRitual");
	      this.WaterRitualsPerformed += 1;
	    }
	    return base.HandleEvent(E);
	  }
  }
}
