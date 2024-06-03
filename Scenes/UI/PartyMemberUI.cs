using Godot;
using Interfaces;
using System;

public partial class PartyMemberUI : PanelContainer
{
	private TextAndBar HPBar;
	private TextAndBar MPBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var idk = GetNode<VSplitContainer>("VSplitContainer/HSplitContainer/Bars");
		HPBar = GetNode<TextAndBar>("VSplitContainer/HSplitContainer/Bars/HP");
		MPBar = GetNode<TextAndBar>("VSplitContainer/HSplitContainer/Bars/MP");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Initialize(ICombatant combatant)
	{
		HPBar.Initialize(combatant.MaxHealth, "HP");
		MPBar.Initialize(combatant.MaxMana, "MP");
	}
}