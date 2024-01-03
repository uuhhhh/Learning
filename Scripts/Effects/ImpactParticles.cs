using Godot;

namespace Learning.Scripts.Effects;

public partial class ImpactParticles : CpuParticles2D
{
    public const double DefaultIntensity = 1.0;

    private Tween _fadeTween;
    private float _originalInitialVelocityMax;
    private float _originalInitialVelocityMin;

    private int _originalNumParticles;

    public override void _Ready()
    {
        base._Ready();

        _originalNumParticles = Amount;
        _originalInitialVelocityMin = InitialVelocityMin;
        _originalInitialVelocityMax = InitialVelocityMax;
    }

    public PropertyTweener EmitParticles(double intensity = DefaultIntensity)
    {
        SetEmissionIntensity(intensity);
        Emitting = true;
        Restart();

        Color = GetColorWithOpacity(1);
        _fadeTween?.Kill();
        _fadeTween = CreateTween();

        return _fadeTween.TweenProperty(this, nameof(Color).ToLower(), GetColorWithOpacity(0),
            Lifetime);
    }

    private Color GetColorWithOpacity(float opacity)
    {
        return new Color(Color, opacity);
    }

    private void SetEmissionIntensity(double intensity)
    {
        Amount = Mathf.RoundToInt(Mathf.Lerp(0, _originalNumParticles, intensity));
        InitialVelocityMin =
            Mathf.RoundToInt(Mathf.Lerp(0, _originalInitialVelocityMin, intensity));
        InitialVelocityMax =
            Mathf.RoundToInt(Mathf.Lerp(0, _originalInitialVelocityMax, intensity));
    }
}