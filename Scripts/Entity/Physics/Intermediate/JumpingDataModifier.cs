using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class JumpingDataModifier : ModifierResource<JumpingData> {
    [Export] public IntCapper NumJumpsCap {
        get => GetModifier<IntCapper>(nameof(NumJumpsCap));
        private set => AddModifierToBeAdded(nameof(NumJumpsCap),
            nameof(JumpingData.NumJumps), value);
    }
    [Export] public VectorScaler VelocityMultiplier {
        get => GetModifier<VectorScaler>(nameof(VelocityMultiplier));
        private set => AddModifierToBeAdded(nameof(VelocityMultiplier),
            nameof(JumpingData.Velocity), value);
    }
    [Export] public FloatMultiplier AccelTimeXMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(AccelTimeXMultiplier));
        private set => AddModifierToBeAdded(nameof(AccelTimeXMultiplier),
            nameof(JumpingData.AccelTimeX), value);
    }
    [Export] public FloatMultiplier AccelTimeYMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(AccelTimeYMultiplier));
        private set => AddModifierToBeAdded(nameof(AccelTimeYMultiplier),
            nameof(JumpingData.AccelTimeY), value);
    }
    [Export] public FloatMultiplier CancelVelocityMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(CancelVelocityMultiplier));
        private set => AddModifierToBeAdded(nameof(CancelVelocityMultiplier),
            nameof(JumpingData.CancelVelocity), value);
    }
    [Export] public FloatMultiplier CancelAccelTimeMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(CancelAccelTimeMultiplier));
        private set => AddModifierToBeAdded(nameof(CancelAccelTimeMultiplier),
            nameof(JumpingData.CancelAccelTime), value);
    }
}