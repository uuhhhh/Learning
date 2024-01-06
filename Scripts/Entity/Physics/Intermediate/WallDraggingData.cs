using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;
using Learning.Scripts.Values.Groups;
using Learning.Scripts.Values.Modifiers;

namespace Learning.Scripts.Entity.Physics.Intermediate;

/// <summary>
///     Data used by a WallDragging to determine wall dragging movement: threshold velocity,
///     max drag velocity, etc. Modifies a FallingData to replace its values with values that
///     resemble dragging on a wall.
/// </summary>
[GlobalClass]
public partial class WallDraggingData : ResourceWithModifiers, IModifierGroup<FallingData>
{
    /// <summary>
    ///     What to replace the Falling's upwards gravity scale with.
    /// </summary>
    [Export]
    public ModifiedFloatReplacer UpwardsGravityScaleReplacement
    {
        get => GetField<float, ModifiedFloatReplacer>(nameof(UpwardsGravityScaleReplacement));
        private set => InitField(nameof(UpwardsGravityScaleReplacement), value);
    }

    /// <summary>
    ///     What to replace the Falling's downwards gravity scale with.
    /// </summary>
    [Export]
    public ModifiedFloatReplacer DownwardsGravityScaleReplacement
    {
        get => GetField<float, ModifiedFloatReplacer>(nameof(DownwardsGravityScaleReplacement));
        private set => InitField(nameof(DownwardsGravityScaleReplacement), value);
    }

    /// <summary>
    ///     The max drag velocity: what to replace the Falling's max fall velocity with.
    /// </summary>
    [Export]
    public ModifiedFloatReplacer MaxFallVelocityReplacement
    {
        get => GetField<float, ModifiedFloatReplacer>(nameof(MaxFallVelocityReplacement));
        private set => InitField(nameof(MaxFallVelocityReplacement), value);
    }

    /// <summary>
    ///     What to replace the Falling's time scale with when hitting a ceiling.
    /// </summary>
    [Export]
    public ModifiedFloatReplacer CeilingHitStopTimeScaleReplacement
    {
        get => GetField<float, ModifiedFloatReplacer>(nameof(CeilingHitStopTimeScaleReplacement));
        private set => InitField(nameof(CeilingHitStopTimeScaleReplacement), value);
    }

    /// <summary>
    ///     What to replace the Falling's deceleration time with, when decelerating to the max fall/drag
    ///     velocity.
    /// </summary>
    [Export]
    public ModifiedFloatReplacer DecelToMaxVelocityTimePer100VelocityReplacement
    {
        get => GetField<float, ModifiedFloatReplacer>(
            nameof(DecelToMaxVelocityTimePer100VelocityReplacement));
        private set => InitField(nameof(DecelToMaxVelocityTimePer100VelocityReplacement), value);
    }

    /// <summary>
    ///     The y velocity that the Falling needs to meet or exceed, for wall dragging to start
    ///     (given other conditions are met).
    /// </summary>
    [Export]
    public float VelocityDragThreshold
    {
        get => GetValue<float>(nameof(VelocityDragThreshold));
        private set => InitValue(nameof(VelocityDragThreshold), value);
    }

    public void AddModifiersTo(FallingData values)
    {
        values.AddModifierTo(nameof(FallingData.UpwardsGravityScale),
            UpwardsGravityScaleReplacement);
        values.AddModifierTo(nameof(FallingData.DownwardsGravityScale),
            DownwardsGravityScaleReplacement);
        values.AddModifierTo(nameof(FallingData.MaxFallVelocity), MaxFallVelocityReplacement);
        values.AddModifierTo(nameof(FallingData.CeilingHitStopTimeScale),
            CeilingHitStopTimeScaleReplacement);
        values.AddModifierTo(
            nameof(FallingData.DecelToMaxVelocityTimePer100Velocity),
            DecelToMaxVelocityTimePer100VelocityReplacement);
    }

    public void RemoveModifiersFrom(FallingData values)
    {
        values.RemoveModifierFrom(nameof(FallingData.UpwardsGravityScale),
            UpwardsGravityScaleReplacement);
        values.RemoveModifierFrom(nameof(FallingData.DownwardsGravityScale),
            DownwardsGravityScaleReplacement);
        values.RemoveModifierFrom(nameof(FallingData.MaxFallVelocity), MaxFallVelocityReplacement);
        values.RemoveModifierFrom(nameof(FallingData.CeilingHitStopTimeScale),
            CeilingHitStopTimeScaleReplacement);
        values.RemoveModifierFrom(
            nameof(FallingData.DecelToMaxVelocityTimePer100Velocity),
            DecelToMaxVelocityTimePer100VelocityReplacement);
    }
}