using Godot;

namespace afterMe.scripts.Objetos.Button;

public partial class LevelButton : Area2D
{
    [Signal] public delegate void ButtonPressedEventHandler(LevelButton button);
    [Signal] public delegate void ButtonReleasedEventHandler(LevelButton button);

    public bool IsPressed { get; private set; } = false;

    private AnimatedSprite2D _sprite;
    private Node2D _ocupado = null;

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public void OnBodyEntered(Node2D node)
    {
        if (IsPressed) return;
        _ocupado = node;
        IsPressed = true;
        _sprite.Play("press");
        EmitSignal(SignalName.ButtonPressed, this);
    }

    public void OnBodyExited(Node2D node)
    {
        if (!IsPressed && _ocupado != node) return;
        IsPressed = false;
        _sprite.Play("unpress");
        _ocupado = null;
        EmitSignal(SignalName.ButtonReleased, this);
    }
}