using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

/// <summary>
/// A VelocitySource acts as an individual component of a VelocityAggregate's Velocity,
/// that can act and determine its own velocity independently of the entity's other VelocitySources.
/// </summary>
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
    
    /// <summary>
    /// If true, a VelocityAggregate won't add this VelocitySource
    /// to the VelocitySource it aggregates.
    /// </summary>
    [Export] internal bool ExcludeThisVelocity { get; private set; }
    
    /// <summary>
    /// If true, Tweens made by this VelocitySource will use a Tween's speed scale instead of
    /// duration for determining the duration of the Tween.
    /// </summary>
    [Export] internal bool UseSpeedScale { get; private set; } = true;
    
    /// <summary>
    /// The default time, in seconds, it takes for a VelocitySource's Velocity to go to zero
    /// upon smoothly disabling it.
    /// </summary>
    [Export] internal float DefaultSmoothlyDisableTime { get; private set; } = .25f;
    
    /// <summary>
    /// The default time, in seconds, it takes for a VelocitySource's Velocity to go from zero
    /// to its current actual velocity upon smoothly enabling it.
    /// </summary>
    [Export] internal float DefaultSmoothlyEnableTime { get; private set; } = .25f;

    /// <summary>
    /// When this is false, this VelocitySource is disabled, meaning the velocity will evaluate
    /// to zero.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// A global multiplier for this VelocitySource's velocity. (Not a global variable)
    /// </summary>
    public float Multiplier { get; set; } = 1;

    /// <summary>
    /// The velocity of this VelocitySource,
    /// before disabling and Multiplier are taken into account.
    /// </summary>
    public Vector2 BaseVelocity { get; set; }

    /// <summary>
    /// What the base velocity of this VelocitySource will be
    /// after the current active Tweens are finished. 
    /// </summary>
    public Vector2 BaseVelocityAfterTransition =>
        TransitioningBaseVelocityX || TransitioningBaseVelocityY
            ? new Vector2(
                TransitioningBaseVelocityX ? _tweeningBaseVelocityXTo : BaseVelocity.X,
                TransitioningBaseVelocityY ? _tweeningBaseVelocityYTo : BaseVelocity.Y)
            : BaseVelocity;

    /// <summary>
    /// The main velocity of this VelocitySource
    /// (after disabling and Multiplier are taken into account).
    /// This is the property that VelocityAggregate uses for this VelocitySource.
    /// </summary>
    public Vector2 Velocity => Enabled ? BaseVelocity * Multiplier : Vector2.Zero;

    /// <summary>
    /// What the main velocity of this VelocitySource will be
    /// after the current active Tweens are finished. 
    /// </summary>
    public Vector2 VelocityAfterTransition =>
        Enabled ? BaseVelocityAfterTransition * Multiplier : Vector2.Zero;

    /// <summary>
    /// Whether the x component of this VelocitySource's velocity
    /// is currently transitioning (Tweening) to a value.
    /// </summary>
    public bool TransitioningBaseVelocityX =>
        _baseVelocityXTween != null && _baseVelocityXTween.IsValid();

    /// <summary>
    /// Whether the y component of this VelocitySource's velocity
    /// is currently transitioning (Tweening) to a value.
    /// </summary>
    public bool TransitioningBaseVelocityY =>
        _baseVelocityYTween != null && _baseVelocityYTween.IsValid();

    /// <summary>
    /// Whether the Multiplier is currently transitioning (Tweening) to a value.
    /// </summary>
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

    /// <summary>
    /// Smoothly transitions the given property from its current value to the given value.
    /// </summary>
    /// <param name="toSet">The tween to use to change the property</param>
    /// <param name="propertyName">The name of the property to change</param>
    /// <param name="to">The value to tween to</param>
    /// <param name="duration">How long, in seconds, it will take to transition the property
    /// from its current value to the new value</param>
    /// <returns>The Tween and Tweener that is changing the property</returns>
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

    /// <summary>
    /// Smoothly transitions the given property from and to the given values.
    /// </summary>
    /// <param name="toSet">The tween to use to change the property</param>
    /// <param name="propertyName">The name of the property to change</param>
    /// <param name="from">The value to start at</param>
    /// <param name="to">The value to tween to</param>
    /// <param name="duration">How long, in seconds, it will take to transition the property
    /// between the values</param>
    /// <returns>The Tween and Tweener that is changing the property</returns>
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

    /// <summary>
    /// Smoothly transitions the x component of the base velocity
    /// from its current value to the given value.
    /// </summary>
    /// <param name="to">The value to tween to</param>
    /// <param name="duration">How long, in seconds, it will take to transition the property
    /// between the values</param>
    /// <returns>The Tween and Tweener that is changing the property</returns>
    public (Tween, PropertyTweener) SmoothlySetBaseVelocityX(float to, float duration)
    {
        _baseVelocityXTweenReady = true;
        _tweeningBaseVelocityXTo = to;
        return SmoothlySet(ref _baseVelocityXTween,
            $"{nameof(BaseVelocity)}:x", // have to hardcode "x" since "BaseVelocity:X" doesn't work
            to,
            duration);
    }

    /// <summary>
    /// Smoothly transitions the x component of the base velocity between the given values.
    /// </summary>
    /// <param name="from">The value to start at</param>
    /// <param name="to">The value to tween to</param>
    /// <param name="duration">How long, in seconds, it will take to transition the property
    /// from its current value to the new value</param>
    /// <returns>The Tween and Tweener that is changing the property</returns>
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

    /// <summary>
    /// Smoothly transitions the y component of the base velocity
    /// from its current value to the given value.
    /// </summary>
    /// <param name="to">The value to tween to</param>
    /// <param name="duration">How long, in seconds, it will take to transition the property
    /// between the values</param>
    /// <returns>The Tween and Tweener that is changing the property</returns>
    public (Tween, PropertyTweener) SmoothlySetBaseVelocityY(float to, float duration)
    {
        _baseVelocityYTweenReady = true;
        _tweeningBaseVelocityYTo = to;
        return SmoothlySet(ref _baseVelocityYTween,
            $"{nameof(BaseVelocity)}:y",
            to,
            duration);
    }

    /// <summary>
    /// Smoothly transitions the y component of the base velocity between the given values.
    /// </summary>
    /// <param name="from">The value to start at</param>
    /// <param name="to">The value to tween to</param>
    /// <param name="duration">How long, in seconds, it will take to transition the property
    /// from its current value to the new value</param>
    /// <returns>The Tween and Tweener that is changing the property</returns>
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

    /// <summary>
    /// Smoothly transitions the Multiplier of this VelocitySource
    /// from its current value to the given value.
    /// </summary>
    /// <param name="to">The value to tween to</param>
    /// <param name="duration">How long, in seconds, it will take to transition the property
    /// between the values</param>
    /// <returns>The Tween and Tweener that is changing the property</returns>
    public (Tween, PropertyTweener) SmoothlySetMultiplier(float to, float duration)
    {
        _multiplierTweenReady = true;
        _tweeningMultiplierTo = to;
        return SmoothlySet(ref _multiplierTween, nameof(Multiplier), to, duration);
    }

    /// <summary>
    /// Smoothly transitions the Multiplier of this VelocitySource between the given values.
    /// </summary>
    /// <param name="from">The value to start at</param>
    /// <param name="to">The value to tween to</param>
    /// <param name="duration">How long, in seconds, it will take to transition the property
    /// from its current value to the new value</param>
    /// <returns>The Tween and Tweener that is changing the property</returns>
    public (Tween, PropertyTweener) SmoothlySetMultiplier(float from, float to, float duration)
    {
        _multiplierTweenReady = true;
        _tweeningMultiplierTo = to;
        return SmoothlySet(ref _multiplierTween, nameof(Multiplier), from, to, duration);
    }

    /// <summary>
    /// Smoothly transitions this VelocitySource from being enabled to being disabled.
    /// Note that this affects the Multiplier and uses the Tween for the Multiplier,
    /// in order to transition from enabled to disabled.
    /// </summary>
    /// <param name="timeToDisable">How long, in seconds,
    /// it will take for disabling to complete</param>
    public void SmoothlyDisable(float timeToDisable)
    {
        if (!Enabled) return;

        _multiplierBeforeDisable = Multiplier;
        (Tween t, _) = SmoothlySetMultiplier(0, timeToDisable);
        t.Finished += () => Enabled = false;
    }

    /// <summary>
    /// Smoothly transitions this VelocitySource from being enabled to being disabled,
    /// in the default amount of time.
    /// Note that this affects the Multiplier and uses the Tween for the Multiplier,
    /// in order to transition from enabled to disabled.
    /// </summary>
    public void SmoothlyDisable()
    {
        SmoothlyDisable(DefaultSmoothlyDisableTime);
    }

    /// <summary>
    /// Smoothly transitions this VelocitySource from being enabled to being disabled.
    /// Note that this affects the Multiplier and uses the Tween for the Multiplier,
    /// in order to transition from enabled to disabled.
    /// </summary>
    /// <param name="timeToEnable">How long, in seconds,
    /// it will take for enabling to complete</param>
    public void SmoothlyEnable(float timeToEnable)
    {
        if (Enabled) return;

        SmoothlySetMultiplier(0, _multiplierBeforeDisable, timeToEnable);
        Enabled = true;
    }

    /// <summary>
    /// Smoothly transitions this VelocitySource from being enabled to being disabled,
    /// in the default amount of time.
    /// Note that this affects the Multiplier and uses the Tween for the Multiplier,
    /// in order to transition from enabled to disabled.
    /// </summary>
    public void SmoothlyEnable()
    {
        SmoothlyEnable(DefaultSmoothlyEnableTime);
    }

    /// <summary>
    /// Cancels all currently-running Tweens.
    /// </summary>
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