using afterMe.scripts.Objetos.Button;
using Godot;
using System;

namespace afterMe.scripts.Objetos.Gate;
public partial class LevelGate : StaticBody2D
{
    private bool _isClosed = false;
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
    }
    private void OnButtonPressed(LevelButton button)
    {
        CloseGate();
    }

    private void OnButtonReleased(LevelButton button)
    {
        OpenGate();
    }

	private void CloseGate()
	{
	    _isClosed = true;
	    _sprite.Play("active");
	    _collision.SetDeferred("disabled", false);  // en vez de _collision.Disabled = true
	}

	private void OpenGate()
	{
	    _isClosed = false;
	    _sprite.Play("desactive");
	    _collision.SetDeferred("disabled", true);   // en vez de _collision.Disabled = true
	}

}