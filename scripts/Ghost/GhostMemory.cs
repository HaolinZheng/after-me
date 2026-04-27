using Godot;
using System.Collections.Generic;
using afterMe.scripts.FrameInput;

namespace afterMe.scripts.Ghost;

public partial class GhostMemory : Node
{
    // Cada entrada es una grabación completa de un run anterior
    public List<(List<FrameInput.FrameInput> frames, Vector2 startPosition)> Recordings { get; private set; } = new();

    public void AddRecording(List<FrameInput.FrameInput> frames, Vector2 startPosition)
    {
        Recordings.Add((frames, startPosition));
    }

    public void Clear()
    {
        Recordings.Clear();
    }
}