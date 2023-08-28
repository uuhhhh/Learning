using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class JumpingDataModifier : Resource, IValueModifier {
    [Export] public int NumJumpsCap { get; private set; }
    [Export] public Vector2 VelocityMultiplier { get; private set; }
    [Export] public float AccelTimeXMultiplier { get; private set; }
    [Export] public float AccelTimeYMultiplier { get; private set; }
    [Export] public float CancelVelocityMultiplier { get; private set; }
    [Export] public float CancelAccelTimeMultiplier { get; private set; }
    
    [Export] public int Priority { get; private set; }

    public TValue ApplyModifier<TValue>(string valueName, TValue value) {
        if (value is int toCap && valueName == nameof(JumpingData.NumJumps)) {
            int cappedNumJumps = JumpingData.MinNumJumps(toCap, NumJumpsCap);
            return IValueModifier.Cast<int, TValue>(cappedNumJumps);
        }
        
        if (value is Vector2 toMultiplyVec && valueName == nameof(JumpingData.Velocity)) {
            Vector2 productVec = toMultiplyVec * VelocityMultiplier;
            return IValueModifier.Cast<Vector2, TValue>(productVec);
        }
        
        float? multiplier = valueName switch {
            nameof(JumpingData.AccelTimeX) => AccelTimeXMultiplier,
            nameof(JumpingData.AccelTimeY) => AccelTimeYMultiplier,
            nameof(JumpingData.CancelVelocity) => CancelVelocityMultiplier,
            nameof(JumpingData.CancelAccelTime) => CancelAccelTimeMultiplier,
            _ => null
        };
        return IValueModifier.MultiplyFloat(value, multiplier);
    }
}