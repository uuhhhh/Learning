using Godot;

namespace Learning.Scripts.Entity.Physics; 

public partial class KinematicComp : CharacterBody2D {
    [Export] public bool AutoLinkImmediateChildren { get; set; } = true;
    [Export] private float DirectionChangeEpsilon { get; set; } = .01f;

    // Whenever a new signal is added here, be sure the change is reflected in IKinematicCompLinkable also
    [Signal]
    public delegate void BecomeOnFloorEventHandler(KinematicComp state);
    [Signal]
    public delegate void BecomeOffFloorEventHandler(KinematicComp state);
    [Signal]
    public delegate void BecomeOnCeilingEventHandler(KinematicComp state);
    [Signal]
    public delegate void BecomeOffCeilingEventHandler(KinematicComp state);
    [Signal]
    public delegate void BecomeOnWallEventHandler(KinematicComp state);
    [Signal]
    public delegate void BecomeOffWallEventHandler(KinematicComp state);
    [Signal]
    public delegate void DirectionChangeXEventHandler(KinematicComp state, float newDirection);
    [Signal]
    public delegate void DirectionChangeYEventHandler(KinematicComp state, float newDirection);

    private Vector2 _mostExtremePosition;
    private bool _increasingX, _increasingY;
    
    public override void _Ready() {
        _mostExtremePosition = GlobalPosition;

        if (AutoLinkImmediateChildren) {
            LinkImmediateChildren();
        }
        SignalInitialState();
    }

    private void LinkImmediateChildren() {
        foreach (Node c in GetChildren()) {
            if (c is IDefaultPhys { DoNotLink: false } l) {
                l.Link(this);
            }
        }
    }

    private void SignalInitialState() {
        EmitSignal(IsOnCeiling() ? SignalName.BecomeOnCeiling : SignalName.BecomeOffCeiling, this);
        EmitSignal(IsOnWall() ? SignalName.BecomeOnWall : SignalName.BecomeOffWall, this);
        EmitSignal(IsOnFloor() ? SignalName.BecomeOnFloor : SignalName.BecomeOffFloor, this);
    }

    public override void _PhysicsProcess(double delta) {
        MoveAndSlideWithStatusChanges();
    }

    protected virtual void MoveAndSlideWithStatusChanges() {
        bool wasOnFloor = IsOnFloor(), wasOnCeiling = IsOnCeiling(), wasOnWall = IsOnWall();

        MoveAndSlide();
        
        CheckFloorStatusChange(wasOnFloor);
        CheckCeilingStatusChange(wasOnCeiling);
        CheckWallStatusChange(wasOnWall);
        
        CheckDirectionChange();
    }

    private void CheckFloorStatusChange(bool wasOnFloor) {
        switch (wasOnFloor, IsOnFloor()) {
            case (true, false):
                EmitSignal(SignalName.BecomeOffFloor, this);
                break;
            case (false, true):
                EmitSignal(SignalName.BecomeOnFloor, this);
                break;
        }
    }

    private void CheckCeilingStatusChange(bool wasOnCeiling) {
        switch (wasOnCeiling, IsOnCeiling()) {
            case (true, false):
                EmitSignal(SignalName.BecomeOffCeiling, this);
                break;
            case (false, true):
                EmitSignal(SignalName.BecomeOnCeiling, this);
                break;
        }
    }

    private void CheckWallStatusChange(bool wasOnWall) {
        switch (wasOnWall, IsOnWall()) {
            case (true, false):
                EmitSignal(SignalName.BecomeOffWall, this);
                break;
            case (false, true):
                EmitSignal(SignalName.BecomeOnWall, this);
                break;
        }
    }

    private void CheckDirectionChange() {
        _mostExtremePosition.X = _increasingX ?
            Mathf.Max(GlobalPosition.X, _mostExtremePosition.X)
            : Mathf.Min(GlobalPosition.X, _mostExtremePosition.X);
        
        if (Mathf.Abs(GlobalPosition.X - _mostExtremePosition.X) > DirectionChangeEpsilon) {
            EmitSignal(SignalName.DirectionChangeX, this, GlobalPosition.X - _mostExtremePosition.X);
            _increasingX = !_increasingX;
            _mostExtremePosition.X = GlobalPosition.X;
        }
        
        _mostExtremePosition.Y = _increasingY ?
            Mathf.Max(GlobalPosition.Y, _mostExtremePosition.Y)
            : Mathf.Min(GlobalPosition.Y, _mostExtremePosition.Y);
        
        if (Mathf.Abs(GlobalPosition.Y - _mostExtremePosition.Y) > DirectionChangeEpsilon) {
            EmitSignal(SignalName.DirectionChangeY, this, GlobalPosition.Y - _mostExtremePosition.Y);
            _increasingY = !_increasingY;
            _mostExtremePosition.Y = GlobalPosition.Y;
        }
    }
    
    public void SetParentPositionToOwn(Node2D parent) {
        parent.Position = GlobalPosition;
        Position = Vector2.Zero;
    }
}