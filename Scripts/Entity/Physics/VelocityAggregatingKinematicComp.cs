using Godot;

namespace Learning.Scripts.Entity.Physics;

/// <summary>
///     This is a KinematicComp that uses a VelocityAggregate's Velocity property as its Velocity.
/// </summary>
public partial class VelocityAggregatingKinematicComp : KinematicComp
{
    /// <summary>
    ///     The VelocityAggregate whose Velocity property is to be used by this node as its Velocity.
    /// </summary>
    [Export]
    private VelocityAggregate VelocityReplacer { get; set; }

    public override void _Ready()
    {
        VelocityReplacer.InitializeInteractions(this);
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Velocity = VelocityReplacer.Velocity;
        MoveAndSlideWithStatusChanges();
    }
}