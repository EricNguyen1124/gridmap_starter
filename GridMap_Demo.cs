using Godot;
using System;
using System.Collections.Generic;
using Scenes.Room;
using System.Linq;

public partial class GridMap_Demo : GridMap
{
	private int levelSizeZ = 30;
	private int levelSizeX = 40;
	private int numberOfRooms = 6;
	private float percentPaths = 0.5f;

	private List<Room> roomArray = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetCellItem(new Vector3I(0,0,0), 0);
		SetCellItem(new Vector3I(levelSizeX, 0, levelSizeZ), 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_down"))
		{
			roomArray.Clear();
			GenerateLevel();
		}
	}

	private void GenerateLevel()
	{
		int roomsPlaced = 0;
		Vector2[] pointsList = new Vector2[numberOfRooms];

		while (roomsPlaced < numberOfRooms)
		{
			Room potentialRoom = new();

			bool roomCollides = true;
			while (roomCollides)
			{
				potentialRoom.SetRoomPosAndSize(levelSizeX, levelSizeZ);
				roomCollides = roomArray.Any((r) => potentialRoom.CheckRoomCollide(r));
			}
			potentialRoom.Id = roomsPlaced;
			roomArray.Add(potentialRoom);
			pointsList[roomsPlaced] = potentialRoom.worldPosition;
			roomsPlaced += 1;
			DrawRooms();
		}

		GDScript DelaunayShellScript = GD.Load<GDScript>("res://Utilities/DelaunayShell.gd");
		GodotObject DelaunayShellNode = (GodotObject)DelaunayShellScript.New();
		var args = Variant.CreateFrom(pointsList);
		var huh = DelaunayShellNode.Call("get_triangles", pointsList);
		var idk = huh.As<Godot.Collections.Array>();
		GD.Print(huh);
	}

	private void DrawRooms()
	{
		Clear();

		foreach (var room in roomArray)
		{
			for (int i = 0; i < room.width; i++)
			{
				for (int j = 0; j < room.height; j++)
				{
					SetCellItem(new Vector3I(room.position.X + i, 0, room.position.Y + j), 0);
				}
			}
		}
	}
}
