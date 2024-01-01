using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class WallDraggingDataMultiplier : ModifierResource<WallDraggingData> {
    [Export] public FloatMultiplier UpwardsGravityScaleReplacementMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(UpwardsGravityScaleReplacementMultiplier));
        private set => AddModifierToBeAdded(nameof(UpwardsGravityScaleReplacementMultiplier),
            nameof(WallDraggingData.UpwardsGravityScaleReplacement), value);
    }
    [Export] public FloatMultiplier DownwardsGravityScaleReplacementMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(DownwardsGravityScaleReplacementMultiplier));
        private set => AddModifierToBeAdded(nameof(DownwardsGravityScaleReplacementMultiplier),
            nameof(WallDraggingData.DownwardsGravityScaleReplacement), value);
    }
    [Export] public FloatMultiplier MaxFallVelocityReplacementMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(MaxFallVelocityReplacementMultiplier));
        private set => AddModifierToBeAdded(nameof(MaxFallVelocityReplacementMultiplier),
            nameof(WallDraggingData.MaxFallVelocityReplacement), value);
    }
    [Export] public FloatMultiplier CeilingHitStopTimeScaleReplacementMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(CeilingHitStopTimeScaleReplacementMultiplier));
        private set => AddModifierToBeAdded(nameof(CeilingHitStopTimeScaleReplacementMultiplier),
            nameof(WallDraggingData.CeilingHitStopTimeScaleReplacement), value);
    }
    [Export] public FloatMultiplier DecelToMaxVelocityTimePer100VelocityReplacementMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(DecelToMaxVelocityTimePer100VelocityReplacementMultiplier));
        private set => AddModifierToBeAdded(nameof(DecelToMaxVelocityTimePer100VelocityReplacementMultiplier),
            nameof(WallDraggingData.DecelToMaxVelocityTimePer100VelocityReplacement), value);
    }
    [Export] public FloatMultiplier VelocityDragThresholdMultiplier {
        get => GetModifier<FloatMultiplier>(nameof(VelocityDragThresholdMultiplier));
        private set => AddModifierToBeAdded(nameof(VelocityDragThresholdMultiplier),
            nameof(WallDraggingData.VelocityDragThreshold), value);
    }
}