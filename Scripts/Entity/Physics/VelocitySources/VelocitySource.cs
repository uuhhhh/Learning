using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

public abstract partial class VelocitySource : Node
{
    private Tween _baseVelocityXTween;
    private bool _baseVelocityXTweenReady;
    private Tween _baseVelocityYTween;
    private bool _baseVelocityYTweenReady;
    private float _multiplierBeforeDisable;

    private Tween _multiplierTween;
    private bool _multiplierTweenReady;
    private float _tweeningBaseVelocityXTo;
    private float _tweeningBaseVelocityYTo;
    private float _tweeningMultiplierTo;
    [Export] internal bool ExcludeThisVelocity { get; private set; }
    [Export] internal bool UseSpeedScale { get; private set; } = true;
    [Export] internal float DefaultSmoothlyDisableTime { get; private set; } = .25f;
    [Export] internal float DefaultSmoothlyEnableTime { get; private set; } = .25f;

    public bool Enabled { get; set; } = true;

    public float Multiplier { get; set; } = 1;

    public Vector2 BaseVelocity { get; set; }

    public Vector2 BaseVelocityAfterTransition =>
        TransitioningBaseVelocityX || TransitioningBaseVelocityY
            ? new Vector2(
                TransitioningBaseVelocityX ? _tweeningBaseVelocityXTo : BaseVelocity.X,
                TransitioningBaseVelocityY ? _tweeningBaseVelocityYTo : BaseVelocity.Y)
            : BaseVelocity;

    public Vector2 Velocity => Enabled ? BaseVelocity * Multiplier : Vector2.Zero;

    public Vector2 VelocityAfterTransition =>
        Enabled ? BaseVelocityAfterTransition * Multiplier : Vector2.Zero;

    public bool TransitioningBaseVelocityX =>
        _baseVelocityXTween != null && _baseVelocityXTween.IsValid();

    public bool TransitioningBaseVelocityY =>
        _baseVelocityYTween != null && _baseVelocityYTween.IsValid();

    public bool TransitioningMultiplier => _multiplierTween != null && _multiplierTween.IsValid();

    public override void _PhysicsProcess(double delta)
    {
        if (!Enabled) return;

        BeginReadyTweens();
    }

    private void BeginReadyTweens()
    {
        if (_baseVelocityXTweenReady)
        {
            if (_baseVelocityXTween.IsValid()) _baseVelocityXTween.Play();
            _baseVelocityXTweenReady = false;
        }

        if (_baseVelocityYTweenReady)
        {
            if (_baseVelocityYTween.IsValid()) _baseVelocityYTween.Play();
            _baseVelocityYTweenReady = false;
        }

        if (_multiplierTweenReady)
        {
            if (_multiplierTween.IsValid()) _multiplierTween.Play();
            _multiplierTweenReady = false;
        }
    }

    protected (Tween, PropertyTweener) SmoothlySet(
        ref Tween toSet,
        string propertyName,
        Variant to,
        float duration)
    {
        toSet?.Kill();
        toSet = CreateTween();
        toSet.Pause();
        toSet.SetProcessMode(Tween.TweenProcessMode.Physics);
        if (UseSpeedScale) toSet.SetSpeedScale(1 / duration);

        PropertyTweener t
            = toSet.TweenProperty(this, propertyName, to, UseSpeedScale ? 1 : duration)
                .FromCurrent();

        return (toSet, t);
    }

    protected (Tween, PropertyTweener) SmoothlySet(
        ref Tween toSet,
        string propertyName,
        Variant from,
        Variant to,
        float duration)
    {
        (Tween t, PropertyTweener p) = SmoothlySet(ref toSet, propertyName, to, duration);
        p = p.From(from);

        return (t, p);
    }

    public (Tween, PropertyTweener) SmoothlySetBaseVelocityX(float to, float duration)
    {
        _baseVelocityXTweenReady = true;
        _tweeningBaseVelocityXTo = to;
        return SmoothlySet(ref _baseVelocityXTween,
            $"{nameof(BaseVelocity)}:x", // have to hardcode "x" since "BaseVelocity:X" doesn't work
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetBaseVelocityX(float from, float to, float duration)
    {
        _baseVelocityXTweenReady = true;
        _tweeningBaseVelocityXTo = to;
        return SmoothlySet(ref _baseVelocityXTween,
            $"{nameof(BaseVelocity)}:x",
            from,
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetBaseVelocityY(float to, float duration)
    {
        _baseVelocityYTweenReady = true;
        _tweeningBaseVelocityYTo = to;
        return SmoothlySet(ref _baseVelocityYTween,
            $"{nameof(BaseVelocity)}:y",
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetBaseVelocityY(float from, float to, float duration)
    {
        _baseVelocityYTweenReady = true;
        _tweeningBaseVelocityYTo = to;
        return SmoothlySet(ref _baseVelocityYTween,
            $"{nameof(BaseVelocity)}:y",
            from,
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetMultiplier(float to, float duration)
    {
        _multiplierTweenReady = true;
        _tweeningMultiplierTo = to;
        return SmoothlySet(ref _multiplierTween, nameof(Multiplier), to, duration);
    }

    public (Tween, PropertyTweener) SmoothlySetMultiplier(float from, float to, float duration)
    {
        _multiplierTweenReady = true;
        _tweeningMultiplierTo = to;
        return SmoothlySet(ref _multiplierTween, nameof(Multiplier), from, to, duration);
    }

    public void SmoothlyDisable(float timeToDisable)
    {
        if (!Enabled) return;

        _multiplierBeforeDisable = Multiplier;
        (Tween t, _) = SmoothlySetMultiplier(0, timeToDisable);
        t.Finished += () => Enabled = false;
    }

    public void SmoothlyDisable()
    {
        SmoothlyDisable(DefaultSmoothlyDisableTime);
    }

    public void SmoothlyEnable(float timeToEnable)
    {
        if (Enabled) return;

        SmoothlySetMultiplier(0, _multiplierBeforeDisable, timeToEnable);
        Enabled = true;
    }

    public void SmoothlyEnable()
    {
        SmoothlyEnable(DefaultSmoothlyEnableTime);
    }

    public void AbortAllTransitions()
    {
        _baseVelocityXTween?.Kill();
        _baseVelocityXTweenReady = false;
        _baseVelocityYTween?.Kill();
        _baseVelocityYTweenReady = false;
        _multiplierTween?.Kill();
        _multiplierTweenReady = false;
    }
}