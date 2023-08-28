using System;

namespace Learning.Scripts; 

public interface IValueModifier {
    int Priority { get; }

    private const string CAST_FAIL_MSG = "toCast failed to cast to param type";

    TValue ApplyModifier<TValue>(string valueName, TValue value);

    protected static TTo Cast<TFrom, TTo>(TFrom toCast) {
        if (toCast is TTo casted) {
            return casted;
        }
        throw new ArgumentException(CAST_FAIL_MSG);
    }

    protected static TValue MultiplyFloat<TValue>(TValue toMultiplyT, float? multiplier) {
        if (toMultiplyT is not float toMultiply || !multiplier.HasValue) return toMultiplyT;

        return Cast<float, TValue>(toMultiply * multiplier.Value);
    }
}