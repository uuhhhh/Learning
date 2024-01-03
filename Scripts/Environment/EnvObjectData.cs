using Godot;
using Learning.Scripts.Entity.Physics;

namespace Learning.Scripts.Environment;

[GlobalClass]
public partial class EnvObjectData : Resource
{
    private int _airPriority;

    private int _floorPriority;
    private int _wallPriority;
    [Export] public FloorData Floor { get; private set; }

    [Export]
    public int FloorPriority
    {
        get => UseFloorDefaultPriority && Floor != null
            ? Floor.FloorDefaultPriority
            : _floorPriority;
        private set => _floorPriority = value;
    }

    [Export] private bool UseFloorDefaultPriority { get; set; }
    [Export] public AirData Air { get; private set; }

    [Export]
    public int AirPriority
    {
        get => UseAirDefaultPriority && Air != null ? Air.AirDefaultPriority : _airPriority;
        private set => _airPriority = value;
    }

    [Export] private bool UseAirDefaultPriority { get; set; }
    [Export] public WallData Wall { get; private set; }

    [Export]
    public int WallPriority
    {
        get => UseWallDefaultPriority && Wall != null ? Wall.WallDefaultPriority : _wallPriority;
        private set => _wallPriority = value;
    }

    [Export] private bool UseWallDefaultPriority { get; set; }
}