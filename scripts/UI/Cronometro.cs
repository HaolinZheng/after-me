using afterMe.scripts.Prota;
using Godot;
using System;

public partial class Cronometro : Control
{
	[Export] private Label _label;
	[Export] private Prota _prota;
	[Export] private int _maxTime;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		double timeLeft = Math.Round(_prota.GetRespawnTimeLeft(), 2);

	if (timeLeft > 0)
	{
    	TimeSpan t = TimeSpan.FromSeconds(timeLeft);
    	_label.Text = t.ToString(@"ss\:ff");
	}
	else
	{
    	_label.Text = $"{_maxTime}:00";
	}
	}
}
