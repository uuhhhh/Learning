using System.Collections.Generic;
using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics; 

public partial class VelocityAggregate : KinematicComp {
    [Export] public bool AggregateChildren { get; private set; } = true;
    
    private readonly IList<VelocitySource> _velocitySources = new List<VelocitySource>();
    
    public override void _Ready() {
        base._Ready();

        if (AggregateChildren) {
            AutoAggregateImmediateChildrenVelocitySources();
        }
    }

    private void AutoAggregateImmediateChildrenVelocitySources() {
        foreach (Node c in GetChildren()) {
            if (c is VelocitySource { ExcludeThisVelocity: false} src) {
                _velocitySources.Add(src);
            }
        }
    }

    public override void _PhysicsProcess(double delta) {
        Velocity = Vector2.Zero;
        foreach (VelocitySource src in _velocitySources) {
            Velocity += src.Velocity;
        }
        
        base._PhysicsProcess(delta);
    }
}