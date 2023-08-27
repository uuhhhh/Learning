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
        if (value is not float toMultiply) return value;
        
        float? product = valueName switch {
            nameof(FallingData.UpwardsGravityScale) => toMultiply * UpwardsGravityScaleMultiplier,
            nameof(FallingData.DownwardsGravityScale) => toMultiply * DownwardsGravityScaleMultiplier,
            nameof(FallingData.MaxVelocity) => toMultiply * MaxVelocityMultiplier,
            nameof(FallingData.CeilingHitStopTimeScale) => toMultiply * CeilingHitStopTimeScaleMultiplier,
            nameof(FallingData.DecelToMaxVelocityTimePer100Velocity) => toMultiply * DecelToMaxVelocityTimePer100VelocityMultiplier,
            _ => null
        };

        return product is TValue productT ? productT : value;
    }
}