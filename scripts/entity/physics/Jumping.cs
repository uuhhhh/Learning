using System;
using System.Linq;
using Godot;

namespace Learning.scripts.entity.physics; 

public partial class Jumping : Node, IKinematicCompLinkable {
    [Export] public bool DoNotLink { get; set; }
    [Export] private Falling AirMovement { get; set; }
    [Export] private LeftRight HorizontalMovement { get; set; }

    [Export] private JumpingData Ground {
        get => JumpDataAll[(int)Location.Ground];
        set => _jumpDataAll[(int)Location.Ground] = value;
    }
    [Export] private JumpingData Air {
        get => JumpDataAll[(int)Location.Air];
        set => _jumpDataAll[(int)Location.Air] = value;
    }
    [Export] private JumpingData Wall {
        get => JumpDataAll[(int)Location.WallNonGround];
        set => _jumpDataAll[(int)Location.WallNonGround] = value;
    }
    
    private JumpingData[] JumpDataAll => _jumpDataAll;
    private JumpingData JumpData {
        get => JumpDataAll[(int)CurrentLocation];
        set => JumpDataAll[(int)CurrentLocation] = value;
    }

    public float JumpFacing { get; set; }
    public Location CurrentLocation { get; set; } = Location.None;
    public Location JumpedFrom { get; private set; } = Location.None;

    public int NumGroundJumps {
        get => _numJumpsAll[(int)Location.Ground];
        set => _numJumpsAll[(int)Location.Ground] = value;
    }
    public int NumAirJumps {
        get => _numJumpsAll[(int)Location.Air];
        set => _numJumpsAll[(int)Location.Air] = value;
    }
    public int NumWallJumps {
        get => _numJumpsAll[(int)Location.WallNonGround];
        set => _numJumpsAll[(int)Location.WallNonGround] = value;
    }
    public int[] NumJumpsAll => _numJumpsAll;
    public int NumJumps {
        get => NumJumpsAll[(int)CurrentLocation];
        private set => NumJumpsAll[(int)CurrentLocation] = value;
    }

    private Timer CoyoteJump { get; set; }
    private Timer JumpBuffer { get; set; }
    private Timer CoyoteWallJump { get; set; }
    private Timer WallJumpBuffer { get; set; }

    private Tween _jumpTweenY;
    private Tween _jumpTweenX;

    private JumpingData[] _jumpDataAll = new JumpingData[Enum.GetValues<Location>().Cast<int>().Max() + 1];
    private int[] _numJumpsAll = new int[Enum.GetValues<Location>().Cast<int>().Max() + 1];

    [Signal]
    public delegate void JumpedEventHandler(Location from);

    [Signal]
    public delegate void CancelledJumpEventHandler(Location jumpWasFrom);

    public override void _Ready() {
        CoyoteJump = GetNode<Timer>(nameof(CoyoteJump));
        JumpBuffer = GetNode<Timer>(nameof(JumpBuffer));
        CoyoteWallJump = GetNode<Timer>(nameof(CoyoteWallJump));
        WallJumpBuffer = GetNode<Timer>(nameof(WallJumpBuffer));

        CoyoteJump.Timeout += () => {
            CurrentLocation = Location.Air;
        };
        CoyoteWallJump.Timeout += () => {
            CurrentLocation = Location.Air;
        };
        
        ResetNumJumps();
    }

    public void Link(KinematicComp2 physics) {
        physics.BecomeOnFloor += _ => {
            CoyoteJump.Stop();
            CoyoteWallJump.Stop();
            
            CurrentLocation = Location.Ground;
            if (JumpBuffer.TimeLeft > 0) {
                Jump();
            }
        };
        physics.BecomeOffFloor += state => {
            if (state.IsOnWall()) {
                CurrentLocation = Location.WallNonGround;
                JumpFacing = state.GetWallNormal().X;
            } else if (AirMovement.Velocity.Y >= 0 && CurrentLocation != Location.None) {
                CoyoteJump.Start();
            } else {
                CurrentLocation = Location.Air;
            }
        };
        physics.BecomeOnWall += state => {
            CoyoteJump.Stop();
            CoyoteWallJump.Stop();
            
            if (!state.IsOnFloor()) {
                CurrentLocation = Location.WallNonGround;
                JumpFacing = state.GetWallNormal().X;
                if (WallJumpBuffer.TimeLeft > 0) {
                    Jump();
                }
            }
        };
        physics.BecomeOffWall += state => {
            if (!state.IsOnFloor()) {
                if (AirMovement.Velocity.Y >= 0 && CurrentLocation != Location.None) {
                    CoyoteWallJump.Start();
                } else {
                    CurrentLocation = Location.Air;
                }
            }
        };
    }

    public void AttemptJump() {
        if (CanJump()) {
            Jump();
        } else {
            JumpBuffer.Start();
            WallJumpBuffer.Start();
        }
    }

    private bool CanJump() {
        return NumJumps is JumpingData.UNLIMITED_JUMPS or > 0;
    }

    private void Jump() {
        PerformJump();

        if (NumJumps != JumpingData.UNLIMITED_JUMPS) {
            NumJumps--;
        }
        
        JumpedFrom = CurrentLocation;
        EmitSignal(SignalName.Jumped, (int)JumpedFrom);
        if (CoyoteJump.TimeLeft > 0 || CoyoteWallJump.TimeLeft > 0) {
            CurrentLocation = Location.Air;
        }
        
        JumpBuffer.Stop();
        WallJumpBuffer.Stop();
        CoyoteJump.Stop();
        CoyoteWallJump.Stop();
    }

    private void PerformJump() {
        if (JumpData.Velocity.Y != 0) {
            (_jumpTweenY, PropertyTweener t)
                = AirMovement.SmoothlySetBaseVelocityY(JumpData.Velocity.Y, JumpData.AccelTimeY);
            t.SetTrans(Tween.TransitionType.Expo);
        }
        if (JumpData.Velocity.X != 0) {
            (_jumpTweenX, PropertyTweener t)
                = HorizontalMovement.SmoothlySetBaseVelocityX(JumpData.Velocity.X * JumpFacing, JumpData.AccelTimeX);
            t.SetTrans(Tween.TransitionType.Expo);
        }
    }

    public void JumpCancel() {
        JumpBuffer.Stop();
        WallJumpBuffer.Stop();

        JumpingData lastJumpData = JumpDataAll[(int)JumpedFrom];
        if ((CurrentLocation == Location.Air && AirMovement.Velocity.Y < GetJumpCancelVelocityThreshold())
            || (_jumpTweenY != null && _jumpTweenY.IsValid())) {
            AirMovement.SmoothlySetBaseVelocityY(lastJumpData.CancelVelocity, lastJumpData.CancelAccelTime);
        }

        EmitSignal(SignalName.CancelledJump, (int)JumpedFrom);
    }

    private float GetJumpCancelVelocityThreshold() {
        JumpingData lastJumpData = JumpDataAll[(int)JumpedFrom];
        float extraVelocity = AirMovement.Gravity * AirMovement.GravityScale * lastJumpData.CancelAccelTime;
        return lastJumpData.CancelVelocity - extraVelocity;
    }

    public void ResetNumJumps() {
        foreach (Location l in Enum.GetValues<Location>()) {
            ResetNumJumps(l);
        }
    }
    
    public void ResetNumJumps(Location which) {
        if (which == Location.None) return;

        NumJumpsAll[(int)which] = JumpDataAll[(int)which].NumJumps;
    }
}