using Godot;

namespace Learning.Scripts.Values; 

public abstract partial class Modifier<TValue> : Resource, IModifier<TValue> {
    public const int DefaultPriority = 0;
    public const bool DefaultCacheable = true;

    [Export] public int Priority { get; private set; }
    [Export] public bool Cacheable { get; private set; }

    protected Modifier(int priority, bool cacheable) {
        Priority = priority;
        Cacheable = cacheable;
    }

    protected Modifier() : this(DefaultPriority, DefaultCacheable) {}
    
    public abstract TValue ApplyModifier(TValue value);
}