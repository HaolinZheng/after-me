using afterMe.scripts.Objetos.Button;
using Godot;
using System;

namespace afterMe.scripts.Objetos.Gate;
public partial class LevelGate : StaticBody2D
{
    [Export] private bool _isDisabled = false;
    [Export] private AnimatedSprite2D _sprite;
    [Export] private CollisionShape2D _collision;
	[Export] private LevelButton _button;
    public override void _Ready()
    {
		if (_button != null)
	    {
	        _button.ButtonPressed += OnButtonPressed;
	        _button.ButtonReleased += OnButtonReleased;
	    }
		if (!_isDisabled)
		{
			_sprite.Play("active");
	    	_collision.SetDeferred("disabled", false);  // en vez de _collision.Disabled = true
		}
	}
    private void OnButtonPressed(LevelButton button)
    {
        ActiveGate();
    }

    private void OnButtonReleased(LevelButton button)
    {
        DesactiveGate();
    }

	private void ActiveGate()
	{
	    _isDisabled = !_isDisabled;
	    _sprite.Play("active");
	    _collision.SetDeferred("disabled", false);  // en vez de _collision.Disabled = true
	}

	private void DesactiveGate()
	{
	    _isDisabled = !_isDisabled;
	    _sprite.Play("desactive");
	    _collision.SetDeferred("disabled", true);   // en vez de _collision.Disabled = true
	}

}