using Godot;

namespace Learning.scripts.entity.physics; 

public partial class WallDragging : Node {
    [Export] private Falling Falling { get; set; }
    [Export] private WallDraggingData Wall { get; set; }
    
    public bool IsDragging {
        get => _isDragging;
        set {
            if (value == IsDragging) return;

            _isDragging = value;
            if (_isDragging) {
                Falling.FallData = Wall.DraggingData;
                EmitSignal(SignalName.StartedDragging);
            } else {
                Falling.FallData = _originalData;
                EmitSignal(SignalName.StoppedDragging);
            }
        }
    }
    
    public bool ValidWallTouching { get; set; }

    private bool _isDragging;
    private FallingData _originalData;

    [Signal]
    public delegate void StartedDraggingEventHandler();

    [Signal]
    public delegate void StoppedDraggingEventHandler();

    public override void _Ready() {
        _originalData = Falling.FallData;
    }

    public override void _PhysicsProcess(double delta) {
        if (ValidWallTouching && Falling.VelocityAfterTransition.Y > Wall.VelocityDragThreshold) {
            IsDragging = true;
        }
    }

    public bool IsOnValidWall(KinematicComp physics) {
        return !physics.IsOnFloor() && physics.IsOnWall() && physics.GetWallNormal().Y == 0;
    }
}