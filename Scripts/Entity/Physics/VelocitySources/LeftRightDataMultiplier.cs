using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

[GlobalClass]
public partial class LeftRightDataMultiplier : Resource, IValueModifier {
    [Export] public float BaseSpeedMultiplier { get; private set; }
    [Export] public float AccelBaseTimeMultiplier { get; private set; }
    [Export] public float DecelBaseTimeMultiplier { get; private set; }
    [Export] public float SpeedScaleHighDeltaPowerMultiplier { get; private set; }
    
    [Export] public int Priority { get; private set; }
    
    public TValue ApplyModifier<TValue>(string valueName, TValue value) {
        float? multiplier = valueName switch {
            nameof(LeftRightData.BaseSpeed) => BaseSpeedMultiplier,
            nameof(LeftRightData.AccelBaseTime) => AccelBaseTimeMultiplier,
            nameof(LeftRightData.DecelBaseTime) => DecelBaseTimeMultiplier,
            nameof(LeftRightData.SpeedScaleHighDeltaPower) => SpeedScaleHighDeltaPowerMultiplier,
            _ => null
        };
        return IValueModifier.MultiplyFloat(value, multiplier);
    }
}