using System;
using System.Linq;

namespace Learning.Scripts.Entity.Physics;

public enum Location {
    Ground,
    Air,
    WallNonGround,
    None = -1
}

public static class Locations {
    public static int NumLocationsNotNone() {
        return Enum.GetValues<Location>().Cast<int>().Max() + 1;
    }
}