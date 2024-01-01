using System;
using Godot;

namespace Learning.Scripts; 

public partial class IntCapper : Modifier<int> {
    [Export] public int Cap { get; private set; }

    public override int ApplyModifier(int value) {
        return Math.Min(value, Cap);
    }
}