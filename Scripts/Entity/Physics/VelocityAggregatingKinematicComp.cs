using Godot;

namespace Learning.Scripts.Entity.Physics;

public partial class VelocityAggregatingKinematicComp : KinematicComp
{
    [Export] private VelocityAggregate Velocities { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = Velocities.Velocity;

        MoveAndSlideWithStatusChanges();
    }
}