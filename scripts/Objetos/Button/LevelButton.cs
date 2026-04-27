using Godot;

namespace afterMe.scripts.Objetos.Button;

public partial class LevelButton : Area2D
{
    [Signal] public delegate void ButtonPressedEventHandler(LevelButton button);
    [Signal] public delegate void ButtonReleasedEventHandler(LevelButton button);

    public bool IsPressed { get; private set; } = false;

    private AnimatedSprite2D _sprite;
    private int _bodiesOnButton = 0; // cuenta cuántos cuerpos hay encima

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public void OnBodyEntered(Node2D body)
    {
        _bodiesOnButton++;

        if (!IsPressed)
        {
            IsPressed = true;
            _sprite.Play("press");
            EmitSignal(SignalName.ButtonPressed, this);
            GD.Print("Button pressed! Bodies on button: " + _bodiesOnButton);
        }
    }

    public void OnBodyExited(Node2D body)
    {
        _bodiesOnButton--;
        if (_bodiesOnButton == 0 && IsPressed)
        {
            IsPressed = false;
            _sprite.Play("unpress");
            EmitSignal(SignalName.ButtonReleased, this);
        }
    }
}