using Godot;
using System;

public class Menu : Spatial
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	private void _on_Barrel_Levels_pressed()
	{
		GetNode<AudioStreamPlayer>("ClickMenuButton").Play();
		var newLevelScene = (PackedScene)ResourceLoader.Load("res://Assets/Main.tscn");
		var LevelScene = newLevelScene.Instance() as Main;
		LevelScene.currentLevel = 1;
		LevelScene.availableLevels = 2;
		LevelScene.currentGamemode = "Barrel Levels";

		GetParent().AddChild(LevelScene);
		QueueFree();
	}

	private void _on_Mob_Levels_pressed()
	{
		GetNode<AudioStreamPlayer>("ClickMenuButton").Play();
		var newLevelScene = (PackedScene)ResourceLoader.Load("res://Assets/Main.tscn");
		var LevelScene = newLevelScene.Instance() as Main;
		LevelScene.currentLevel = 1;
		LevelScene.availableLevels = 2;
		LevelScene.currentGamemode = "Mob Levels";

		GetParent().AddChild(LevelScene);
		QueueFree();
	}

	private void _on_Exit_pressed()
	{
		GetTree().Quit();
	}
}
