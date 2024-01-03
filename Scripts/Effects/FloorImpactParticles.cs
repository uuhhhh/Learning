using Godot;

namespace Learning.Scripts.Effects;

public partial class FloorImpactParticles : ImpactParticles
{
    [Export] public float MinFallDistance { get; set; }
    [Export] public float MaxFallDistance { get; set; }

    public PropertyTweener FloorImpact(float fallDistance)
    {
        if (fallDistance < MinFallDistance) return null;

        double intensity = (fallDistance - MinFallDistance) / (MaxFallDistance - MinFallDistance);
        return EmitParticles(Mathf.Min(1, intensity));
    }
}