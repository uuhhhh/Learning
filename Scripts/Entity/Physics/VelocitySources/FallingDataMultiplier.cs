using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

[GlobalClass]
public partial class FallingDataMultiplier : Resource, IValueModifier {
    [Export] public float UpwardsGravityScaleMultiplier { get; private set; }
    [Export] public float DownwardsGravityScaleMultiplier { get; private set; }
    [Export] public float MaxVelocityMultiplier { get; private set; }
    [Export] public float CeilingHitStopTimeScaleMultiplier { get; private set; }
    [Export] public float DecelToMaxVelocityTimePer100VelocityMultiplier { get; private set; }
    
    [Export] public int Priority { get; private set; }

    public TValue ApplyModifier<TValue>(string valueName, TValue value) {
        float? multiplier = valueName switch {
            nameof(FallingData.UpwardsGravityScale) => UpwardsGravityScaleMultiplier,
            nameof(FallingData.DownwardsGravityScale) => DownwardsGravityScaleMultiplier,
            nameof(FallingData.MaxFallVelocity) => MaxVelocityMultiplier,
            nameof(FallingData.CeilingHitStopTimeScale) => CeilingHitStopTimeScaleMultiplier,
            nameof(FallingData.DecelToMaxVelocityTimePer100Velocity) => DecelToMaxVelocityTimePer100VelocityMultiplier,
            _ => null
        };
        return IValueModifier.MultiplyFloat(value, multiplier);
    }
}