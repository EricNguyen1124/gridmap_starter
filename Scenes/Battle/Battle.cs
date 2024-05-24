using Godot;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Classes.Combatant;
using Scenes.Enemy;

public partial class Battle : Node3D
{
	private Control battleUI;
	private List<ICombatant> combatants = new();
	private ICombatant currentCombatant;

	enum BATTLE_STATE {TURN_IN_PROGRESS, TURN_ENDED, TURN_STARTED}
	private BATTLE_STATE currentState = BATTLE_STATE.TURN_STARTED;
	public override void _Ready()
	{
		battleUI = GetNode<Control>("Control");
		battleUI.Visible = false;
		var player = new Combatant
		{
			CombatantName = "jim",
			Speed = 5.0f,
			Health = 5.0f,
			Mana = 4.0f,
			Attack = 1.0f,
			PlayerControlled = true,
			Skills = new() {
				SkillList.AllSkills.First()
			}
		};

		var enemy = new Enemy
		{
			CombatantName = "enemy",
			Speed = 8.0f,
			Health = 3.0f,
			Mana = 4.0f,
			Attack = 1.0f,
			PlayerControlled = false,
			Skills = new() {
				SkillList.AllSkills.First()
			},
			Behavior = ENEMY_BEHAVIOR.NORMAL
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
					// Take Player Turn (only if message received from UI)
					if (Input.IsActionJustPressed("ui_left"))
					{
						var enemy = combatants.First(c => c.CombatantName == "enemy");

						currentState = BATTLE_STATE.TURN_IN_PROGRESS;
						currentCombatant.TakeTurn(enemy, COMBATANT_COMMANDS.ATTACK);
						currentState = BATTLE_STATE.TURN_ENDED;
					}
				}
				else
				{
					// Take AI Turn

					var enemy = (Enemy)currentCombatant;
					(ICombatant target, COMBATANT_COMMANDS command, string specifier) = enemy.MakeTurnDecision(combatants);

					currentState = BATTLE_STATE.TURN_IN_PROGRESS;
					currentCombatant.TakeTurn(target, command, specifier);
					currentState = BATTLE_STATE.TURN_ENDED;
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

	private ICombatant FindNextCombatant()
	{
		combatants = combatants.OrderByDescending(c => c.Speed).ToList();
		ICombatant nextCombatant = combatants.FirstOrDefault(c => c.Speed < currentCombatant.Speed);
		// tie break if speed is equal to current
		if (nextCombatant == null)
		{
			return combatants.First();
		}

		return nextCombatant;
	}	
}
