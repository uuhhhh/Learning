using System;
using Godot;

namespace Learning.Scripts.Entity.Physics; 

public partial class LeftRight : VelocitySource {
    [Export] public LeftRightData Ground {
        get => _ground;
        set {
            _ground = value;
            if (IsNodeReady() && _isOnGround) {
                UpdateSpeed();
            }
        }
    }

    [Export] public LeftRightData Air {
        get => _air;
        set {
            _air = value;
            if (IsNodeReady() && !_isOnGround) {
                UpdateSpeed();
            }
        }
    }

    private const int SPEED_SCALE_ROUNDING_DIGITS = 4;
    
    public bool IsOnGround {
        get => _isOnGround;
        set {
            _isOnGround = value;
            UpdateSpeed();
        }
    }
    
    public float CurrentSpeedScale {
        get => CurrentSpeed / Params.BaseSpeed;
        private set => CurrentSpeed = value * Params.BaseSpeed;
    }
    
    public float CurrentSpeed {
        get => BaseVelocity.X;
        private set => (_accelerationTween, _) = SmoothlySetBaseVelocityX(value, GetAccelTime());
    }

    public float IntendedSpeedScale {
        get => _intendedSpeedScale;
        set {
            _intendedSpeedScale = MathF.Round(value, SPEED_SCALE_ROUNDING_DIGITS);
            UpdateSpeed();
        }
    }

    public float IntendedSpeed {
        get => IntendedSpeedScale * Params.BaseSpeed;
        set => IntendedSpeedScale = value / Params.BaseSpeed;
    }

    private LeftRightData Params => IsOnGround ? Ground : Air;

    private LeftRightData _ground;
    private LeftRightData _air;

    private bool _isOnGround;
    private float _intendedSpeedScale;

    private Vector2 _oldBaseVelocity;

    private Tween _accelerationTween;
    
    [Signal]
    public delegate void GoingRightEventHandler();
    [Signal]
    public delegate void GoingLeftEventHandler();
    [Signal]
    public delegate void StopMovingEventHandler();

    [Signal]
    public delegate void IntendedSpeedUpdateEventHandler(float newSpeed);
    
    public override void _PhysicsProcess(double delta) {
        if (!Enabled) return;
        base._PhysicsProcess(delta);
        
        switch (_oldBaseVelocity.X, BaseVelocity.X) {
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

    private void UpdateSpeed() {
        CurrentSpeedScale = IntendedSpeedScale;
        EmitSignal(SignalName.IntendedSpeedUpdate, IntendedSpeedScale);
    }

    private float GetAccelTime() {
        float speedScaleDelta = Mathf.Abs(CurrentSpeedScale - IntendedSpeedScale);
        float speedScaleAccelMultiplier = speedScaleDelta;
        if (speedScaleDelta > 1) {
            speedScaleAccelMultiplier = Mathf.Pow(speedScaleAccelMultiplier, Params.SpeedScaleHighDeltaPower);
        }
        
        return speedScaleAccelMultiplier * GetAccelBaseTime();
    }

    private float GetAccelBaseTime() {
        bool decelerating = IntendedSpeedScale == 0
                            || (Mathf.Abs(IntendedSpeedScale) < Mathf.Abs(CurrentSpeedScale)
                                && Mathf.Sign(IntendedSpeedScale) == Mathf.Sign(CurrentSpeedScale));
        return decelerating ? Params.DecelBaseTime : Params.AccelBaseTime;
    }
}