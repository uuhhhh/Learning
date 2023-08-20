using System;
using Godot;

namespace Learning.scripts.entity.physics; 

public partial class LeftRight : VelocitySource, IKinematicCompLinkable {
    [Export] public bool DoNotLink { get; set; }
    [Export] private LeftRightData Ground { get; set; }
    [Export] private LeftRightData Air { get; set; }

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

    public void DefaultOnBecomeOnFloor(KinematicComp2 physics) {
        IsOnGround = true;
    }

    public void DefaultOnBecomeOffFloor(KinematicComp2 physics) {
        IsOnGround = false;
    }
}