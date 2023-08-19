using Godot;

namespace Learning.scripts.entity.physics; 

public partial class WallDragging : Node, IKinematicCompLinkable {
    [Export] public bool DoNotLink { get; set; }
    [Export] private Falling AirMovement { get; set; }
    [Export] private FallingData DraggingData { get; set; }
    [Export] private float VelocityThreshold { get; set; }
    
    public bool IsDragging {
        get => _isDragging;
        set {
            if (value == IsDragging) return;

            _isDragging = value;
            if (_isDragging) {
                AirMovement.FallData = DraggingData;
                EmitSignal(SignalName.StartedDragging);
            } else {
                AirMovement.FallData = _originalData;
                EmitSignal(SignalName.StoppedDragging);
            }
        }
    }
    
    public Location CurrentLocation { get; set; } = Location.None;

    private bool _isDragging;
    private FallingData _originalData;

    [Signal]
    public delegate void StartedDraggingEventHandler();

    [Signal]
    public delegate void StoppedDraggingEventHandler();

    public override void _Ready() {
        _originalData = AirMovement.FallData;
    }

    public override void _PhysicsProcess(double delta) {
        if (CurrentLocation == Location.WallNonGround && AirMovement.Velocity.Y > VelocityThreshold) {
            IsDragging = true;
        }
    }

    public void Link(KinematicComp2 physics) {
        physics.BecomeOnFloor += _ => {
            CurrentLocation = Location.Ground;
            IsDragging = false;
        };
        physics.BecomeOffFloor += state =>
            CurrentLocation = state.IsOnWall() ? Location.WallNonGround : Location.Air;
        physics.BecomeOnWall += state => {
            if (!state.IsOnFloor()) {
                CurrentLocation = Location.WallNonGround;
            }
        };
        physics.BecomeOffWall += state => {
            if (!state.IsOnFloor()) {
                CurrentLocation = Location.Air;
            }
            IsDragging = false;
        };
    }
}