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
        if (value is not float toMultiply) return value;
        
        float? product = valueName switch {
            nameof(LeftRightData.BaseSpeed) => toMultiply * BaseSpeedMultiplier,
            nameof(LeftRightData.AccelBaseTime) => toMultiply * AccelBaseTimeMultiplier,
            nameof(LeftRightData.DecelBaseTime) => toMultiply * DecelBaseTimeMultiplier,
            nameof(LeftRightData.SpeedScaleHighDeltaPower) => toMultiply * SpeedScaleHighDeltaPowerMultiplier,
            _ => null
        };
        
        return product is TValue productT ? productT : value;
    }
}