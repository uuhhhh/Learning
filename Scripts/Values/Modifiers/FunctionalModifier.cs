namespace Learning.Scripts.Values.Modifiers;

public partial class FunctionalModifier<TValue> : Modifier<TValue>
{
    public delegate TValue ToApplyModifierTo(TValue value);

    public FunctionalModifier(ToApplyModifierTo func, ModifierPriority priority, bool cacheable)
        : base(priority, cacheable)
    {
        ToApplyModifierToFunc = func;
    }

    public ToApplyModifierTo ToApplyModifierToFunc { get; }

    public override TValue ApplyModifier(TValue value)
    {
        return ToApplyModifierToFunc.Invoke(value);
    }
}