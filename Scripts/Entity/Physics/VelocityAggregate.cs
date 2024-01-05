using System.Collections.Generic;
using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics;

/// <summary>
/// A VelocityAggregate contains and aggregates one or more VelocitySources.
/// Subclasses can also add more rules and interactions for their VelocitySources
/// and intermediates, for more precise behavior.
/// </summary>
public partial class VelocityAggregate : Node
{
    private readonly IList<VelocitySource> _velocitySources = new List<VelocitySource>();
    
    /// <summary>
    /// Set this to true for any child Nodes that are VelocitySources to automatically
    /// be added to this VelocityAggregate upon initialization.
    /// </summary>
    [Export] private bool AggregateChildrenVelocitySources { get; set; } = true;
    
    /// <summary>
    /// Set this to true for any child Nodes that are DefaultPhys to automatically
    /// be linked to the KinematicComp when InitializeInteractions is called.
    /// </summary>
    [Export] private bool LinkChildrenDefaultPhys { get; set; } = true;
    
    /// <summary>
    /// Set this to true for any child Nodes that are DefaultPhys to automatically
    /// have ExtraInit called using the KinematicComp when InitializeInteractions is called.
    /// </summary>
    [Export] private bool ExtraInitChildrenDefaultPhys { get; set; } = true;
    
    /// <summary>
    /// The main source of physics interactions for this VelocityAggregate and for
    /// the DefaultPhys Nodes associated with this VelocityAggregate's VelocitySources.
    /// </summary>
    protected KinematicComp PhysicsInteractions { get; private set; }

    public Vector2 Velocity
    /// <summary>
    /// Aggregates this VelocityAggregate's VelocitySources into a single velocity vector.
    /// By default, this is done by taking the sum of the velocity vectors for each VelocitySource.
    /// </summary>
    {
        get
        {
            Vector2 velocity = Vector2.Zero;
            foreach (VelocitySource src in _velocitySources) velocity += src.Velocity;

            return velocity;
        }
    }

    /// <summary>
    /// Makes the given KinematicComp the main source of physics interactions for this
    /// VelocityAggregate and its child DefaultPhys Nodes. 
    /// </summary>
    /// <param name="interactions">The source of physics interactions</param>
    public virtual void InitializeInteractions(KinematicComp interactions)
    {
        PhysicsInteractions = interactions;

        if (LinkChildrenDefaultPhys)
            foreach (Node n in GetChildren())
                PhysicsInteractions.LinkDefaultPhys(n);

        if (ExtraInitChildrenDefaultPhys)
            foreach (Node n in GetChildren())
                PhysicsInteractions.ExtraInitDefaultPhys(n);
    }

    public void AddVelocitySourceToAggregated(Node toAggregate)
    /// <summary>
    /// Has this VelocityAggregate now also aggregate the given VelocitySource.
    /// Note that this method doesn't link or call ExtraInit on the DefaultPhys associated
    /// with the VelocitySource.
    /// </summary>
    /// <param name="toAggregate">The VelocitySource for this VelocityAggregate to </param>
    {
        if (toAggregate is VelocitySource {ExcludeThisVelocity: false} src)
            _velocitySources.Add(src);
    }
}