using Godot;
using System.Collections.Generic;

namespace afterMe.scripts.FrameInput;
public struct FrameInput
{
    public float MoveDirection;
    public bool JumpPressed;
    public bool WasOnFloor;
    public float Delta;
}

public partial class InputRecorder : Node
{
    public List<FrameInput> RecordedFrames { get; private set; } = new();
    public bool IsRecording { get; private set; } = false; // empieza en false

    public void TryStartRecording()
    {
        if (!IsRecording)
            IsRecording = true;
    }

    public void RecordFrame(float delta)
{
    // Captura el salto SIEMPRE, aunque aún no grabe
    bool jumpThisFrame = Input.IsActionJustPressed("ui_accept");
    float moveThisFrame = Input.GetAxis("ui_left", "ui_right");

    // Activa la grabación si hay cualquier input
    if (!IsRecording && (jumpThisFrame || moveThisFrame != 0f))
        IsRecording = true;

    if (!IsRecording) return;

    RecordedFrames.Add(new FrameInput
    {
        MoveDirection = moveThisFrame,
        JumpPressed = jumpThisFrame,
        WasOnFloor = GetParent<CharacterBody2D>().IsOnFloor(),
        Delta = delta
    });
}

    public List<FrameInput> GetRecordingCopy() => [.. RecordedFrames];

    public void Clear()
    {
        RecordedFrames.Clear();
        IsRecording = false; // al limpiar vuelve a esperar el primer input
    }
}
