﻿namespace Learning.Scripts.Values.Modifiers; 

public partial class FunctionalModifier<TValue> : Modifier<TValue> {
    public delegate TValue ToApplyModifierTo(TValue value);
    
    public ToApplyModifierTo ToApplyModifierToFunc { get; }

    public FunctionalModifier(ToApplyModifierTo func, ModifierPriority priority, bool cacheable)
        : base(priority, cacheable) {
        ToApplyModifierToFunc = func;
    }

    public override TValue ApplyModifier(TValue value) {
        return ToApplyModifierToFunc.Invoke(value);
    }
}