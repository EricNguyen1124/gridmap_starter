using System.Numerics;
using Godot;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

namespace Utilities.DebugDraw;

public static class DebugDraw
{
    public static Label3D GenerateLabel(string text, Vector2 pos)
    {
        Label3D label = new();
        label.RotationDegrees = new Vector3(-90,0,0);
        label.Scale = new Vector3(8,8,8);
        label.Text = text;
        label.Position = new Vector3(pos.X, 0, pos.Y);
        
        return label;
    }
}