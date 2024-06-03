using Godot;
using System;

public partial class TextAndBar : VBoxContainer
{
	private Label label;
	private ProgressBar progressBar;
	private string valueType = "HP";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		progressBar = GetNode<ProgressBar>("ProgressBar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		label.Text = $"{progressBar.Value} {valueType}";
	}

	public void Initialize(float maxValue, string type)
	{
		progressBar.MaxValue = maxValue;
		progressBar.Value = Math.Round(maxValue);
		valueType = type;
	}

	public void SetValue(float value)
	{
		progressBar.Value = Math.Round(value);
	}
}
