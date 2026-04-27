using Godot;
using afterMe.scripts.FrameInput;
using afterMe.scripts.Ghost;

namespace afterMe.scripts.Prota;

public partial class Prota : CharacterBody2D
{
    [Export] public float Speed = 200f;
    [Export] public float JumpVelocity = -400.0f;
    [Export] public float Gravity = 980f;
    [Export] public float RespawnTime = 10.0f;
    [Export] public float ActionDelay = 0.5f;
    [Export] public int maxGhosts = 2;
    [Export] public PackedScene GhostScene;

    [Signal] public delegate void PlayerStartedMovingEventHandler();

    private bool _reseteable = false;
    private bool _canMove = false;
    private bool _movingSignalEmitted = false;
    public bool levelCleared = false;

    private Vector2 _startPosition;
    private Vector2 _velocity = Vector2.Zero;
    private InputRecorder _recorder;
    private Node _ghostContainer;
    private SceneTreeTimer _respawnTimer;
    private GhostMemory _ghostMemory;

    public override void _Ready()
    {
        _recorder = new InputRecorder();
        AddChild(_recorder);

        _ghostContainer = GetTree().Root.FindChild("GhostContainer", true, false)
                          ?? GetParent();

        _ghostMemory = GetNode<GhostMemory>("/root/GhostMemory");

        _startPosition = GlobalPosition;

        // Recrea los ghosts del run anterior al entrar al nivel
        RespawnAllGhosts();

        StartActionTimer();
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        if (!IsOnFloor())
            _velocity.Y += Gravity * dt;

        if (_canMove)
        {
            _recorder.RecordFrame(dt);

            // Emite la señal solo una vez cuando arranca la grabación
            if (_recorder.IsRecording && !_movingSignalEmitted)
            {
                _movingSignalEmitted = true;
                EmitSignal(SignalName.PlayerStartedMoving);
            }

            if (!_reseteable &&
                (Input.IsActionPressed("ui_accept") ||
                 Input.IsActionPressed("ui_left") ||
                 Input.IsActionPressed("ui_right")))
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

        bool timerDone = _reseteable && _respawnTimer != null && _respawnTimer.TimeLeft <= 0;
        bool resetPressed = _reseteable && Input.IsActionJustPressed("Reset");

        if (resetPressed || timerDone)
        {
            // Guarda la grabación actual en el singleton antes de resetear
            if (_recorder.IsRecording)
                _ghostMemory.AddRecording(_recorder.GetRecordingCopy(), _startPosition);
            if (maxGhosts > 0)
                ResetPlayer();
        }
    }

    // Destruye todos los ghosts actuales y los recrea desde GhostMemory
    private void RespawnAllGhosts()
    {
        // Elimina ghosts existentes
        foreach (Node child in _ghostContainer.GetChildren())
            child.QueueFree();

        // Recrea uno por cada grabación guardada
        foreach (var (frames, startPos) in _ghostMemory.Recordings)
        {
            var ghost = GhostScene.Instantiate<GhostController>();
            ghost.Initialize(frames, startPos);
            PlayerStartedMoving += ghost.Start;
            _ghostContainer.AddChild(ghost);
        }
    }

    private void StartRespawnTimer()
    {
        _respawnTimer = GetTree().CreateTimer(RespawnTime);
    }

    private void StartActionTimer()
    {
        _canMove = false;
        _movingSignalEmitted = false;
        var timer = GetTree().CreateTimer(ActionDelay);
        timer.Timeout += () => _canMove = true;
    }

    private void ResetPlayer()
    {
        maxGhosts = Mathf.Max(0, maxGhosts - 1); // Reduce el número de ghosts permitidos para el próximo run
        GlobalPosition = _startPosition;
        _velocity = Vector2.Zero;
        Velocity = Vector2.Zero;
        _reseteable = false;
        _respawnTimer = null;
        _recorder.Clear();

        // Destruye ghosts viejos y recrea todos con las grabaciones acumuladas
        RespawnAllGhosts();
        StartActionTimer();
    }

    public double GetRespawnTimeLeft()
    {
        if (_respawnTimer == null) return 0;
        return _respawnTimer.TimeLeft;
    }
    public int GetRemainingGhosts()
    {
        return maxGhosts;
    }
}