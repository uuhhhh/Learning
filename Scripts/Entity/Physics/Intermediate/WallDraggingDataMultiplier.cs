using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class WallDraggingDataMultiplier : Resource, IValueModifier {
    [Export] public float UpwardsGravityScaleReplacementMultiplier { get; private set; }
    [Export] public float DownwardsGravityScaleReplacementMultiplier { get; private set; }
    [Export] public float MaxVelocityReplacementMultiplier { get; private set; }
    [Export] public float CeilingHitStopTimeScaleReplacementMultiplier { get; private set; }
    [Export] public float DecelToMaxVelocityTimePer100VelocityReplacementMultiplier { get; private set; }
    [Export] public float VelocityDragThresholdMultiplier { get; private set; }
    
    [Export] public int Priority { get; private set; }
    
    public TValue ApplyModifier<TValue>(string valueName, TValue value) {
        float? multiplier = valueName switch {
            nameof(WallDraggingData.UpwardsGravityScaleReplacement) => UpwardsGravityScaleReplacementMultiplier,
            nameof(WallDraggingData.DownwardsGravityScaleReplacement) => DownwardsGravityScaleReplacementMultiplier,
            nameof(WallDraggingData.MaxVelocityReplacement) => MaxVelocityReplacementMultiplier,
            nameof(WallDraggingData.CeilingHitStopTimeScaleReplacement) => CeilingHitStopTimeScaleReplacementMultiplier,
            nameof(WallDraggingData.DecelToMaxVelocityTimePer100VelocityReplacement)
                => DecelToMaxVelocityTimePer100VelocityReplacementMultiplier,
            _ => null
        };
        return IValueModifier.MultiplyFloat(value, multiplier);
    }
}