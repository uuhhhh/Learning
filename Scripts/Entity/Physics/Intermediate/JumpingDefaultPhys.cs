using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate;

/// <summary>
/// Default behavior for what a Jumping will do, based on the actions of a KinematicComp
/// </summary>
public partial class JumpingDefaultPhys : DefaultPhys
{
    private float _directionGoing;

    private float _wallDirection;
    
    /// <summary>
    /// The Jumping to be controlled by the callback KinematicComp.
    /// </summary>
    [Export] private Jumping ToLink { get; set; }

    /// <summary>
    /// The x component of the normal vector for the wall that the callback KinematicComp
    /// is touching. A value of 0 means that a wall isn't being touched.
    /// </summary>
    public float WallDirection
    {
        get => _wallDirection;
        set
        {
            _wallDirection = value;
            WallPressCheck();
        }
    }

    /// <summary>
    /// What direction the Jumping's LeftRight intends to move. A value of 0 means that the
    /// LeftRight intends to not move.
    /// </summary>
    public float DirectionGoing
    {
        get => _directionGoing;
        set
        {
            _directionGoing = value;
            WallPressCheck();
        }
    }

    /// <summary>
    /// Update DirectionGoing whenever the Jumping's LeftRight intends to change x velocity.
    /// </summary>
    internal override void ExtraInit(KinematicComp physics)
    {
        ToLink.LeftRight.IntendedSpeedChange += speed => DirectionGoing = speed;
    }

    /// <summary>
    /// When the given KinematicComp becomes on a floor, transition the Jumping's location to the
    /// ground if necessary.
    /// </summary>
    internal override void OnBecomeOnFloor(KinematicComp physics)
    {
        WallDirection = 0;
        if (physics.IsOnFloor() && ToLink.CurrentLocationAfterTransition != Location.Ground)
            ToLink.TransitionToGround();
    }

    /// <summary>
    /// When the given KinematicComp becomes on a floor, transition the Jumping's location to the
    /// wall or air as appropriate.
    /// </summary>
    internal override void OnBecomeOffFloor(KinematicComp physics)
    {
        if (physics.IsOnWall() && ToLink.CurrentLocationAfterTransition != Location.WallNonGround)
            WallDirection = physics.GetWallNormal().X;
        else if (ToLink.CurrentLocationAfterTransition != Location.Air) ToLink.TransitionToAir();
    }

    /// <summary>
    /// When the given KinematicComp becomes on a wall, transition the Jumping's location to the
    /// ground if necessary.
    /// </summary>
    internal override void OnBecomeOnWall(KinematicComp physics)
    {
        if (physics.IsOnWall() && !physics.IsOnFloor()
                               && ToLink.CurrentLocationAfterTransition != Location.WallNonGround)
            WallDirection = physics.GetWallNormal().X;
    }

    /// <summary>
    /// When the given KinematicComp becomes off a wall, transition the Jumping's location to the
    /// air if appropriate.
    /// </summary>
    internal override void OnBecomeOffWall(KinematicComp physics)
    {
        WallDirection = 0;
        if (!physics.IsOnFloor() && ToLink.CurrentLocationAfterTransition != Location.Air)
            ToLink.TransitionToAir();
    }

    /// <summary>
    /// If the Jumping's LeftRight is intending to move in the same direction as the wall being
    /// touched, transition the Jumping's location to the wall.
    /// </summary>
    internal void WallPressCheck()
    {
        if (!ToLink.IsJumpingEnabledFor(Location.WallNonGround) &&
            ToLink.IsJumpingEnabledFor(Location.Air)) return;

        int wallDirectionSign = Mathf.Sign(WallDirection);
        if (wallDirectionSign != 0 && (wallDirectionSign == -Mathf.Sign(DirectionGoing)
                                       || ToLink.CurrentLocationAfterTransition ==
                                       Location.WallNonGround))
            ToLink.TransitionToWall(WallDirection);
    }
}