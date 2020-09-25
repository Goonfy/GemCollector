using Godot;
using System;

public class GameOver : CenterContainer
{
	public bool playerWon;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public override void _Process(float delta)
	{
		if (playerWon && GetNode<Label>("Won").Visible == false)
		{
			GetNode<Label>("Won").Visible = true;
			GetNode<AudioStreamPlayer>("Won_sound").Play();
		}
		else if (!playerWon && GetNode<Label>("GameOver").Visible == false)
		{
			GetNode<Label>("GameOver").Visible = true;
			GetNode<AudioStreamPlayer>("GameOver_sound").Play();
		}
	}

	private void _on_Timer_timeout()
	{
		var newLevelScene = (PackedScene)ResourceLoader.Load("res://Assets/Menu.tscn");
		var LevelScene = newLevelScene.Instance();
		GetTree().Root.AddChild(LevelScene);

		Input.SetMouseMode(Input.MouseMode.Visible);

		GetTree().Root.GetChild(0).QueueFree();
	}
}
