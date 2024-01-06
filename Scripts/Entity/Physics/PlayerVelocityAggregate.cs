using Godot;
using Learning.Scripts.Entity.Physics.Intermediate;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics;

/// <summary>
///     Combines behavior of various VelocitySources and intermediates,
///     in order to define the movement of the player.
/// </summary>
public partial class PlayerVelocityAggregate : VelocityAggregate
{
    private bool _canDoWallBehavior;

    private int _playerLeftRightInput;

    private Vector2 _wallDragCheckerInitialPosition;

    /// <summary>
    ///     The player's falling behavior and velocity.
    /// </summary>
    public Falling Falling { get; private set; }

    /// <summary>
    ///     The player's horizontal movement behavior and velocity.
    /// </summary>
    public LeftRight LeftRight { get; private set; }

    /// <summary>
    ///     The player's jump movement.
    /// </summary>
    public Jumping Jumping { get; private set; }

    /// <summary>
    ///     The player's wall dragging movement.
    /// </summary>
    public WallDragging WallDragging { get; private set; }

    /// <summary>
    ///     The player's wall snapping movement.
    /// </summary>
    public WallSnapping WallSnapping { get; private set; }

    private JumpingDefaultPhys JumpingDefaultPhys { get; set; }
    private WallDraggingDefaultPhys WallDraggingDefaultPhys { get; set; }

    private Timer WallJumpInputTakeover { get; set; }

    /// <summary>
    ///     Whether or not the player currently can do wall behavior, i.e., wall drag or wall jump.
    ///     Setting this to fall will stop wall dragging.
    /// </summary>
    public bool CanDoWallBehavior
    {
        get => _canDoWallBehavior;
        set
        {
            _canDoWallBehavior = value;

            ValidWallTouchingCheck(PhysicsInteractions);

            if (!_canDoWallBehavior &&
                Jumping.CurrentLocationAfterTransition == Location.WallNonGround)
                Jumping.TransitionToAir(true);
        }
    }

    public override void _Ready()
    {
        base._Ready();
        SetChildren();
    }

    public override void InitializeInteractions(KinematicComp interactions)
    {
        base.InitializeInteractions(interactions);

        InitNumJumpsBehavior();
        InitWallJumpInputTakeoverBehavior();
        InitWallTouchLeftRightStopBehavior();
        InitWallSnappingBehavior();
        ModifyWallTouchingBehavior();
    }

    private void SetChildren()
    {
        Falling = GetNode<Falling>(nameof(Falling));
        LeftRight = GetNode<LeftRight>(nameof(LeftRight));

        Jumping = GetNode<Jumping>(nameof(Jumping));
        WallDragging = GetNode<WallDragging>(nameof(WallDragging));
        WallSnapping = GetNode<WallSnapping>(nameof(WallSnapping));

        JumpingDefaultPhys = GetNode<JumpingDefaultPhys>(nameof(JumpingDefaultPhys));
        WallDraggingDefaultPhys = GetNode<WallDraggingDefaultPhys>(nameof(WallDraggingDefaultPhys));

        WallJumpInputTakeover = GetNode<Timer>(nameof(WallJumpInputTakeover));
    }

    private void InitNumJumpsBehavior()
    {
        PhysicsInteractions.BecomeOnFloor += _ => Jumping.ResetNumJumps();
        WallDragging.StartedValidWallTouching += Jumping.ResetNumJumps;
    }

    private void InitWallJumpInputTakeoverBehavior()
    {
        Jumping.Jumped += from =>
        {
            if (from == Location.WallNonGround) WallJumpInputTakeover.Start();
        };
        WallJumpInputTakeover.Timeout += UpdateLeftRightSpeed;
    }

    private void InitWallTouchLeftRightStopBehavior()
    {
        // can't set to exactly 0 due to physics weirdness
        // (this body changing state to becoming off wall when Falling tweening from above max velocity to max velocity)
        WallDragging.StartedValidWallTouching += () =>
            LeftRight.IntendedSpeed = Mathf.Sign(LeftRight.IntendedSpeed);

        PhysicsInteractions.BecomeOffWall += _ => UpdateLeftRightSpeedIfAble();
        PhysicsInteractions.BecomeOnFloor += _ => UpdateLeftRightSpeedIfAble();
    }

    private void InitWallSnappingBehavior()
    {
        WallSnapping.WallSnapStopped += () =>
        {
            if (!PhysicsInteractions.IsOnWall()) UpdateLeftRightSpeed();
        };
        WallDragging.StartedValidWallTouching += () => WallSnapping.IsWallSnapping = false;
        PhysicsInteractions.BecomeOffWall += _ =>
        {
            if (WallSnapping.IsWallSnapping)
            {
                WallSnapping.IsWallSnapping = false;
                UpdateLeftRightSpeed();
            }
        };
    }

    private void ModifyWallTouchingBehavior()
    {
        PhysicsInteractions.BecomeOnWall -= WallDraggingDefaultPhys.OnBecomeOnWall;
        PhysicsInteractions.BecomeOnWall -= JumpingDefaultPhys.OnBecomeOnWall;
        PhysicsInteractions.BecomeOnWall += ValidWallTouchingCheck;

        PhysicsInteractions.BecomeOffFloor -= WallDraggingDefaultPhys.OnBecomeOffFloor;
        PhysicsInteractions.BecomeOffFloor += ValidWallTouchingCheck;
    }

    private void ValidWallTouchingCheck(KinematicComp physics)
    {
        bool playerPressingAgainstWall = Mathf.Sign(physics.GetWallNormal().X) ==
                                         -Mathf.Sign(_playerLeftRightInput);
        bool noInputDragging = WallDragging.IsDragging && _playerLeftRightInput == 0;

        bool isValidWallPressing =
            playerPressingAgainstWall || noInputDragging || WallSnapping.IsWallSnapping;

        WallDragging.ValidWallTouching = WallDraggingDefaultPhys.IsOnValidWall(physics)
                                         && isValidWallPressing
                                         && CanDoWallBehavior;

        if (physics.IsOnWall() && CanDoWallBehavior) JumpingDefaultPhys.OnBecomeOnWall(physics);
    }

    /// <summary>
    ///     Intends for the player's x velocity to decrease. Updates the x velocity to the intended
    ///     amount when able.
    /// </summary>
    public void MoveLeft()
    {
        _playerLeftRightInput--;
        UpdateLeftRightSpeedIfAble();
    }

    /// <summary>
    ///     Intends for the player's x velocity to increase. Updates the x velocity to the intended
    ///     amount when able.
    /// </summary>
    public void MoveRight()
    {
        _playerLeftRightInput++;
        UpdateLeftRightSpeedIfAble();
    }

    private void UpdateLeftRightSpeedIfAble()
    {
        ValidWallTouchingCheck(PhysicsInteractions);

        WallSnapOppositeInputCheck();

        if (!WallSnapping.IsWallSnapping) JumpingDefaultPhys.DirectionGoing = _playerLeftRightInput;

        bool pressingOrStayingAgainstWall =
            WallDragging.ValidWallTouching
            && Mathf.Sign(PhysicsInteractions.GetWallNormal().X) !=
            Mathf.Sign(_playerLeftRightInput);
        if (!pressingOrStayingAgainstWall
            && !(WallJumpInputTakeover.TimeLeft > 0)
            && !WallSnapping.IsWallSnapping)
            UpdateLeftRightSpeed();
    }

    private void WallSnapOppositeInputCheck()
    {
        if (WallSnapping.IsWallSnapping
            && (_playerLeftRightInput == 0
                || Mathf.Sign(_playerLeftRightInput) == -Mathf.Sign(LeftRight.IntendedSpeedScale)))
            WallSnapping.IsWallSnapping = false;
    }

    private void UpdateLeftRightSpeed()
    {
        LeftRight.IntendedSpeedScale = _playerLeftRightInput;
    }

    /// <summary>
    ///     Makes the player jump, if they currently can jump.
    /// </summary>
    public void AttemptJump()
    {
        Jumping.AttemptJump();
    }

    /// <summary>
    ///     Makes the player cancel their current jump, if applicable.
    /// </summary>
    public void JumpCancel()
    {
        Jumping.JumpCancel();
    }
}