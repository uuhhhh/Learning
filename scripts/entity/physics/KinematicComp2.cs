using Godot;
using System.Collections.Generic;

namespace Learning.scripts.entity.physics; 

public partial class KinematicComp2 : CharacterBody2D {
    private readonly IList<VelocitySource> _velocitySources = new List<VelocitySource>();

    [Signal]
    public delegate void BecomeOnFloorEventHandler(KinematicComp2 state);
    [Signal]
    public delegate void BecomeOffFloorEventHandler(KinematicComp2 state);
    [Signal]
    public delegate void BecomeOnCeilingEventHandler(KinematicComp2 state);
    [Signal]
    public delegate void BecomeOffCeilingEventHandler(KinematicComp2 state);
    [Signal]
    public delegate void BecomeOnWallEventHandler(KinematicComp2 state);
    [Signal]
    public delegate void BecomeOffWallEventHandler(KinematicComp2 state);
    
    public override void _Ready() {
        AutoLinkChildren();
        SignalInitialState();
    }

    private void AutoLinkChildren() {
        foreach (Node c in GetChildren()) {
            if (c is VelocitySource { ExcludeThisVelocity: false} src) {
                _velocitySources.Add(src);
            }

            if (c is IKinematicCompLinkable { DoNotLink: false } l) {
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
        Velocity = Vector2.Zero;
        foreach (VelocitySource src in _velocitySources) {
            Velocity += src.Velocity;
        }
        
        MoveAndSlideWithStatusChanges();
    }

    protected virtual void MoveAndSlideWithStatusChanges() {
        bool wasOnFloor = IsOnFloor(), wasOnCeiling = IsOnCeiling(), wasOnWall = IsOnWall();
        MoveAndSlide();
        CheckFloorStatusChange(wasOnFloor);
        CheckCeilingStatusChange(wasOnCeiling);
        CheckWallStatusChange(wasOnWall);
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
    
    public void SetParentPositionToOwn(Node2D parent) {
        parent.Position = GlobalPosition;
        Position = Vector2.Zero;
    }
}