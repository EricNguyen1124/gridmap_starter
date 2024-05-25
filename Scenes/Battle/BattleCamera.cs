using Godot;
using System;

public partial class BattleCamera : Camera3D
{
	double progress = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		progress += delta;
		//GlobalPosition = new Vector3((float)Math.Sin(progress) * 3, 0, (float)Math.Cos(progress) * 3);
		GlobalPosition = new Vector3((float)Math.Sin(progress) * 2 + 1, 0, (float)Math.Sin(progress)+2);
		GD.Print(GlobalPosition);
		//LookAt(Vector3.Zero);
	}
}