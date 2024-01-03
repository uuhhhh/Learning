﻿using Godot;

namespace Learning.Scripts.Values.Modifiers; 

public abstract partial class Modifier<TValue> : Resource, IModifier<TValue> {
    public const int DefaultPriority = 0;
    public const bool DefaultCacheable = true;

    [Export] public ModifierPriority Priority { get; private set; }
    [Export] public bool Cacheable { get; private set; }

    protected Modifier(ModifierPriority priority, bool cacheable) {
        Priority = priority;
        Cacheable = cacheable;
    }

    protected Modifier() : this(DefaultPriority, DefaultCacheable) {}
    
    public abstract TValue ApplyModifier(TValue value);
}