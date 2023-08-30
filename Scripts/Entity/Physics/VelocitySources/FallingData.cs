using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

[GlobalClass]
public partial class FallingData : ResourceWithModifiers {
    [Export] public float UpwardsGravityScale {
        get => GetField<float>(nameof(UpwardsGravityScale));
        private set => SetField(nameof(UpwardsGravityScale), value);
    }
    [Export] public float DownwardsGravityScale {
        get => GetField<float>(nameof(DownwardsGravityScale));
        private set => SetField(nameof(DownwardsGravityScale), value);
    }
    [Export] public float MaxVelocity {
        get => GetField<float>(nameof(MaxVelocity));
        private set => SetField(nameof(MaxVelocity), value);
    }
    [Export] public float CeilingHitStopTimeScale {
        get => GetField<float>(nameof(CeilingHitStopTimeScale));
        private set => SetField(nameof(CeilingHitStopTimeScale), value);
    }
    [Export] public float DecelToMaxVelocityTimePer100Velocity {
        get => GetField<float>(nameof(DecelToMaxVelocityTimePer100Velocity));
        private set => SetField(nameof(DecelToMaxVelocityTimePer100Velocity), value);
    }

    public float DecelToMaxVelocityTimePerVelocity => DecelToMaxVelocityTimePer100Velocity / 100;

    protected override void RefreshAllFields() {
        RefreshField<float>(nameof(UpwardsGravityScale));
        RefreshField<float>(nameof(DownwardsGravityScale));
        RefreshField<float>(nameof(MaxVelocity));
        RefreshField<float>(nameof(CeilingHitStopTimeScale));
        RefreshField<float>(nameof(DecelToMaxVelocityTimePer100Velocity));
    }
}