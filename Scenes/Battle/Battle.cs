using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Battle : Node3D
{
	private Control battleUI;

	private List<Combatant> combatants = new();
	private Combatant currentCombatant;


	enum BATTLE_STATE {TURN_IN_PROGRESS, TURN_ENDED, TURN_STARTED}
	private BATTLE_STATE currentState = BATTLE_STATE.TURN_STARTED;
	public override void _Ready()
	{
		battleUI = GetNode<Control>("Control");
		battleUI.Visible = false;
		var player = new Combatant
		{
			Name = "jim",
			Speed = 5.0f,
			Health = 5.0f,
			Mana = 4.0f,
			Attack = 1.0f,
			PlayerControlled = true,
			skills = new() {
				SkillList.AllSkills.First()
			}
		};

		var enemy = new Combatant
		{
			Name = "enemy",
			Speed = 8.0f,
			Health = 3.0f,
			Mana = 4.0f,
			Attack = 1.0f,
			PlayerControlled = false,
			skills = new() {
				SkillList.AllSkills.First()
			}
		};

		combatants.Add(player);
		combatants.Add(enemy);
		
		currentCombatant = combatants.MaxBy(c => c.Speed);
	}

	public override void _Process(double delta)
	{
		switch (currentState)
		{
			case BATTLE_STATE.TURN_STARTED:
				if (currentCombatant.PlayerControlled)
				{
					if (Input.IsActionJustPressed("ui_left"))
					{
						TakePlayerTurn();
					}
				}
				else
				{
					// Battle class should ask Combatant for turn, while giving the Combatant the current state of the battle
					TakeEnemyTurn();
				}
				break;

			case BATTLE_STATE.TURN_ENDED:
				currentCombatant = FindNextCombatant();
				battleUI.Visible = currentCombatant.PlayerControlled;
				currentState = BATTLE_STATE.TURN_STARTED;
				break;

			case BATTLE_STATE.TURN_IN_PROGRESS:
			default:
				break;
		}

		if (!combatants.Any(c => !c.PlayerControlled && c.Health > 0))
		{
			GD.Print("Battle Done!");
		}
	}

	private async void TakePlayerTurn()
	{
		GD.Print("player turn!");
		currentState = BATTLE_STATE.TURN_IN_PROGRESS;
		var enemy = combatants.First(c => c.Name == "enemy");
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		enemy.Health -= 1.0f;
		GD.Print(enemy.Health);
		currentState = BATTLE_STATE.TURN_ENDED;
	}

	private async void TakeEnemyTurn()
	{
		GD.Print("enemy turn!");
		currentState = BATTLE_STATE.TURN_IN_PROGRESS;
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		var player = combatants.First(c => c.Name == "jim");
		player.Health -= 1.0f;
		GD.Print(player.Health);
		currentState = BATTLE_STATE.TURN_ENDED;
	}

	private Combatant FindNextCombatant()
	{
		combatants = combatants.OrderByDescending(c => c.Speed).ToList();
		Combatant nextCombatant = combatants.FirstOrDefault(c => c.Speed < currentCombatant.Speed);
		// tie break if speed is equal to current
		if (nextCombatant == null)
		{
			return combatants.First();
		}

		return nextCombatant;
	}	
}
