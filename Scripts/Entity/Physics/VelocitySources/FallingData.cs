using Godot;
using Learning.Scripts.Values.Groups;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

/// <summary>
///     Data used by a Falling to determine fall acceleration, max fall velocity, etc.
/// </summary>
[GlobalClass]
public partial class FallingData : ResourceWithModifiers
{
    /// <summary>
    ///     A scale factor for the gravity, when the Falling's y velocity is upwards.
    /// </summary>
    [Export]
    public float UpwardsGravityScale
    {
        get => GetValue<float>(nameof(UpwardsGravityScale));
        private set => InitValue(nameof(UpwardsGravityScale), value);
    }

    /// <summary>
    ///     A scale factor for the gravity, when the Falling's y velocity is zero or downwards.
    /// </summary>
    [Export]
    public float DownwardsGravityScale
    {
        get => GetValue<float>(nameof(DownwardsGravityScale));
        private set => InitValue(nameof(DownwardsGravityScale), value);
    }

    /// <summary>
    ///     The maximum downwards y velocity that the Falling can fall. If the Falling's current
    ///     downwards y velocity exceeds this, then the Falling will decelerate to this value.
    /// </summary>
    [Export]
    public float MaxFallVelocity
    {
        get => GetValue<float>(nameof(MaxFallVelocity));
        private set => InitValue(nameof(MaxFallVelocity), value);
    }

    /// <summary>
    ///     A scale factor for the time it takes for a Falling to decelerate to zero y velocity
    ///     upon hitting a ceiling.
    /// </summary>
    [Export]
    public float CeilingHitStopTimeScale
    {
        get => GetValue<float>(nameof(CeilingHitStopTimeScale));
        private set => InitValue(nameof(CeilingHitStopTimeScale), value);
    }

    /// <summary>
    ///     The time it takes for a Falling to decelerate to the max fall velocity from a y velocity
    ///     greater than the max fall velocity, per 100 * (y velocity - max fall velocity)
    /// </summary>
    [Export]
    public float DecelToMaxVelocityTimePer100Velocity
    {
        get => GetValue<float>(nameof(DecelToMaxVelocityTimePer100Velocity));
        private set => InitValue(nameof(DecelToMaxVelocityTimePer100Velocity), value);
    }

    /// <summary>
    ///     The time it takes for a Falling to decelerate to the max fall velocity from a y velocity
    ///     greater than the max fall velocity, per (y velocity - max fall velocity)
    /// </summary>
    public float DecelToMaxVelocityTimePerVelocity => DecelToMaxVelocityTimePer100Velocity / 100;
}