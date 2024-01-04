using Godot;
using Learning.Scripts.Values.Groups;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

/// <summary>
/// Data used by a LeftRight for determining its movement and acceleration speed.
/// </summary>
[GlobalClass]
public partial class LeftRightData : ResourceWithModifiers
{
    /// <summary>
    /// The base speed used by a LeftRight when moving, in units/second.
    /// </summary>
    [Export]
    public float BaseSpeed
    {
        get => GetValue<float>(nameof(BaseSpeed));
        private set => InitValue(nameof(BaseSpeed), value);
    }

    /// <summary>
    /// The base time it takes for a LeftRight to accelerate from 0 speed to BaseSpeed speed.
    /// </summary>
    [Export]
    public float AccelBaseTime
    {
        get => GetValue<float>(nameof(AccelBaseTime));
        private set => InitValue(nameof(AccelBaseTime), value);
    }

    /// <summary>
    /// The base time it takes for a LeftRight to accelerate from BaseSpeed speed to 0 speed.
    /// </summary>
    [Export]
    public float DecelBaseTime
    {
        get => GetValue<float>(nameof(DecelBaseTime));
        private set => InitValue(nameof(DecelBaseTime), value);
    }

    /// <summary>
    /// A power factor for when a LeftRight intends to accelerate an amount higher than BaseSpeed
    /// (e.g. accelerating from BaseSpeed to -BaseSpeed) (i.e., a high "speed scale delta").
    /// Set this to 1 for this to not affect the acceleration time. Set this to a lower value for
    /// the acceleration time to be dampened (e.g. closer to the base accel time).
    /// </summary>
    [Export]
    public float SpeedScaleHighDeltaPower
    {
        get => GetValue<float>(nameof(SpeedScaleHighDeltaPower));
        private set => InitValue(nameof(SpeedScaleHighDeltaPower), value);
    }
}