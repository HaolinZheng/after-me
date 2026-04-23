using Godot;
using afterMe.scripts.FrameInput;
using afterMe.scripts.Ghost;
using System.Dynamic;
using System;

namespace afterMe.scripts.Prota;
public partial class Prota : CharacterBody2D
{
    [Export] public float Speed = 200f;
    [Export] public float JumpVelocity = -400.0f;
    [Export] public float Gravity = 980f;
    [Export] public float RespawnTime = 10.0f;  // tiempo para auto-reset
    [Export] public float ActionDelay = 0.5f;  // tiempo quieto tras reset
    [Export] public PackedScene GhostScene;
    
    [Signal] public delegate void PlayerStartedMovingEventHandler();

    private bool _reseteable = false;
    private bool _canMove = false;
    public bool levelCleared = false;
    private Vector2 _startPosition;
    private Vector2 _velocity = Vector2.Zero;
    private InputRecorder _recorder;
    private Node _ghostContainer;
    private SceneTreeTimer _respawnTimer;
    private SceneTreeTimer _actionTimer;

    public override void _Ready()
    {
        _recorder = new InputRecorder();
        AddChild(_recorder);

        _ghostContainer = GetTree().Root.FindChild("GhostContainer", true, false)
                          ?? GetParent();

        _startPosition = GlobalPosition;

        // Empieza sin poder moverse hasta que pase el ActionDelay inicial
        StartActionTimer();
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        // Gravedad siempre activa
        if (!IsOnFloor())
            _velocity.Y += Gravity * dt;

        if (_canMove)
        {
            _recorder.RecordFrame(dt); // detecta el primer input y graba en el mismo frame

            if (!_reseteable && (Input.IsActionPressed("ui_accept") || Input.IsActionPressed("ui_left") || Input.IsActionPressed("ui_right")))
            {
                _reseteable = true;
                StartRespawnTimer();
            }

            if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            _velocity.Y = JumpVelocity;

            _velocity.X = Input.GetAxis("ui_left", "ui_right") * Speed;
        }
        else
        {
            _velocity.X = 0f;
        }

        Velocity = _velocity;
        MoveAndSlide();
        _velocity = Velocity;

        // Reset por botón o por tiempo agotado
        bool timerDone = _reseteable && _respawnTimer != null && _respawnTimer.TimeLeft <= 0;
        bool resetPressed = _reseteable && Input.IsActionJustPressed("Reset");

        if (resetPressed || timerDone)
        {
            SpawnGhost();
            ResetPlayer();
        }
    }

    private void StartRespawnTimer()
    {
        _respawnTimer = GetTree().CreateTimer(RespawnTime);
    }

    private void StartActionTimer()
    {
        _canMove = false;
        _actionTimer = GetTree().CreateTimer(ActionDelay);
        _actionTimer.Timeout += () => _canMove = true;
    }

    private void SpawnGhost()
    {
        if (GhostScene == null)
        {
            return;
        }

        var ghost = GhostScene.Instantiate<GhostController>();
        ghost.Initialize(_recorder.GetRecordingCopy(), _startPosition);
        _ghostContainer.AddChild(ghost);
    }
    

    private void ResetPlayer()
    {
        GlobalPosition = _startPosition;
        _velocity = Vector2.Zero;
        Velocity = Vector2.Zero;
        _reseteable = false;
        _respawnTimer = null;
        _recorder.Clear();
        StartActionTimer();
    }
    public double GetRespawnTimeLeft()
    {
        if (_respawnTimer == null) return 0;
        return _respawnTimer.TimeLeft;
    }
}