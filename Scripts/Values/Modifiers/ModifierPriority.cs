namespace Learning.Scripts.Values.Modifiers;

public enum ModifierPriority {
    DefaultPriority,
    WallDragging,
    WallSnapping
}

public static class ModifierPriorityExtensions {
    public static int PriorityNum(this ModifierPriority priority) {
        return priority switch {
            ModifierPriority.DefaultPriority => 0,
            ModifierPriority.WallDragging => -100,
            ModifierPriority.WallSnapping => -100,
            _ => 0
        };
    }
}