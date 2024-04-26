using Godot;
using System;

public partial class Player : Node3D
{
	enum PlayerState {STANDING, MOVING}

	private Vector2I gridPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gridPosition = new Vector2I(1,1);
		SetNewPosition(gridPosition);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
		if(@event.IsActionPressed("forward"))
		{
			gridPosition += new Vector2I(0,-1);
			SetNewPosition(gridPosition);
		}
		if(@event.IsActionPressed("back"))
		{
			gridPosition += new Vector2I(0,1);
			SetNewPosition(gridPosition);
		}
		if(@event.IsActionPressed("left"))
		{
			gridPosition += new Vector2I(-1,0);
			SetNewPosition(gridPosition);
		}
		if(@event.IsActionPressed("right"))
		{
			gridPosition += new Vector2I(1,0);
			SetNewPosition(gridPosition);
		}

		GD.Print(gridPosition);	
        //base._Input(@event);
    }

	public void SetNewPosition(Vector2I newGridPos)
	{
		var translatedPos = newGridPos * 2;
		GlobalPosition = new Vector3(translatedPos.X + 1, 0.5f, translatedPos.Y + 1);
	}
}
