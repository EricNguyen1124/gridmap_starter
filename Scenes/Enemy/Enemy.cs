using Godot;
using Interfaces;
using System;
using System.Collections.Generic;

public class AIBehavior
{
	public Func<List<ICombatant>, (ICombatant, COMBATANT_COMMANDS, string)> MakeTurnDecision { get; set;}
}

public partial class Enemy : Node3D, ICombatant
{
    public AIBehavior Behavior { get; set; }

    public string CombatantName { get; set; }
    public float Health { get; set; }
    public float Mana { get; set; }
    public float Speed { get; set; }
    public float Attack { get; set; }
    public bool PlayerControlled { get; set; }
    public List<Skill> Skills { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
