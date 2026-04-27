using Godot;
using System.Collections.Generic;
using afterMe.scripts.FrameInput;
namespace afterMe.scripts.Ghost;
public partial class GhostController : CharacterBody2D
{
    [Export] public float Speed = 200f;
    [Export] public float JumpForce = -400f;
    [Export] public float Gravity = 980f;

    private List<FrameInput.FrameInput> _frames;
    private int _frameIndex = 0;
    private Vector2 _startPosition;
    private bool _finished = false;
    private bool _started = false;

    // Estado interno de física del ghost
    private Vector2 _velocity = Vector2.Zero;

    public void Initialize(List<FrameInput.FrameInput> frames, Vector2 startPosition)
    {
        _frames = frames;
        _startPosition = startPosition;
    }

    public override void _Ready()
    {
        GlobalPosition = _startPosition;
        _velocity = Vector2.Zero;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_started) return;

        if (_finished || _frames == null || _frameIndex >= _frames.Count)
        {
            _finished = true;
            return;
        }

        FrameInput.FrameInput frame = _frames[_frameIndex++];
        float dt = frame.Delta;

        if (!IsOnFloor())
            _velocity.Y += Gravity * dt;

        if (frame.JumpPressed && frame.WasOnFloor)
            _velocity.Y = JumpForce;

        _velocity.X = frame.MoveDirection * Speed;

        Velocity = _velocity;
        MoveAndSlide();
        _velocity = Velocity;
    }
    public void Start()
    {
        _started = true;
    }
}