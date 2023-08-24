﻿using System;
using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

public partial class LeftRight : VelocitySource {
    [Export] public LeftRightData Ground {
        get => _ground;
        set {
            _ground = value;
            if (IsNodeReady() && _isOnGround) {
                UpdateSpeedToIntended();
            }
        }
    }

    [Export] public LeftRightData Air {
        get => _air;
        set {
            _air = value;
            if (IsNodeReady() && !_isOnGround) {
                UpdateSpeedToIntended();
            }
        }
    }

    private const int SPEED_SCALE_ROUNDING_DIGITS = 4;
    
    public bool IsOnGround {
        get => _isOnGround;
        set {
            _isOnGround = value;
            UpdateSpeedToIntended();
        }
    }
    
    public float CurrentSpeedScale => CurrentSpeed / CurrentParams.BaseSpeed;
    
    public float CurrentSpeed => BaseVelocity.X;

    public float IntendedSpeedScale {
        get => _intendedSpeedScale;
        set => SetIntendedSpeedScale(value, GetAccelTime(value));
    }

    public float IntendedSpeed {
        get => IntendedSpeedScale * CurrentParams.BaseSpeed;
        set => IntendedSpeedScale = value / CurrentParams.BaseSpeed;
    }

    private LeftRightData CurrentParams => IsOnGround ? Ground : Air;

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

    public void SetIntendedSpeed(float speed, float time) {
        SetIntendedSpeedScale(speed / CurrentParams.BaseSpeed, time);
    }

    public void SetIntendedSpeedScale(float speedScale, float time) {
        _intendedSpeedScale = MathF.Round(speedScale, SPEED_SCALE_ROUNDING_DIGITS);
        (_accelerationTween, _) =
            SmoothlySetBaseVelocityX(_intendedSpeedScale * CurrentParams.BaseSpeed, time);
        
        EmitSignal(SignalName.IntendedSpeedUpdate, IntendedSpeedScale);
    }

    private void UpdateSpeedToIntended() {
        IntendedSpeedScale = _intendedSpeedScale;
    }

    private float GetAccelTime(float toSpeedScale) {
        float speedScaleDelta = Mathf.Abs(CurrentSpeedScale - toSpeedScale);
        float speedScaleAccelMultiplier = speedScaleDelta;
        if (speedScaleDelta > 1) {
            speedScaleAccelMultiplier = Mathf.Pow(speedScaleAccelMultiplier, CurrentParams.SpeedScaleHighDeltaPower);
        }
        
        return speedScaleAccelMultiplier * GetAccelBaseTime(toSpeedScale);
    }

    private float GetAccelBaseTime(float toSpeedScale) {
        bool decelerating = toSpeedScale == 0
                            || (Mathf.Abs(toSpeedScale) < Mathf.Abs(CurrentSpeedScale)
                                && Mathf.Sign(toSpeedScale) == Mathf.Sign(CurrentSpeedScale));
        return decelerating ? CurrentParams.DecelBaseTime : CurrentParams.AccelBaseTime;
    }
}