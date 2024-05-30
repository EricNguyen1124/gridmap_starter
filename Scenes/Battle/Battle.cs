using Godot;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Classes.Combatant;
using Scenes.Enemy;
using Classes.Database;

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
            Speed = 6.0f,
            Health = 5.0f,
            Mana = 4.0f,
            Attack = 1.0f,
            PlayerControlled = true,
            Skills = new() {
                Database.SkillLibrary[SKILLS.STRIKE]
            }
        };

        Party.Members.Add(player);
        combatants.AddRange(Party.Members);

        var enemies = ChooseEnemiesFromLibrary();
        combatants.AddRange(enemies);
        SpawnEnemies(enemies);

        int id = 0;
        foreach (var combatant in combatants)
        {
            combatant.Id = id++;
        }

        currentCombatant = combatants.MaxBy(c => c.Speed);
    }

    private void SpawnEnemies(List<Enemy> enemies)
    {
        var spacing = 5.8f / (float)(enemies.Count + 1);
        var spawnPosition = -2.8f;

        foreach (Enemy enemy in enemies)
        {
            var enemyScene = GD.Load<PackedScene>("res://Scenes/Enemy/Enemy.tscn");
            var enemyInstance = (Enemy)enemyScene.Instantiate();
            enemyInstance.SetEnemyProperties(enemy);
            enemyInstance.Position = new Vector3(spawnPosition + spacing, 0, 0);
            AddChild(enemyInstance);
            spawnPosition += spacing;
        }
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
						var enemy = combatants.First(c => c.CombatantName == "Blob");

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

	private List<Enemy> ChooseEnemiesFromLibrary()
	{
		return new List<Enemy>() {
			Database.EnemyLibrary[ENEMIES.BLOB],
			Database.EnemyLibrary[ENEMIES.BLOB],
			Database.EnemyLibrary[ENEMIES.SKELETON],
		};
	}
}
