using System;
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
            return cappedNumJumps is TValue capT ? capT : value;
        }
        
        if (value is Vector2 toMultiplyVec && valueName == nameof(JumpingData.Velocity)) {
            Vector2 productVec = toMultiplyVec * VelocityMultiplier;
            return productVec is TValue productVecT ? productVecT : value;
        }
        
        if (value is not float toMultiply) return value;
        
        float? product = valueName switch {
            nameof(JumpingData.AccelTimeX) => toMultiply * AccelTimeXMultiplier,
            nameof(JumpingData.AccelTimeY) => toMultiply * AccelTimeYMultiplier,
            nameof(JumpingData.CancelVelocity) => toMultiply * CancelVelocityMultiplier,
            nameof(JumpingData.CancelAccelTime) => toMultiply * CancelAccelTimeMultiplier,
            _ => null
        };

        return product is TValue productT ? productT : value;
    }
}