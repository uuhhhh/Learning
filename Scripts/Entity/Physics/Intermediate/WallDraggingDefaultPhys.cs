using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate;

/// <summary>
/// Default behavior for what a WallDragging will do, based on the actions of a KinematicComp
/// </summary>
public partial class WallDraggingDefaultPhys : DefaultPhys
{
    /// <summary>
    /// The WallDragging to be controlled by the callback KinematicComp.
    /// </summary>
    [Export] private WallDragging ToLink { get; set; }

    /// <inheritdoc cref="OnBecomeOffWall"/>
    internal override void OnBecomeOnFloor(KinematicComp physics)
    {
        OnBecomeOffWall(physics);
    }

    /// <inheritdoc cref="OnBecomeOffWall"/>
    internal override void OnBecomeOffFloor(KinematicComp physics)
    {
        OnBecomeOnWall(physics);
    }

    /// <summary>
    /// When the given KinematicComp becomes on a wall, check if the wall touch is valid
    /// (i.e., eligible for wall dragging)
    /// </summary>
    internal override void OnBecomeOnWall(KinematicComp physics)
    {
        if (IsOnValidWall(physics)) ToLink.ValidWallTouching = true;
    }

    /// <summary>
    /// When the given KinematicComp becomes off a wall, make the WallDragging no longer be valid
    /// wall touching.
    /// </summary>
    internal override void OnBecomeOffWall(KinematicComp physics)
    {
        ToLink.ValidWallTouching = false;
    }

    /// <returns>Whether the given body is validly touching a wall
    /// (i.e., eligible for wall dragging)</returns>
    internal static bool IsOnValidWall(CharacterBody2D physics)
    {
        return !physics.IsOnFloor() && physics.IsOnWall() && physics.GetWallNormal().Y == 0;
    }
}