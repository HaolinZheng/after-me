using afterMe.scripts.Prota;
using Godot;
using System;

public partial class Counter : Control
{
	[Export] private Label _label;
	[Export] private Prota _prota;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_label.Text = _prota.GetRemainingGhosts().ToString();
	}
}
