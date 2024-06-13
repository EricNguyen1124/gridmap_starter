using Classes.Database;
using Godot;
using Interfaces;
using Scenes.Enemy;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerUI : PanelContainer
{
	private Button attackButton;
	private MenuButton skillsMenu;
	private Sprite3D targeter;

	private List<Enemy> enemies;
	private Enemy currentTargetEnemy;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		attackButton = GetNode<Button>("VBoxContainer/AttackButton");
		skillsMenu = GetNode<MenuButton>("VBoxContainer/SkillsMenu");
		targeter = GetNode<Sprite3D>("Targeter");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		targeter.Position = currentTargetEnemy.Position;
		if (Input.IsActionPressed("left"))
		{
			currentTargetEnemy = enemies[enemies.IndexOf(currentTargetEnemy) + 1 % enemies.Count];
		}
		if (Input.IsActionPressed("right"))
		{
			currentTargetEnemy = enemies[enemies.IndexOf(currentTargetEnemy) - 1 % enemies.Count];
		}
	}

	public void Initialize(ICombatant player, List<Enemy> targetableEnemies)
	{
		SetProcess(true);
		Visible = true;

		var popup = skillsMenu.GetPopup();
		popup.Clear(true);
		foreach (var skill in player.Skills)
		{
			popup.AddItem($"{skill.Name}  {skill.Cost}MP");
		}

		enemies = targetableEnemies;
		currentTargetEnemy = enemies.First();
	}

	public void AttackButtonPressed()
	{
		
	}
}
