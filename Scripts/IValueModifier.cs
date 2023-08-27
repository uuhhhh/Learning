namespace Learning.Scripts; 

public interface IValueModifier {
    int Priority { get; }

    TValue ApplyModifier<TValue>(string valueName, TValue value);
}