using Godot;

namespace Learning.Scripts.Values.Modifiers;

/// <summary>
///     An abstract IModifier that's also a Godot Resource, so values for subclasses of Modifier
///     can specify values in the Godot editor.
/// </summary>
/// <typeparam name="TValue">The type of the value to apply modifications to.</typeparam>
public abstract partial class Modifier<TValue> : Resource, IModifier<TValue>
{
    public const int DefaultPriority = 0;
    public const bool DefaultCacheable = true;

    protected Modifier(ModifierPriority priority, bool cacheable)
    {
        Priority = priority;
        Cacheable = cacheable;
    }

    protected Modifier() : this(DefaultPriority, DefaultCacheable)
    {
    }

    [Export] public ModifierPriority Priority { get; private set; }
    [Export] public bool Cacheable { get; private set; }

    public abstract TValue ApplyModifier(TValue value);
}