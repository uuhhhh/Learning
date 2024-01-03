using System;
using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

public partial class LeftRight : VelocitySource
{
    [Signal]
    public delegate void GoingLeftEventHandler();

    [Signal]
    public delegate void GoingRightEventHandler();

    [Signal]
    public delegate void IntendedSpeedChangeEventHandler(float newSpeed);

    [Signal]
    public delegate void StopMovingEventHandler();

    private const int SPEED_SCALE_ROUNDING_DIGITS = 4;

    private Tween _accelerationTween;
    private LeftRightData _air;

    private LeftRightData _ground;
    private float _intendedSpeedScale;

    private bool _isOnGround;

    private Vector2 _oldBaseVelocity;

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

    public float CurrentSpeedScale => CurrentSpeed / CurrentParams.BaseSpeed;

    public float CurrentSpeed => BaseVelocity.X;

    public float IntendedSpeedScale
    {
        get => _intendedSpeedScale;
        set => SetIntendedSpeedScale(value, GetAccelTime(value));
    }

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

    public (Tween, PropertyTweener) SetIntendedSpeed(float speed, float time)
    {
        return SetIntendedSpeedScale(speed / CurrentParams.BaseSpeed, time);
    }

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