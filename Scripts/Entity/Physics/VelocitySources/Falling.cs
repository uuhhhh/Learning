using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

public partial class Falling : VelocitySource {
    [Export] public FallingData FallData {
        get => _fallData;
        private set {
            _fallData = value;
            FallDataUpdated();
        }
    }
    
    public bool IsFalling {
        get => _isFalling;
        set {
            if (IsFalling == value) return;
            
            _isFalling = value;
            if (IsFalling) {
                EmitSignal(SignalName.StartFalling);
            } else {
                BaseVelocity = Vector2.Zero;
                EmitSignal(SignalName.StopFalling);
            }
        }
    }
    
    public float GravityScale => Velocity.Y < 0 ? FallData.UpwardsGravityScale : FallData.DownwardsGravityScale;

    public bool StoppingDueToCeilingHit => _ceilingHitStopTween is not null && _ceilingHitStopTween.IsValid();

    public bool DeceleratingToMaxFallVelocity =>
        _decelerateToMaxVelocityTween is not null && _decelerateToMaxVelocityTween.IsValid();
    
    public readonly float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private FallingData _fallData;
    
    private bool _isFalling;

    private float _originalCeilingHitStopTime;
    private Tween _ceilingHitStopTween;

    private float _originalDecelerateVelocityDelta;
    private Tween _decelerateToMaxVelocityTween;
    
    [Signal]
    public delegate void StartFallingEventHandler();

    [Signal]
    public delegate void StopFallingEventHandler();

    public override void _Ready() {
        FallData.ModifiersUpdated += FallDataUpdated;
    }

    private void FallDataUpdated() {
        if (IsNodeReady()) {
            UpdateDecelerationToMaxVelocity();
            UpdateCeilingHitStop();
        }
    }

    private void UpdateDecelerationToMaxVelocity() {
        if (_decelerateToMaxVelocityTween != null && _decelerateToMaxVelocityTween.IsValid()) {
            if (BaseVelocity.Y > FallData.MaxFallVelocity) {
                float newDuration = _originalDecelerateVelocityDelta * FallData.DecelToMaxVelocityTimePerVelocity;
                _decelerateToMaxVelocityTween.SetSpeedScale(1 / newDuration);
            } else {
                _decelerateToMaxVelocityTween.Kill();
            }
        }
    }

    private void UpdateCeilingHitStop() {
        if (_ceilingHitStopTween != null && _ceilingHitStopTween.IsValid()) {
            float newDuration = _originalCeilingHitStopTime * FallData.CeilingHitStopTimeScale;
            _ceilingHitStopTween.SetSpeedScale(1 / newDuration);
        }
    }
    
    public override void _PhysicsProcess(double delta) {
        if (!Enabled) return;
        base._PhysicsProcess(delta);
        if (!IsFalling) return;

        if (BaseVelocity.Y > FallData.MaxFallVelocity) {
            DecelerateToMaxFallVelocity();
        } else {
            AccelerateFromGravity(delta);
        }
    }

    private void DecelerateToMaxFallVelocity() {
        if (_decelerateToMaxVelocityTween != null && _decelerateToMaxVelocityTween.IsValid()) return;
            
        _originalDecelerateVelocityDelta = BaseVelocity.Y - FallData.MaxFallVelocity;
        float decelerateTime = _originalDecelerateVelocityDelta * FallData.DecelToMaxVelocityTimePerVelocity;
        (_decelerateToMaxVelocityTween, PropertyTweener t)
            = SmoothlySetBaseVelocityY(FallData.MaxFallVelocity, decelerateTime);
        t.SetEase(Tween.EaseType.Out);
        t.SetTrans(Tween.TransitionType.Quad);
    }

    private void AccelerateFromGravity(double delta) {
        Vector2 velocity = BaseVelocity;
        velocity.Y += Gravity * GravityScale * (float) delta;
        velocity.Y = Mathf.Min(velocity.Y, FallData.MaxFallVelocity);
        BaseVelocity = velocity;
    }

    internal void CeilingHitStop() {
        _originalCeilingHitStopTime = Mathf.Abs(BaseVelocity.Y) / (Gravity * GravityScale);
        (_ceilingHitStopTween, _) =
            SmoothlySetBaseVelocityY(0, _originalCeilingHitStopTime * FallData.CeilingHitStopTimeScale);
    }
}