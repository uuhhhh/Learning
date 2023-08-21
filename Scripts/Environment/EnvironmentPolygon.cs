using Godot;

namespace Learning.Scripts.Environment;

public partial class EnvironmentPolygon : CollisionPolygon2D
{
    private Polygon2D _visibleShape;
    
    public override void _Ready() {
        _visibleShape = GetNode<Polygon2D>("VisibleShape");
        _visibleShape.Polygon = Polygon;
    }
}
