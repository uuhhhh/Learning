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

    public bool ValidWallTouching {
        get => _isValidWallTouching;
        set {
            if (value == _isValidWallTouching) return;

            _isValidWallTouching = value;
            EmitSignal(ValidWallTouching ? SignalName.StartedValidWallTouching : SignalName.StoppedValidWallTouching);
        }
    }
    
    private bool _isDragging;
    private bool _isValidWallTouching;
    private FallingData _originalData;
    private WallDraggingData _wall;

    [Signal]
    public delegate void StartedDraggingEventHandler();

    [Signal]
    public delegate void StoppedDraggingEventHandler();
    
    [Signal]
    public delegate void StartedValidWallTouchingEventHandler();
    
    [Signal]
    public delegate void StoppedValidWallTouchingEventHandler();

    public override void _Ready() {
        _originalData = Falling.FallData;
    }

    public override void _PhysicsProcess(double delta) {
        DraggingCheck();
    }

    public void DraggingCheck() {
        IsDragging = ValidWallTouching && Falling.VelocityAfterTransition.Y >= Wall.VelocityDragThreshold;
    }
}