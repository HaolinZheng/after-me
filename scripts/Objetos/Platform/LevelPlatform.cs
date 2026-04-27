using Godot;
using System.Collections.Generic;
using afterMe.scripts.Objetos.Button;
using afterMe.scripts.Prota;
using afterMe.scripts.Ghost;

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

    private bool _isActive = false;
    private bool _retunning = false;
    private Vector2 _startPosition;

    // Lista de todos los cuerpos encima — Prota y GhostController
    private List<CharacterBody2D> _bodiesOnPlatform = new();

    public override void _Ready()
    {
        if (_button != null)
        {
            _button.ButtonPressed  += OnButtonPressed;
            _button.ButtonReleased += OnButtonReleased;
        }
        _startPosition = GlobalPosition;
    }

    public override void _Process(double delta)
    {
        _topRay.ForceRaycastUpdate();
        _bottomRay.ForceRaycastUpdate();
        _leftRay.ForceRaycastUpdate();
        _rightRay.ForceRaycastUpdate();

        if (_isActive)
            MovingPlatform(delta);

        if ((_topRay.IsColliding() && _goingUp) || (_rightRay.IsColliding() && !_goingUp))
            _retunning = true;
        else if ((_bottomRay.IsColliding() && _goingUp) || _leftRay.IsColliding())
            _retunning = false;
    }

    private void OnButtonPressed(LevelButton button)  => _isActive = true;
    private void OnButtonReleased(LevelButton button) => _isActive = false;

    private void MovingPlatform(double delta)
    {
        Vector2 movement = Vector2.Zero;

        if (_goingUp)
        {
            movement = _retunning
                ? new Vector2(0,  _distance) * (float)delta
                : new Vector2(0, -_distance) * (float)delta;
        }
        else
        {
            movement = _retunning
                ? new Vector2(-_distance, 0) * (float)delta
                : new Vector2( _distance, 0) * (float)delta;
        }

        GlobalPosition += movement;

        // Mueve todos los cuerpos encima (Prota y ghosts)
        // Solo en horizontal — en vertical la gravedad/colisión lo gestiona Godot
        if (!_goingUp)
        {
            foreach (var body in _bodiesOnPlatform)
            {
                if (IsInstanceValid(body))
                    body.GlobalPosition += movement;
            }
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Prota.Prota prota)
            _bodiesOnPlatform.Add(prota);
        else if (body is GhostController ghost)
            _bodiesOnPlatform.Add(ghost);
    }

    private void OnBodyExited(Node body)
    {
        if (body is CharacterBody2D character)
            _bodiesOnPlatform.Remove(character);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Reset"))
        {
            GlobalPosition = _startPosition;
            _isActive = false;
            _retunning = false;
            _bodiesOnPlatform.Clear();
        }
    }
}