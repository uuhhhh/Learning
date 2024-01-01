using Godot;
using Learning.Scripts.Values;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

[GlobalClass]
public partial class FallingData : ResourceWithModifiers {
    [Export] public float UpwardsGravityScale {
        get => GetValue<float>(nameof(UpwardsGravityScale));
        private set => InitValue(nameof(UpwardsGravityScale), value);
    }
    [Export] public float DownwardsGravityScale {
        get => GetValue<float>(nameof(DownwardsGravityScale));
        private set => InitValue(nameof(DownwardsGravityScale), value);
    }
    [Export] public float MaxFallVelocity {
        get => GetValue<float>(nameof(MaxFallVelocity));
        private set => InitValue(nameof(MaxFallVelocity), value);
    }
    [Export] public float CeilingHitStopTimeScale {
        get => GetValue<float>(nameof(CeilingHitStopTimeScale));
        private set => InitValue(nameof(CeilingHitStopTimeScale), value);
    }
    [Export] public float DecelToMaxVelocityTimePer100Velocity {
        get => GetValue<float>(nameof(DecelToMaxVelocityTimePer100Velocity));
        private set => InitValue(nameof(DecelToMaxVelocityTimePer100Velocity), value);
    }

    public float DecelToMaxVelocityTimePerVelocity => DecelToMaxVelocityTimePer100Velocity / 100;
}