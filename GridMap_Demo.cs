using Godot;
using System;
using System.Collections.Generic;
using Scenes.Room;
using System.Linq;
using Utilities.DebugDraw;

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
			pointsList[roomsPlaced] = potentialRoom.WorldPosition;
			roomsPlaced += 1;
			DrawRooms();
		}

		GDScript DelaunayShellScript = GD.Load<GDScript>("res://Utilities/DelaunayShell.gd");
		GodotObject DelaunayShellNode = (GodotObject)DelaunayShellScript.New();
		var args = Variant.CreateFrom(pointsList);
		var huh = DelaunayShellNode.Call("get_triangles", pointsList);
		
		foreach (var edge in huh.As<Godot.Collections.Array>())
		{
			Vector2 fromPoint = edge.As<Godot.Collections.Array>()[0].As<Vector2>();
			Room fromRoom = roomArray.Single(r => r.WorldPosition == fromPoint);

			Vector2 toPoint = edge.As<Godot.Collections.Array>()[1].As<Vector2>();
			Room toRoom = roomArray.Single(r => r.WorldPosition == toPoint);

			if (!fromRoom.Edges.Any(e => e.RoomId == toRoom.Id))
			{
				Edge e = new(toRoom.Id);
				fromRoom.Edges.Add(e);
			}

			if (!toRoom.Edges.Any(e => e.RoomId == fromRoom.Id))
			{
				Edge e = new(fromRoom.Id);
				toRoom.Edges.Add(e);
			}
		}
		
		int attempts = 0;
		List<Room> visitedRooms = new();

		while (visitedRooms.Count != roomArray.Count)
		{
			if (attempts > 100) 
			{
				GD.PrintErr("Failed to pick edges");
				return;
			};

			foreach (Room room in roomArray)
			{
				foreach (Edge edge in room.Edges)
				{
					edge.Active = true;
				}
			}
			Dfs(visitedRooms, roomArray.Single(r => r.Id == 0));
			visitedRooms.Clear();
			Dfs(visitedRooms, roomArray.Single(r => r.Id == 0), true);
			attempts += 1;
		}
		var label = DebugDraw.GenerateLabel("Hi",new Vector2(3,5));
		AddChild(label);
		GD.Print(attempts);
		GD.Print("hi");
	}

	private void Dfs(List<Room> visited, Room room, bool checkActive = false)
	{
		if (!visited.Contains(room))
		{
			visited.Add(room);
			List<Edge> edges = checkActive ? room.Edges.FindAll(e => e.Active) : room.Edges;
			foreach (Edge edge in edges)
			{
				var toRoom = roomArray.Single((r) => r.Id == edge.RoomId);

				if (!checkActive && GD.Randf() > percentPaths && room.Edges.Count(e => e.Active) > 1)
				{
					edge.Active = false;
					toRoom.SetEdgeActive(room.Id, false);
				}

				Dfs(visited, toRoom, checkActive);
			}
		}
	}

	// private void DfsActive(List<Room> visited, Room room)
	// {
	// 	if (!visited.Contains(room))
	// 	{
	// 		visited.Add(room);
	// 		List<Edge> activeEdges = room.Edges.FindAll(e => e.Active);
	// 		foreach (Edge edge in activeEdges)
	// 		{
	// 			var toRoom = roomArray.Single((r) => r.Id == edge.RoomId);
	// 			DfsActive(visited, toRoom);
	// 		}
	// 	}
	// }

	private void DrawRooms()
	{
		Clear();

		foreach (var room in roomArray)
		{
			for (int i = 0; i < room.Width; i++)
			{
				for (int j = 0; j < room.Height; j++)
				{
					SetCellItem(new Vector3I(room.position.X + i, 0, room.position.Y + j), 0);
				}
			}
		}
	}
}
