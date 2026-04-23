using Godot;
using System;
using afterMe.scripts.Objetos.Button;
using afterMe.scripts.Prota;
namespace afterMe.scripts.Objetos.Platform;
public partial class LevelPlatform : StaticBody2D
{
	[Export] private bool _goingUp = false;
	[Export] private LevelButton _button;
	[Export] private float _distance = 100f;
	[Export] private RayCast2D _topRay;
	[Export] private RayCast2D _bottomRay;
	[Export] private RayCast2D _leftRay;
	[Export] private RayCast2D _rightRay;
	private Prota.Prota _prota;
	private bool _isActive = false;
	private bool _retunning = false;
	public override void _Ready()
	{
	    if (_button != null)
	    {
	        _button.ButtonPressed += OnButtonPressed;
	        _button.ButtonReleased += OnButtonReleased;
	    }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_topRay.ForceRaycastUpdate();
		_bottomRay.ForceRaycastUpdate();
		_leftRay.ForceRaycastUpdate();
		_rightRay.ForceRaycastUpdate();
		if (_isActive)
		{
			MovingPlatform(delta);
		}
		if (_topRay.IsColliding() || _rightRay.IsColliding())
			_retunning = true;
		else if (_bottomRay.IsColliding() || _leftRay.IsColliding())
			_retunning = false;
	}
	private void OnButtonPressed(LevelButton button)
	{
	    _isActive = true;
	}

	private void OnButtonReleased(LevelButton button)
	{
    	_isActive = false;
	}
	private void MovingPlatform(double delta)
	{
		if (_goingUp)
		{
			if (_retunning)
			GlobalPosition += new Vector2(0, _distance) * (float)delta;
			else
			GlobalPosition += new Vector2(0, -_distance) * (float)delta;
		}
		else
		{
			if (_retunning)
			{
				GlobalPosition += new Vector2(-_distance, 0) * (float)delta;
				if (_prota != null)
					_prota.GlobalPosition += new Vector2(-_distance, 0) * (float)delta;
			}
			else
			{
				GlobalPosition += new Vector2(_distance, 0) * (float)delta;
				if (_prota != null)
					_prota.GlobalPosition += new Vector2(_distance, 0) * (float)delta;
			}
		}
	}
	private void OnBodyEntered(Node body)
	{
    	if (body is Prota.Prota prota)
    	    _prota = prota;
		GD.Print("Prota entró en la plataforma");
	}

	private void OnBodyExited(Node body)
	{
        if (body is Prota.Prota)
    	    _prota = null;
	}
}
