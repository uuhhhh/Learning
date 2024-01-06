using System;
using Godot;
using Learning.Scripts.Values.Groups;

namespace Learning.Scripts.Entity.Physics.Intermediate;

/// <summary>
///     Data used by a Jumping to determine jump velocity, jump acceleration, etc.
/// </summary>
[GlobalClass]
public partial class JumpingData : ResourceWithModifiers
{
    public const int UnlimitedJumps = -1;

    /// <summary>
    ///     The base number of jumps. 0 means the entity currently cannot jump anymore.
    /// </summary>
    [Export]
    public int NumJumps
    {
        get => GetValue<int>(nameof(NumJumps));
        private set => InitValue(nameof(NumJumps), value);
    }

    /// <summary>
    ///     The velocity the Jumping goes to when performing a jump.
    /// </summary>
    [Export]
    public Vector2 Velocity
    {
        get => GetValue<Vector2>(nameof(Velocity));
        private set => InitValue(nameof(Velocity), value);
    }

    /// <summary>
    ///     The time, in seconds, for the Jumping's x velocity to transition to the jump velocity
    ///     when performing a jump.
    /// </summary>
    [Export]
    public float AccelTimeX
    {
        get => GetValue<float>(nameof(AccelTimeX));
        private set => InitValue(nameof(AccelTimeX), value);
    }

    /// <summary>
    ///     The time, in seconds, for the Jumping's y velocity to transition to the jump velocity
    ///     when performing a jump.
    /// </summary>
    [Export]
    public float AccelTimeY
    {
        get => GetValue<float>(nameof(AccelTimeY));
        private set => InitValue(nameof(AccelTimeY), value);
    }

    /// <summary>
    ///     The velocity the Jumping goes to when cancelling a jump.
    /// </summary>
    [Export]
    public float CancelVelocity
    {
        get => GetValue<float>(nameof(CancelVelocity));
        private set => InitValue(nameof(CancelVelocity), value);
    }

    /// <summary>
    ///     The time, in seconds, for the Jumping's velocity to transition to the cancel velocity
    ///     when cancelling a jump.
    /// </summary>
    [Export]
    public float CancelAccelTime
    {
        get => GetValue<float>(nameof(CancelAccelTime));
        private set => InitValue(nameof(CancelAccelTime), value);
    }

    /// <returns>
    ///     The minimum number of jumps between the two;
    ///     UnlimitedJumps is always higher.
    /// </returns>
    public static int MinNumJumps(int numJumps1, int numJumps2)
    {
        return (numJumps1, numJumps2) switch
        {
            (_, UnlimitedJumps) => numJumps1,
            (UnlimitedJumps, _) => numJumps2,
            (_, _) => Math.Min(numJumps1, numJumps2)
        };
    }
}