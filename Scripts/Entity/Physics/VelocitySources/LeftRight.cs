using System;
using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

/// <summary>
/// A source of velocity representing general horizontal movement.
/// Meant for walking, horizontal jump movement, etc.
/// </summary>
public partial class LeftRight : VelocitySource
{
    /// <summary>
    /// A signal for when this LeftRight's velocity goes from not -x to -x.
    /// </summary>
    [Signal]
    public delegate void GoingLeftEventHandler();

    /// <summary>
    /// A signal for when this LeftRight's velocity goes from not +x to +x.
    /// </summary>
    [Signal]
    public delegate void GoingRightEventHandler();

    /// <summary>
    /// A signal for when this LeftRight intends to change its velocity.
    /// </summary>
    [Signal]
    public delegate void IntendedSpeedChangeEventHandler(float newSpeed);

    /// <summary>
    /// A signal for when this LeftRight goes from nonzero to zero.
    /// </summary>
    [Signal]
    public delegate void StopMovingEventHandler();

    private const int SPEED_SCALE_ROUNDING_DIGITS = 4;

    private Tween _accelerationTween;
    private LeftRightData _air;

    private LeftRightData _ground;
    private float _intendedSpeedScale;

    private bool _isOnGround;

    private Vector2 _oldBaseVelocity;

    /// <summary>
    /// The data used to determine this LeftRight's movement speed, etc. when on the ground.
    /// </summary>
    [Export]
    public LeftRightData Ground
    {
        get => _ground;
        private set
        {
            _ground = value;
            GroundUpdated();
        }
    }

    /// <summary>
    /// The data used to determine this LeftRight's movement speed, etc. when in the air.
    /// </summary>
    [Export]
    public LeftRightData Air
    {
        get => _air;
        private set
        {
            _air = value;
            AirUpdated();
        }
    }

    /// <summary>
    /// Whether this LeftRight should act as if it's on the ground (true) or in the air (false).
    /// </summary>
    public bool IsOnGround
    {
        get => _isOnGround;
        set
        {
            if (IsOnGround == value) return;

            _isOnGround = value;
            UpdateSpeedToIntended();
        }
    }

    /// <summary>
    /// The current scale factor of this LeftRight's horizontal velocity,
    /// when scaling the current base speed.
    /// </summary>
    public float CurrentSpeedScale => CurrentSpeed / CurrentParams.BaseSpeed;

    /// <summary>
    /// The current horizontal velocity of this LeftRight.
    /// </summary>
    public float CurrentSpeed => BaseVelocity.X;

    /// <summary>
    /// The horizontal velocity scale that this LeftRight intends to accelerate towards.
    /// Setting this accelerates this LeftRight to the given horizontal velocity scale over
    /// <code>GetAccelTime(value)</code> seconds.
    /// </summary>
    public float IntendedSpeedScale
    {
        get => _intendedSpeedScale;
        set => SetIntendedSpeedScale(value, GetAccelTime(value));
    }

    /// <summary>
    /// The horizontal velocity that this LeftRight intends to accelerate towards.
    /// Setting this accelerates this LeftRight to the given horizontal velocity over
    /// <code>GetAccelTime(value)</code> seconds.
    /// </summary>
    public float IntendedSpeed
    {
        get => IntendedSpeedScale * CurrentParams.BaseSpeed;
        set => IntendedSpeedScale = value / CurrentParams.BaseSpeed;
    }

    private LeftRightData CurrentParams => IsOnGround ? Ground : Air;

    public override void _Ready()
    {
        Ground.ModifiersUpdated += GroundUpdated;
        Air.ModifiersUpdated += AirUpdated;
    }

    private void GroundUpdated()
    {
        if (IsNodeReady() && IsOnGround) UpdateSpeedToIntended();
    }

    private void AirUpdated()
    {
        if (IsNodeReady() && !IsOnGround) UpdateSpeedToIntended();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!Enabled) return;
        base._PhysicsProcess(delta);

        switch (_oldBaseVelocity.X, BaseVelocity.X)
        {
            case (<= 0f, > 0):
                EmitSignal(SignalName.GoingRight);
                break;
            case (>= 0f, < 0):
                EmitSignal(SignalName.GoingLeft);
                break;
            case (not 0, 0) when IntendedSpeed == 0:
                EmitSignal(SignalName.StopMoving);
                break;
        }

        _oldBaseVelocity = BaseVelocity;
    }

    /// <summary>
    /// Accelerates this LeftRight to a velocity.
    /// </summary>
    /// <param name="speed">The horizontal velocity to accelerate towards</param>
    /// <param name="time">The amount of time to take to accelerate to that velocity</param>
    /// <returns>The tween for controlling the acceleration.</returns>
    public (Tween, PropertyTweener) SetIntendedSpeed(float speed, float time)
    {
        return SetIntendedSpeedScale(speed / CurrentParams.BaseSpeed, time);
    }

    /// <summary>
    /// Accelerates this LeftRight to a velocity, based on a scale of the current base speed.
    /// </summary>
    /// <param name="speedScale">The horizontal velocity scale to accelerate towards</param>
    /// <param name="time">The amount of time to take to accelerate to that velocity</param>
    /// <returns>The tween for controlling the acceleration.</returns>
    public (Tween, PropertyTweener) SetIntendedSpeedScale(float speedScale, float time)
    {
        float newIntendedSpeedScale = MathF.Round(speedScale, SPEED_SCALE_ROUNDING_DIGITS);
        bool speedScaleChange = !Mathf.IsEqualApprox(_intendedSpeedScale, newIntendedSpeedScale);
        _intendedSpeedScale = newIntendedSpeedScale;

        (_accelerationTween, PropertyTweener t) =
            SmoothlySetBaseVelocityX(_intendedSpeedScale * CurrentParams.BaseSpeed, time);

        if (speedScaleChange) EmitSignal(SignalName.IntendedSpeedChange, IntendedSpeedScale);

        return (_accelerationTween, t);
    }

    private void UpdateSpeedToIntended()
    {
        IntendedSpeedScale = _intendedSpeedScale;
    }

    /// <summary>
    /// Calculates the default amount of time it'd take for this LeftRight to accelerate to
    /// the given horizontal velocity scale.
    /// </summary>
    /// <param name="toSpeedScale">The scale of the horizontal velocity
    /// that would've been accelerated to</param>
    /// <returns>The calculated time, in seconds</returns>
    public float GetAccelTime(float toSpeedScale)
    {
        float speedScaleDelta = Mathf.Abs(CurrentSpeedScale - toSpeedScale);
        float speedScaleAccelMultiplier = speedScaleDelta;
        if (speedScaleDelta > 1)
            speedScaleAccelMultiplier = Mathf.Pow(speedScaleAccelMultiplier,
                CurrentParams.SpeedScaleHighDeltaPower);

        return speedScaleAccelMultiplier * GetAccelBaseTime(toSpeedScale);
    }

    private float GetAccelBaseTime(float toSpeedScale)
    {
        bool decelerating = toSpeedScale == 0
                            || (Mathf.Abs(toSpeedScale) < Mathf.Abs(CurrentSpeedScale)
                                && Mathf.Sign(toSpeedScale) == Mathf.Sign(CurrentSpeedScale));
        return decelerating ? CurrentParams.DecelBaseTime : CurrentParams.AccelBaseTime;
    }
}