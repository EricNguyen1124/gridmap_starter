using Godot;
using System;
using System.Collections.Generic;

namespace Scenes.Room
{
	public partial class Room : Node3D
	{
		public int Id;
		public Vector2I position;
		public Vector2 worldPosition;
		public int width;
		public int height;
		public List<Edge> edges;

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
			width = rng.RandiRange(minRoomSizeX, maxRoomSizeX);
			height = rng.RandiRange(minRoomSizeZ, maxRoomSizeZ);
			var iW = width-1 / 2;
			var iH = height-1 / 2;
			worldPosition = new Vector2((float)(position.X + iW + 0.5), (float)(position.Y + iH + 0.5));
		}

		public bool CheckRoomCollide(Room room)
		{
			int left = position.X;
			int right = position.X + width;
			int up = position.Y;
			int down = position.Y + height;

			int left2 = room.position.X;
			int right2 = room.position.X + room.width;
			int up2 = room.position.Y;
			int down2 = room.position.Y + room.height;

			if (right + 2 < left2 || left - 2 > right2 || up - 2 > down2 || down + 2 < up2)
				return false;

			return true;
		}

		public bool IsPointInside(Vector2 point)
		{
			if (point.X < position.X || point.X > position.X + width || point.Y < position.Y || point.Y > position.Y + height)
				return false;
			return true;
		}

		public void SetEdgeActive(int roomId, bool active)
		{
			foreach(var edge in edges)
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