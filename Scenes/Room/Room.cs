using Godot;
using System;
using System.Collections.Generic;

namespace Scenes.Room
{
	public partial class Room : Node3D
	{
		public int Id;
		public Vector2I position;
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
			position = new Vector2I(rng.RandiRange(0, maxX), rng.RandiRange(0,maxZ));
			Width = rng.RandiRange(minRoomSizeX, maxRoomSizeX);
			Height = rng.RandiRange(minRoomSizeZ, maxRoomSizeZ);
			var iW = Width-1 / 2;
			var iH = Height-1 / 2;
			WorldPosition = new Vector2((float)(position.X + iW + 0.5)*2.0f, (float)(position.Y + iH + 0.5)*2.0f);
		}

		public bool CheckRoomCollide(Room room)
		{
			int left = position.X;
			int right = position.X + Width;
			int up = position.Y;
			int down = position.Y + Height;

			int left2 = room.position.X;
			int right2 = room.position.X + room.Width;
			int up2 = room.position.Y;
			int down2 = room.position.Y + room.Height;

			if (right + 2 < left2 || left - 2 > right2 || up - 2 > down2 || down + 2 < up2)
				return false;

			return true;
		}

		public bool IsPointInside(Vector2 point)
		{
			if (point.X < position.X || point.X > position.X + Width || point.Y < position.Y || point.Y > position.Y + Height)
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