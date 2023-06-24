using System;
using XRL.Core;

namespace XRL.World.Effects
{
  [Serializable]
  public class CRYPTOGEOLOGY_DistrustEffect : Effect, ITierInitialized
  {
		public int QuicknessShift = 20;
		public int MaxDuration = 20;
    [FieldSaveVersion(307)]
    public string Source;

    public CRYPTOGEOLOGY_DistrustEffect() => this.DisplayName = "wary";

    public CRYPTOGEOLOGY_DistrustEffect(int Duration)
      : this()
    {
      this.Duration = Duration;
    }

    public CRYPTOGEOLOGY_DistrustEffect(int Duration, int QuicknessShift, GameObject Source)
      : this(Duration)
    {
      this.QuicknessShift = QuicknessShift;
      this.Source = Source.id;
    }

    public void Initialize(int Tier) => this.Duration = MaxDuration;

		public override bool UseStandardDurationCountdown() => true;

    public override int GetEffectType() => TYPE_MENTAL + TYPE_REMOVABLE + TYPE_VOLUNTARY;

    public override bool SameAs(Effect e) => false;

    public override string GetDetails() => "+20 Quickness";

    public override bool Apply(GameObject Object)
    {
      if (Object.IsPlayer())
        XRL.Messages.MessageQueue.AddPlayerMessage("Your wariness heightens");
      this.ApplyChanges();
      return true;
    }

    private void ApplyChanges()
    {
      this.StatShifter.SetStatShift("Speed", this.QuicknessShift);
    }

    private void UnapplyChanges() => this.StatShifter.RemoveStatShifts();

    public override void Remove(GameObject Object)
    {
      this.UnapplyChanges();
      if (Object.IsPlayer())
        XRL.Messages.MessageQueue.AddPlayerMessage("Your heightened wariness subsides");
      base.Remove(Object);
    }

    public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == BeginTakeActionEvent.ID;

    public override bool HandleEvent(BeginTakeActionEvent E)
    {
      GameObject byId = GameObject.findById(this.Source);
      if (!GameObject.validate(ref byId) || byId.Equipped != this.Object)
        this.Duration = 0;
      return base.HandleEvent(E);
    }
  }
}
