using System.Collections.Generic;
using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics;

public partial class VelocityAggregate : Node
{
    private readonly IList<VelocitySource> _velocitySources = new List<VelocitySource>();
    [Export] private bool AggregateChildrenVelocitySources { get; set; } = true;
    [Export] private bool LinkChildrenDefaultPhys { get; set; } = true;
    [Export] private bool ExtraInitChildrenDefaultPhys { get; set; } = true;
    protected KinematicComp PhysicsInteractions { get; private set; }

    public Vector2 Velocity
    {
        get
        {
            Vector2 velocity = Vector2.Zero;
            foreach (VelocitySource src in _velocitySources) velocity += src.Velocity;

            return velocity;
        }
    }

    public virtual void InitializeInteractions(KinematicComp interactions)
    {
        PhysicsInteractions = interactions;

        if (AggregateChildrenVelocitySources)
            foreach (Node n in GetChildren())
                AddVelocitySourceToAggregated(n);

        if (LinkChildrenDefaultPhys)
            foreach (Node n in GetChildren())
                PhysicsInteractions.LinkDefaultPhys(n);

        if (ExtraInitChildrenDefaultPhys)
            foreach (Node n in GetChildren())
                PhysicsInteractions.ExtraInitDefaultPhys(n);
    }

    public void AddVelocitySourceToAggregated(Node toAggregate)
    {
        if (toAggregate is VelocitySource {ExcludeThisVelocity: false} src)
            _velocitySources.Add(src);
    }
}