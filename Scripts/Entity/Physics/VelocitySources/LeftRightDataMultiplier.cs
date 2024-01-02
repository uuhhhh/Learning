using Godot;
using Learning.Scripts.Values.Groups;
using Learning.Scripts.Values.Modifiers;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

[GlobalClass]
public partial class LeftRightDataMultiplier : ModifierResource<LeftRightData> {
    [Export] public FloatMultiplier BaseSpeedMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(BaseSpeedMultiplier));
        private set => AddModifierToBeAdded(nameof(BaseSpeedMultiplier),
            nameof(LeftRightData.BaseSpeed), value);
    }
    [Export] public FloatMultiplier AccelBaseTimeMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(AccelBaseTimeMultiplier));
        private set => AddModifierToBeAdded(nameof(AccelBaseTimeMultiplier),
            nameof(LeftRightData.AccelBaseTime), value);
    }
    [Export] public FloatMultiplier DecelBaseTimeMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(DecelBaseTimeMultiplier));
        private set => AddModifierToBeAdded(nameof(DecelBaseTimeMultiplier),
            nameof(LeftRightData.DecelBaseTime), value);
    }
    [Export] public FloatMultiplier SpeedScaleHighDeltaPowerMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(SpeedScaleHighDeltaPowerMultiplier));
        private set => AddModifierToBeAdded(nameof(SpeedScaleHighDeltaPowerMultiplier),
            nameof(LeftRightData.SpeedScaleHighDeltaPower), value);
    }
}