using System;
using Godot;
using System.Collections.Generic;

namespace Learning.scripts.entity.physics; 

public partial class KinematicComp2 : CharacterBody2D {
    [Export] private float _directionChangeEpsilon = .01f;
    
    private readonly IList<VelocitySource> _velocitySources = new List<VelocitySource>();

    // Whenever a new signal is added here, be sure the change is reflected in IKinematicCompLinkable also
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
    [Signal]
    public delegate void DirectionChangeXEventHandler(KinematicComp2 state, float newDirection);
    [Signal]
    public delegate void DirectionChangeYEventHandler(KinematicComp2 state, float newDirection);

    private Vector2 _mostExtremePosition;
    private bool _increasingX, _increasingY;
    
    public override void _Ready() {
        _mostExtremePosition = GlobalPosition;
        
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
        
        if (Mathf.Abs(GlobalPosition.X - _mostExtremePosition.X) > _directionChangeEpsilon) {
            EmitSignal(SignalName.DirectionChangeX, this, GlobalPosition.X - _mostExtremePosition.X);
            _increasingX = !_increasingX;
            _mostExtremePosition.X = GlobalPosition.X;
        }
        
        _mostExtremePosition.Y = _increasingY ?
            Mathf.Max(GlobalPosition.Y, _mostExtremePosition.Y)
            : Mathf.Min(GlobalPosition.Y, _mostExtremePosition.Y);
        
        if (Mathf.Abs(GlobalPosition.Y - _mostExtremePosition.Y) > _directionChangeEpsilon) {
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