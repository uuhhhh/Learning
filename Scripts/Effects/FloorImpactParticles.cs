using Godot;

namespace Learning.Scripts.Effects;

/// <summary>
/// This set of particles is meant to look like a body is hitting the floor after falling.
/// </summary>
public partial class FloorImpactParticles : ImpactParticles
{
    /// <summary>
    /// The minimum fall distance for FloorImpact to emit any particles.
    /// </summary>
    [Export] public float MinFallDistance { get; set; }
    
    /// <summary>
    /// The minimum fall distance for FloorImpact to emit the maximum intensity of particles.
    /// </summary>
    [Export] public float MaxFallDistance { get; set; }

    /// <summary>
    /// Emits particles that look like the "splash" of hitting a floor.
    /// </summary>
    /// <param name="fallDistance">The distance the thing hitting the floor fell</param>
    /// <returns>The Tweener that makes the particles fade</returns>
    public PropertyTweener FloorImpact(float fallDistance)
    {
        if (fallDistance < MinFallDistance) return null;

        double intensity = (fallDistance - MinFallDistance) / (MaxFallDistance - MinFallDistance);
        return EmitParticles(Mathf.Min(1, intensity));
    }
}