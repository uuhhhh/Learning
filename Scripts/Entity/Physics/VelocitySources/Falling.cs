using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

public partial class Falling : VelocitySource {
    [Export] public FallingData FallData {
        get => _fallData;
        set {
            FallingData oldFallData = _fallData;
            _fallData = value;

            if (_decelerateToMaxVelocityTween != null && _decelerateToMaxVelocityTween.IsValid()) {
                if (BaseVelocity.Y > FallData.MaxVelocity) {
                    float newDuration
                        = _originalDecelerateToMaxVelocityTime / oldFallData.DecelToMaxVelocityTimePerVelocity *
                          FallData.DecelToMaxVelocityTimePerVelocity;
                    _decelerateToMaxVelocityTween.SetSpeedScale(1 / newDuration);
                } else {
                    _decelerateToMaxVelocityTween.Kill();
                }
            }
            if (_ceilingHitStopTween != null && _ceilingHitStopTween.IsValid()) {
                float newDuration = _originalCeilingHitStopTime * FallData.CeilingHitStopTimeScale;
                _ceilingHitStopTween.SetSpeedScale(1 / newDuration);
            }
        }
    }
    
    public bool IsFalling {
        get => _isFalling;
        set {
            if (_isFalling == value) return;
            
            _isFalling = value;
            if (_isFalling) {
                EmitSignal(SignalName.StartFalling);
            } else {
                BaseVelocity = Vector2.Zero;
                EmitSignal(SignalName.StopFalling);
            }
        }
    }
    
    public float GravityScale => Velocity.Y < 0 ? FallData.UpwardsGravityScale : FallData.DownwardsGravityScale;
    
    public readonly float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private FallingData _fallData;
    
    private bool _isFalling;

    private float _originalCeilingHitStopTime;
    private Tween _ceilingHitStopTween;

    private float _originalDecelerateToMaxVelocityTime;
    private Tween _decelerateToMaxVelocityTween;
    
    [Signal]
    public delegate void StartFallingEventHandler();

    [Signal]
    public delegate void StopFallingEventHandler();
    
    public override void _PhysicsProcess(double delta) {
        if (!Enabled) return;
        base._PhysicsProcess(delta);
        if (!IsFalling) return;

        if (BaseVelocity.Y > FallData.MaxVelocity) {
            if (_decelerateToMaxVelocityTween != null && _decelerateToMaxVelocityTween.IsValid()) return;

            _originalDecelerateToMaxVelocityTime
                = (BaseVelocity.Y - FallData.MaxVelocity) * FallData.DecelToMaxVelocityTimePerVelocity;
            (_decelerateToMaxVelocityTween, PropertyTweener t)
                = SmoothlySetBaseVelocityY(FallData.MaxVelocity, _originalDecelerateToMaxVelocityTime);
            t.SetEase(Tween.EaseType.Out);
            t.SetTrans(Tween.TransitionType.Quad);
        } else {
            Vector2 velocity = BaseVelocity;
            velocity.Y += Gravity * GravityScale * (float) delta;
            velocity.Y = Mathf.Min(velocity.Y, FallData.MaxVelocity);
            BaseVelocity = velocity;
        }
    }

    internal void CeilingHitStop() {
        _originalCeilingHitStopTime = Mathf.Abs(BaseVelocity.Y) / (Gravity * GravityScale);
        (_ceilingHitStopTween, _) =
            SmoothlySetBaseVelocityY(0, _originalCeilingHitStopTime * FallData.CeilingHitStopTimeScale);
    }
}