using Godot;
using System.Collections.Generic;
using afterMe.scripts.Objetos.Button;
using afterMe.scripts.Prota;
using afterMe.scripts.Ghost;

namespace afterMe.scripts.Objetos.Door;

public partial class Door : Area2D
{
    private string _nextScene;
    private bool _isOpen = false;

    private AnimatedSprite2D _sprite;
    private CollisionShape2D _collision;

    private List<LevelButton> _buttons = new();
    private int _pressedCount = 0;

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _collision = GetNode<CollisionShape2D>("CollisionShape2D");

        // Calcula la siguiente escena igual que antes
        string scenePath = GetTree().CurrentScene.SceneFilePath;
        string currentScene = scenePath.GetFile().GetBaseName();
        int currentLevel = int.Parse(currentScene.Replace("Level", ""));
        _nextScene = "Level" + (currentLevel + 1);

        // Busca todos los botones del nivel y se suscribe a sus señales
        RegisterButtons();

        // Si no hay botones en el nivel, la puerta abre directamente
        if (_buttons.Count == 0)
            OpenDoor();
        else
            CloseDoor();
    }

    private void RegisterButtons()
    {
        // Busca todos los nodos LevelButton en la escena
        var allButtons = GetTree().GetNodesInGroup("level_buttons");
        foreach (Node node in allButtons)
        {
            if (node is LevelButton btn)
            {
                _buttons.Add(btn);
                btn.ButtonPressed  += OnButtonPressed;
                btn.ButtonReleased += OnButtonReleased;
            }
        }
    }

    private void OnButtonPressed(LevelButton button)
    {
        _pressedCount++;
        if (_pressedCount == _buttons.Count)
            OpenDoor();
    }

    private void OnButtonReleased(LevelButton button)
    {
        _pressedCount--;
        if (_pressedCount < _buttons.Count)
            CloseDoor();
    }

	private void OpenDoor()
	{
	    _isOpen = true;
	    _sprite.Play("open");
	    _collision.SetDeferred("disabled", false);  // en vez de _collision.Disabled = false
	}

	private void CloseDoor()
	{
	    _isOpen = false;
	    _sprite.Play("close");
	    _collision.SetDeferred("disabled", true);   // en vez de _collision.Disabled = true
	}

	    public void OnBodyEntered(Node body)
    {
        if (_isOpen && body is Prota.Prota)
            CallDeferred(MethodName.ChangeScene);
    }

    private void ChangeScene()
{
    // Limpia las grabaciones al cambiar de nivel
    var ghostMemory = GetNode<GhostMemory>("/root/GhostMemory");
    ghostMemory.Clear();

    GetTree().ChangeSceneToFile("res://scenes/Maps/" + _nextScene + ".tscn");
}
}