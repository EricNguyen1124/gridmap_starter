using Godot;
using System;
using System.Collections.Generic;

namespace Scenes.Room
{
	public partial class Room : Node3D
	{
		public int Id;
		public Vector2I GridCoordinates;
		public Vector2 WorldPosition;
		public int Width;
		public int Height;
		public List<Edge> Edges = new();

		private const int minRoomSizeX = 3;
		private const int maxRoomSizeX = 4;
		private const int minRoomSizeZ = 3;
		private const int maxRoomSizeZ = 4;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public void SetRoomPosAndSize(int maxX, int maxZ)
		{
			RandomNumberGenerator rng = new();
			rng.Randomize();
			GridCoordinates = new Vector2I(rng.RandiRange(0, maxX), rng.RandiRange(0,maxZ));
			Width = rng.RandiRange(minRoomSizeX, maxRoomSizeX);
			Height = rng.RandiRange(minRoomSizeZ, maxRoomSizeZ);
			WorldPosition = new Vector2((float)(GridCoordinates.X + Width/2.0f)*2.0f, (float)(GridCoordinates.Y + Height/2.0f)*2.0f);
		}

		public bool CheckRoomCollide(Room room)
		{
			int left = GridCoordinates.X;
			int right = GridCoordinates.X + Width;
			int up = GridCoordinates.Y;
			int down = GridCoordinates.Y + Height;

			int left2 = room.GridCoordinates.X;
			int right2 = room.GridCoordinates.X + room.Width;
			int up2 = room.GridCoordinates.Y;
			int down2 = room.GridCoordinates.Y + room.Height;

			if (right + 2 < left2 || left - 2 > right2 || up - 2 > down2 || down + 2 < up2)
				return false;

			return true;
		}

		public bool IsPointInside(Vector2 point)
		{
			if (point.X < GridCoordinates.X || point.X > GridCoordinates.X + Width || point.Y < GridCoordinates.Y || point.Y > GridCoordinates.Y + Height)
				return false;
			return true;
		}

		public void SetEdgeActive(int roomId, bool active)
		{
			foreach(var edge in Edges)
			{
				if (edge.RoomId == roomId)
				{
					edge.Active = active;
					return;
				}
			}
			GD.Print("EDGE NOT FOUND");
		}
	}


	public class Edge
	{
		public bool Active = true;
		public int RoomId;
		
		public Edge(int id)
		{
			RoomId = id;
		}
    }
}