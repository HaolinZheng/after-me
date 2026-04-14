using Godot;

public partial class PlayerController : CharacterBody2D
{
    [Export] public float Speed = 200f;
    [Export] public PackedScene GhostScene;

    private InputRecorder _recorder;
    private Node _ghostContainer;

    public override void _Ready()
    {
        _recorder = new InputRecorder();
        AddChild(_recorder);

        // Busca o crea el contenedor de ghosts en el nivel
        _ghostContainer = GetTree().Root.FindChild("GhostContainer", true, false)
                          ?? GetParent();
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        // Graba el frame antes de mover
        _recorder.RecordFrame(dt);

        // Movimiento normal del jugador
        Vector2 dir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Velocity = dir * Speed;
        MoveAndSlide();

        // Reset: crea ghost y reinicia
        if (Input.IsActionJustPressed("ui_cancel")) // Cambia "ui_cancel" por tu botón de reset
        {
            SpawnGhost();
            ResetPlayer();
        }
    }

    private void SpawnGhost()
    {
        if (GhostScene == null)
        {
            GD.PrintErr("Asigna GhostScene en el inspector del Player.");
            return;
        }

        var ghost = GhostScene.Instantiate<GhostController>();
        ghost.Initialize(_recorder.GetRecordingCopy(), GlobalPosition);
        _ghostContainer.AddChild(ghost);
    }

    private void ResetPlayer()
    {
        // Vuelve al origen o al spawn point que prefieras
        GlobalPosition = Vector2.Zero;
        Velocity = Vector2.Zero;
        _recorder.Clear();
    }
}