using System;
using System.Linq;

namespace Learning.Scripts.Entity.Physics;

/// <summary>
///     The different locations that a Jumping recognizes as places that can be jumped from.
/// </summary>
public enum Location
{
    Ground,
    Air,
    WallNonGround,
    None = -1
}

public static class Locations
{
    /// <returns>The total number of Locations that aren't Location.None.</returns>
    public static int NumLocationsNotNone()
    {
        return Enum.GetValues<Location>().Cast<int>().Max() + 1;
    }
}