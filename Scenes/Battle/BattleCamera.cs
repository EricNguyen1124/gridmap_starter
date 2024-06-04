using Godot;
using Scenes.Enemy;
using System;

public partial class BattleCamera : Camera3D
{
	double progress = 0;

	private HealthBar healthBar;
	private Enemy enemy;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthBar = GetNode<HealthBar>("ProgressBar");
		enemy = GetParent().GetNode<Enemy>("Marker3D/Enemy");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		progress += delta;
		//GlobalPosition = new Vector3((float)Math.Sin(progress) * 3, 0, (float)Math.Cos(progress) * 3);
		//GlobalPosition = new Vector3((float)Math.Sin(progress) * 2 + 1, 0, (float)Math.Sin(progress)+2);

		Vector2 barPos = UnprojectPosition(enemy.GlobalPosition + new Vector3(-0.25f, 0.5f, 0));
		healthBar.Position = barPos;
		//LookAt(Vector3.Zero);
	}
}