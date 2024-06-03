using Godot;
using Interfaces;
using System;

public partial class PartyMemberUI : PanelContainer
{
	private TextAndBar HPBar;
	private TextAndBar MPBar;
	private Label Label;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HPBar = GetNode<TextAndBar>("VSplitContainer/HSplitContainer/Bars/HP");
		MPBar = GetNode<TextAndBar>("VSplitContainer/HSplitContainer/Bars/MP");
		Label = GetNode<Label>("VSplitContainer/Label");
	}

	public void Initialize(ICombatant combatant)
	{
		HPBar.Initialize(combatant.MaxHealth, "HP");
		MPBar.Initialize(combatant.MaxMana, "MP");
		Label.Text = combatant.CombatantName;
	}
}