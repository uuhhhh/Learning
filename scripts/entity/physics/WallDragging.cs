using System;
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
    
    public bool ValidWallTouching { get; set; }

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
        if (ValidWallTouching && AirMovement.VelocityAfterTransition.Y > VelocityThreshold) {
            IsDragging = true;
        }
    }

    public void DefaultOnBecomeOnFloor(KinematicComp physics) {
        DefaultOnBecomeOffWall(physics);
    }

    public void DefaultOnBecomeOffFloor(KinematicComp physics) {
        DefaultOnBecomeOnWall(physics);
    }

    public void DefaultOnBecomeOnWall(KinematicComp physics) {
        if (IsOnValidWall(physics)) {
            ValidWallTouching = true;
        }
    }

    public void DefaultOnBecomeOffWall(KinematicComp physics) {
        ValidWallTouching = false;
        IsDragging = false;
    }

    public bool IsOnValidWall(KinematicComp physics) {
        return !physics.IsOnFloor() && physics.IsOnWall() && physics.GetWallNormal().Y == 0;
    }
}