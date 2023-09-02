using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

public partial class WallDragging : Node {
    [Export] private bool Enabled { get; set; } = true;
    [Export] internal Falling Falling { get; set; }
    [Export] public WallDraggingData Wall {
        get => _wall;
        private set {
            _wall = value;
            WallDataUpdated();
        }
    }
    [Export] private bool DraggingEnabled { get; set; } = true;
    [Export] private bool ValidWallTouchingEnabled { get; set; } = true;
    
    public bool IsDragging {
        get => Enabled && DraggingEnabled && _isDragging;
        private set {
            if (!(Enabled && DraggingEnabled) || value == IsDragging) return;

            _isDragging = value;
            if (IsDragging) {
                Falling.FallData.AddModifier(Wall);
                EmitSignal(SignalName.StartedDragging);
            } else {
                Falling.FallData.RemoveModifier(Wall);
                EmitSignal(SignalName.StoppedDragging);
            }
        }
    }

    public bool ValidWallTouching {
        get => Enabled && ValidWallTouchingEnabled && _isValidWallTouching;
        set {
            if (!(Enabled && ValidWallTouchingEnabled) || value == _isValidWallTouching) return;

            _isValidWallTouching = value;

            if (!_isValidWallTouching) {
                IsDragging = false;
            }
            EmitSignal(ValidWallTouching ? SignalName.StartedValidWallTouching : SignalName.StoppedValidWallTouching);
        }
    }
    
    private bool _isDragging;
    private bool _isValidWallTouching;
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
        Wall.ModifiersUpdated += WallDataUpdated;
    }

    private void WallDataUpdated() {
        if (IsNodeReady()) {
            DraggingCheck();
            Falling.FallData.RefreshValues();
        }
    }

    public override void _PhysicsProcess(double delta) {
        DraggingCheck();
    }

    internal void DraggingCheck() {
        IsDragging = ValidWallTouching && Falling.VelocityAfterTransition.Y >= Wall.VelocityDragThreshold;
    }
}