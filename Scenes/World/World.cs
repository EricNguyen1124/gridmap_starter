using Godot;
using System;

public partial class World : Node3D
{
	// Called when the node enters the scene tree for the first time.
	private Player player;
	private DungeonGrid dungeon;

	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		dungeon = GetNode<DungeonGrid>("DungeonGrid");
		dungeon.GenerateLevel();
		SpawnPlayer();
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void SpawnPlayer()
	{
		var spawnPoint = dungeon.roomArray[0].GridCoordinates;
		player.GlobalPosition = new Vector3(spawnPoint.X * 2.0f + 1.0f, 0.5f, spawnPoint.Y * 2.0f + 1.0f);
		player.oldWorldPosition = player.GlobalPosition;
		//player.newWorldPosition = player.GlobalPosition;
		player.gridPosition = spawnPoint;
		//player.newGridPosition = spawnPoint;
		//new Vector2((float)(GridCoordinates.X + Width/2.0f)*2.0f, (float)(GridCoordinates.Y + Height/2.0f)*2.0f);
	}
}
