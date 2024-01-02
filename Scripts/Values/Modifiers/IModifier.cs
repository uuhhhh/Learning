namespace Learning.Scripts.Values.Modifiers; 

public interface IModifier<TValue> {
    public int Priority { get; }
    public bool Cacheable { get; }

    public TValue ApplyModifier(TValue value);
}