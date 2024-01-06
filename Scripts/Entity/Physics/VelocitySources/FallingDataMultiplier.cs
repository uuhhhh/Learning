using Godot;
using Learning.Scripts.Values.Groups;
using Learning.Scripts.Values.Modifiers;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

/// <summary>
///     A group of modifiers that can be applied to a FallingData.
/// </summary>
[GlobalClass]
public partial class FallingDataMultiplier : ModifierResource<FallingData>
{
    /// <summary>
    ///     A scale factor for a Falling's UpwardsGravityScale.
    /// </summary>
    [Export]
    public FloatMultiplier UpwardsGravityScaleMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(UpwardsGravityScaleMultiplier));
        private set => AddModifierToBeAdded(nameof(UpwardsGravityScaleMultiplier),
            nameof(FallingData.UpwardsGravityScale), value);
    }

    /// <summary>
    ///     A scale factor for a Falling's DownwardsGravityScale.
    /// </summary>
    [Export]
    public FloatMultiplier DownwardsGravityScaleMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(DownwardsGravityScaleMultiplier));
        private set => AddModifierToBeAdded(nameof(DownwardsGravityScaleMultiplier),
            nameof(FallingData.DownwardsGravityScale), value);
    }

    /// <summary>
    ///     A scale factor for a Falling's MaxFallVelocity.
    /// </summary>
    [Export]
    public FloatMultiplier MaxFallVelocityMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(MaxFallVelocityMultiplier));
        private set => AddModifierToBeAdded(nameof(MaxFallVelocityMultiplier),
            nameof(FallingData.MaxFallVelocity), value);
    }

    /// <summary>
    ///     A scale factor for a Falling's CeilingHitStopTimeScale.
    /// </summary>
    [Export]
    public FloatMultiplier CeilingHitStopTimeScaleMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(CeilingHitStopTimeScaleMultiplier));
        private set => AddModifierToBeAdded(nameof(CeilingHitStopTimeScaleMultiplier),
            nameof(FallingData.CeilingHitStopTimeScale), value);
    }

    /// <summary>
    ///     A scale factor for a Falling's DecelToMaxVelocityTimePer100Velocity.
    /// </summary>
    [Export]
    public FloatMultiplier DecelToMaxVelocityTimePer100VelocityMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(DecelToMaxVelocityTimePer100VelocityMultiplier));
        private set => AddModifierToBeAdded(nameof(DecelToMaxVelocityTimePer100VelocityMultiplier),
            nameof(FallingData.DecelToMaxVelocityTimePer100Velocity), value);
    }
}