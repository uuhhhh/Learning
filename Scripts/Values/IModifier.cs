namespace Learning.Scripts.Values; 

public interface IModifier<TValue> {
    public int Priority { get; }
    public bool Cacheable { get; }

    public TValue ApplyModifier(TValue value);
}