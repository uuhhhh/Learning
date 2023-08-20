using System.Numerics;
using Godot;
using Vector2 = Godot.Vector2;

namespace Learning.scripts.entity.physics; 

public partial class Falling : VelocitySource, IKinematicCompLinkable {
    [Export] public bool DoNotLink { get; set; }
    [Export] public FallingData FallData { get; set; }
    
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
    
    public readonly float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public float GravityScale => Velocity.Y < 0 ? FallData.UpwardsGravityScale : FallData.DownwardsGravityScale;

    private bool _isFalling;
    
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

    public void DefaultOnBecomeOnFloor(KinematicComp2 physics) {
        IsFalling = false;
    }

    public void DefaultOnBecomeOffFloor(KinematicComp2 physics) {
        IsFalling = true;
    }

    public void DefaultOnBecomeOnCeiling(KinematicComp2 physics) {
        float originalTimeToStop = Mathf.Abs(BaseVelocity.Y) / (Gravity * GravityScale);
        SmoothlySetBaseVelocityY(0, originalTimeToStop * FallData.CeilingHitStopTimeScale);
    }
}