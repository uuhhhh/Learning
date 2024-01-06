using Godot;
using Learning.Scripts.Values.Groups;
using Learning.Scripts.Values.Modifiers;

namespace Learning.Scripts.Entity.Physics.Intermediate;

/// <summary>
///     A group of modifiers that can be applied to a WallDraggingData.
/// </summary>
[GlobalClass]
public partial class WallDraggingDataMultiplier : ModifierResource<WallDraggingData>
{
    /// <summary>
    ///     A scale factor for the WallDragging's upwards gravity scale replacement.
    /// </summary>
    [Export]
    public FloatMultiplier UpwardsGravityScaleReplacementMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(UpwardsGravityScaleReplacementMultiplier));
        private set => AddModifierToBeAdded(nameof(UpwardsGravityScaleReplacementMultiplier),
            nameof(WallDraggingData.UpwardsGravityScaleReplacement), value);
    }

    /// <summary>
    ///     A scale factor for the WallDragging's downwards gravity scale replacement.
    /// </summary>
    [Export]
    public FloatMultiplier DownwardsGravityScaleReplacementMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(DownwardsGravityScaleReplacementMultiplier));
        private set => AddModifierToBeAdded(nameof(DownwardsGravityScaleReplacementMultiplier),
            nameof(WallDraggingData.DownwardsGravityScaleReplacement), value);
    }

    /// <summary>
    ///     A scale factor for the WallDragging's max fall/drag velocity.
    /// </summary>
    [Export]
    public FloatMultiplier MaxFallVelocityReplacementMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(MaxFallVelocityReplacementMultiplier));
        private set => AddModifierToBeAdded(nameof(MaxFallVelocityReplacementMultiplier),
            nameof(WallDraggingData.MaxFallVelocityReplacement), value);
    }

    /// <summary>
    ///     A scale factor for the WallDragging's ceiling hit-stop time scale replacement.
    /// </summary>
    [Export]
    public FloatMultiplier CeilingHitStopTimeScaleReplacementMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(CeilingHitStopTimeScaleReplacementMultiplier));
        private set => AddModifierToBeAdded(nameof(CeilingHitStopTimeScaleReplacementMultiplier),
            nameof(WallDraggingData.CeilingHitStopTimeScaleReplacement), value);
    }

    /// <summary>
    ///     A scale factor for the WallDragging's replacement for the deceleration time to max velocity
    ///     per 100 velocity.
    /// </summary>
    [Export]
    public FloatMultiplier DecelToMaxVelocityTimePer100VelocityReplacementMultiplier
    {
        get => GetModifier<FloatMultiplier>(
            nameof(DecelToMaxVelocityTimePer100VelocityReplacementMultiplier));
        private set => AddModifierToBeAdded(
            nameof(DecelToMaxVelocityTimePer100VelocityReplacementMultiplier),
            nameof(WallDraggingData.DecelToMaxVelocityTimePer100VelocityReplacement), value);
    }

    /// <summary>
    ///     A scale factor for the WallDragging's threshold for starting wall dragging.
    /// </summary>
    [Export]
    public FloatMultiplier VelocityDragThresholdMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(VelocityDragThresholdMultiplier));
        private set => AddModifierToBeAdded(nameof(VelocityDragThresholdMultiplier),
            nameof(WallDraggingData.VelocityDragThreshold), value);
    }
}