using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Player : Node3D
{
	private const float SPEED = 2.0f;

	enum PlayerState {IDLE, MOVING, TURNING, BATTLING}
	private PlayerState state = PlayerState.IDLE;
	public Vector2 gridPosition;
	public Vector2 newGridPosition;
	public Vector3 oldWorldPosition;
	public Vector3 newWorldPosition;
	private Vector3 oldAngle;
	private Vector3 newAngle;
	private float moveProgress = 0.0f;

	public override void _Ready()
	{
		oldAngle = RotationDegrees;
		//gridPosition = new Vector2(1,1);
		//SetNewPosition(Vector2.Zero);
	}

	public override void _Process(double delta)
	{
		switch(state)
		{
			case PlayerState.MOVING:
				moveProgress = Math.Clamp(moveProgress + (float)delta * SPEED, 0.0f, 1.0f);
				GlobalPosition = oldWorldPosition.Lerp(newWorldPosition, moveProgress);
				if (moveProgress == 1.0f)
				{
					moveProgress = 0.0f;
					oldWorldPosition = GlobalPosition;
					gridPosition = newGridPosition;
					state = PlayerState.IDLE;
				}
				break;
			case PlayerState.TURNING:
				moveProgress = Math.Clamp(moveProgress + (float)delta * SPEED * 2.0f, 0.0f, 1.0f);
				RotationDegrees = oldAngle.Lerp(newAngle, moveProgress);
				if (moveProgress == 1.0f)
				{
					moveProgress = 0.0f;
					oldAngle = RotationDegrees.Round();
					state = PlayerState.IDLE;
				}
				break;
			case PlayerState.IDLE:
				if (Input.IsActionPressed("forward"))
				{
					var directionOffset = new Vector2(0,-1);
					SetNewPosition(directionOffset.Rotated(-Rotation.Y));
				}
				if (Input.IsActionPressed("back"))
				{
					var directionOffset = new Vector2(0,1);
					SetNewPosition(directionOffset.Rotated(-Rotation.Y));
				}
				if (Input.IsActionPressed("left"))
				{
					var directionOffset = new Vector2(-1,0);
					SetNewPosition(directionOffset.Rotated(-Rotation.Y));
				}
				if (Input.IsActionPressed("right"))
				{
					var directionOffset = new Vector2(1,0);
					SetNewPosition(directionOffset.Rotated(-Rotation.Y));
				}
				if (Input.IsActionPressed("turn_left"))
				{
					SetNewRotation(90);
				}
				if (Input.IsActionPressed("turn_right"))
				{
					SetNewRotation(-90);
				}
				break;
		}
	}

    public override void _Input(InputEvent @event)
    {

    }

	private void SetNewPosition(Vector2 directionOffset)
	{
		state = PlayerState.MOVING;
		newGridPosition = gridPosition + directionOffset;
		var translatedPos = newGridPosition * 2;
		newWorldPosition = new Vector3(translatedPos.X + 1, 0.5f, translatedPos.Y + 1);
	}

	private void SetNewRotation(float angleOffset)
	{
		state = PlayerState.TURNING;
		newAngle = oldAngle + new Vector3(0, angleOffset, 0);
	}
}
