using Godot;
using System;
using System.Collections.Generic;
using Scenes.Room;
using System.Linq;
using Utilities.DebugDraw;

public partial class GridMap_Demo : GridMap
{
	static readonly Random rng = new Random();

	private int levelSizeZ = 30;
	private int levelSizeX = 40;
	private int numberOfRooms = 7;
	private float percentPaths = 0.6f;

	private List<Room> roomArray = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetCellItem(new Vector3I(0,0,0), 0);
		SetCellItem(new Vector3I(levelSizeX, 0, levelSizeZ), 0);
		DebugDraw3D.DebugEnabled = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_down"))
		{
			roomArray.Clear();
			GenerateLevel();
		}
		ShowEdges();
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
			SetRoomCells();
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
			visitedRooms.Clear();
			Dfs(visitedRooms, roomArray.Single(r => r.Id == 0));
			visitedRooms.Clear();
			Dfs(visitedRooms, roomArray.Single(r => r.Id == 0), true);
			attempts += 1;
		}

		List<(int, int)> drawnEdge = new();
		foreach(Room room in roomArray)
		{
			foreach(Edge edge in room.Edges.FindAll(e => e.Active))
			{
				if (!drawnEdge.Contains((room.Id, edge.RoomId)) && !drawnEdge.Contains((edge.RoomId, room.Id)))
				{
					List<(int x, int y)> path = RandomPath(room, roomArray.Single(r => r.Id == edge.RoomId), 4);

					foreach (var (x, y) in path)
					{
						SetCellItem(new Vector3I(x, 0, y), 0);
					}

					drawnEdge.Add((room.Id, edge.RoomId));
				}
			}
		}
		
		GD.Print(attempts);
		GD.Print("hi");
	}

	enum CellState {OPEN, FORCED, BLOCKED}
	private List<(int x, int y)> RandomPath(Room fromRoom, Room toRoom, float wiggliness = 1)
	{
		(int x, int y) fromPos = (fromRoom.GridCoordinates.X, fromRoom.GridCoordinates.Y);
		(int x, int y) toPos = (toRoom.GridCoordinates.X, toRoom.GridCoordinates.Y);

		List<(int x, int y)> openCells = new();
		Dictionary<(int x, int y), CellState> cellStates = new();

		for (int x = 0; x < levelSizeX + 5; x++)
		{
			for (int y = 0; y < levelSizeZ + 5; y++)
			{
				openCells.Add((x, y));
				cellStates[(x, y)] = CellState.OPEN;
			}
		}

		openCells.Remove(fromPos);
		openCells.Remove(toPos);
		cellStates[fromPos] = CellState.FORCED;
		cellStates[toPos] = CellState.FORCED;

		AStarGrid2D astar = new();
		astar.Region = new Rect2I(0, 0 , levelSizeX+5, levelSizeZ+5);
		astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
		astar.Update();
		List<(int x, int y)> witness = Astar(astar, fromRoom, toRoom, cellStates);
		while (openCells.Count != 0)
		{
			List<(int x, int y)> openPathCells = witness.FindAll(c => cellStates[c] == CellState.OPEN);

			float pathWeight = openPathCells.Count * wiggliness;
			float nonPathWeight = openCells.Count - openPathCells.Count;
			float totalWeight = pathWeight + nonPathWeight;

			(int x, int y) randomCell;
			if (rng.NextDouble() * totalWeight <= pathWeight)
			{
				randomCell = openPathCells[rng.Next(openPathCells.Count)];
			}
			else
			{
				List<(int x, int y)> nonPathOpenCells = openCells.FindAll(c => !witness.Contains(c));
				randomCell = nonPathOpenCells[rng.Next(openPathCells.Count)];
			}

			cellStates[randomCell] = CellState.BLOCKED;
			openCells.Remove(randomCell);

			if (witness.Contains(randomCell))
			{
				List<(int x, int y)> newPath = Astar(astar, fromRoom, toRoom, cellStates);
				if (!newPath.Any())
				{
					cellStates[randomCell] = CellState.FORCED;
				}
				else
				{
					witness = newPath;
				}
			}
		}
		return witness;
	}

	private List<(int x, int y)> Astar(AStarGrid2D astar, Room from, Room to, Dictionary<(int x, int y), CellState> cellStates)
	{
		for (int x = 0; x < levelSizeX + 5; x++)
		{
			for (int y = 0; y < levelSizeZ + 5; y++)
			{
				Room room = null;
				foreach (Room r in roomArray)
				{
					if (r.IsPointInside(new Vector2(x,y)))
					{
						room = r;
					}
				}
				if (cellStates[(x,y)] == CellState.BLOCKED || (room != null && room.Id != from.Id && room.Id != to.Id))
				{
					astar.SetPointSolid(new Vector2I(x, y), true);
				}
			}
		}
		return astar.GetPointPath(from.GridCoordinates, to.GridCoordinates).Select(v => ((int)v.X, (int)v.Y)).ToList();
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

				if (!checkActive && rng.NextDouble() > percentPaths)// && room.Edges.Count(e => e.Active) > 1)
				{
					edge.Active = false;
					toRoom.SetEdgeActive(room.Id, false);
				}

				Dfs(visited, toRoom, checkActive);
			}
		}
	}

	private void ShowEdges()
	{
		foreach (Room room in roomArray)
		{
			DebugDraw3D.DrawSphere(new Vector3(room.WorldPosition.X, 0, room.WorldPosition.Y), 0.5f, Colors.Blue);			
			foreach (Edge edge in room.Edges.FindAll(e => e.Active))
			{
				Room toRoom = roomArray.Single(r => r.Id == edge.RoomId);
				DebugDraw3D.DrawArrow(
					new Vector3(room.WorldPosition.X, 0, room.WorldPosition.Y),
					new Vector3(toRoom.WorldPosition.X, 0, toRoom.WorldPosition.Y),
					Colors.Yellow,
					0.05f
				);
			}
		}
	}

	private void SetRoomCells()
	{
		Clear();

		foreach (var room in roomArray)
		{
			for (int i = 0; i < room.Width; i++)
			{
				for (int j = 0; j < room.Height; j++)
				{
					SetCellItem(new Vector3I(room.GridCoordinates.X + i, 0, room.GridCoordinates.Y + j), 0);
				}
			}
		}
	}
}