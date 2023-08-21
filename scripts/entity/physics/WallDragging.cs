using Godot;

namespace Learning.scripts.entity.physics; 

public partial class WallDragging : Node {
    [Export] private Falling Falling { get; set; }
    [Export] public WallDraggingData Wall {
        get => _wall;
        set {
            _wall = value;
            if (IsDragging) {
                Falling.FallData = Wall.DraggingData;
            }
            DraggingCheck();
        }
    }
    
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
    private WallDraggingData _wall;

    [Signal]
    public delegate void StartedDraggingEventHandler();

    [Signal]
    public delegate void StoppedDraggingEventHandler();

    public override void _Ready() {
        _originalData = Falling.FallData;
    }

    public override void _PhysicsProcess(double delta) {
        DraggingCheck();
    }

    public void DraggingCheck() {
        IsDragging = ValidWallTouching && Falling.VelocityAfterTransition.Y >= Wall.VelocityDragThreshold;
    }

    public bool IsOnValidWall(CharacterBody2D physics) {
        return !physics.IsOnFloor() && physics.IsOnWall() && physics.GetWallNormal().Y == 0;
    }
}