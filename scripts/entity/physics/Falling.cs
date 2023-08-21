using Godot;

namespace Learning.scripts.entity.physics; 

public partial class Falling : VelocitySource {
    [Export] public FallingData FallData {
        get => _fallData;
        set {
            _fallData = value;
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
    
    [Signal]
    public delegate void StartFallingEventHandler();

    [Signal]
    public delegate void StopFallingEventHandler();
    
    public override void _PhysicsProcess(double delta) {
        if (!Enabled) return;
        base._PhysicsProcess(delta);
        if (!IsFalling) return;

        Vector2 velocity = BaseVelocity;
        velocity.Y += Gravity * GravityScale * (float) delta;
        velocity.Y = Mathf.Min(velocity.Y, FallData.MaxVelocity);
        BaseVelocity = velocity;
    }

    public void CeilingHitStop() {
        _originalCeilingHitStopTime = Mathf.Abs(BaseVelocity.Y) / (Gravity * GravityScale);
        (_ceilingHitStopTween, _) =
            SmoothlySetBaseVelocityY(0, _originalCeilingHitStopTime * FallData.CeilingHitStopTimeScale);
    }
}