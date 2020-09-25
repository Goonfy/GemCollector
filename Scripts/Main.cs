using Godot;
using System;

public class Main : Node
{
	public int currentLevel;
	public int availableLevels;

	public string currentGamemode = "Barrel Levels";

	private PackedScene newBarrelScene;

	public override void _Ready()
	{
		GD.Randomize();

		var newLevelScene = (PackedScene)ResourceLoader.Load("res://Levels/" + currentGamemode + "/Level" + currentLevel + ".tscn");
		var newLevel = newLevelScene.Instance();
		AddChild(newLevel);

		newBarrelScene = (PackedScene)ResourceLoader.Load("res://Assets/Barrel.tscn");
	}

	public override void _Process(float delta)
	{
		if (currentGamemode == "Barrel Levels" && GetNode<Timer>("barrel_timer").IsStopped())
		{
			GetNode<Timer>("barrel_timer").Start();
		}
		else if (currentGamemode != "Barrel Levels" && !GetNode<Timer>("barrel_timer").IsStopped())
		{
			GetNode<Timer>("barrel_timer").Stop();
		}
	}

	public void NextLevel()
	{
		if (currentLevel < availableLevels)
		{
			currentLevel += 1;
			GetChild(1).CallDeferred("free");

			var newLevelScene = (PackedScene)ResourceLoader.Load("res://Levels/" + currentGamemode + "/Level" + currentLevel + ".tscn");
			var newLevel = newLevelScene.Instance();
			AddChild(newLevel);
		}
		else
		{
			var newLevelScene = (PackedScene)ResourceLoader.Load("res://Assets/GameOver.tscn");
			var LevelScene = newLevelScene.Instance() as GameOver;
			LevelScene.playerWon = true;
			GetTree().Root.AddChild(LevelScene);

			Input.SetMouseMode(Input.MouseMode.Visible);

			GetTree().Root.GetChild(0).QueueFree();
		}
	}

	private void spawnBarrels()
	{
		/*
		var player = GetTree().GetNodesInGroup("player")[0] as KinematicBody;

		foreach (Spatial f in GetNode("Level" + currentLevel + "/Floors").GetChildren())
		{
			var floorMesh = f.GetNode("MeshInstance") as MeshInstance;
			var floorSize = floorMesh.GetAabb();

			if (player.Translation.z >= (f.Translation.z - (floorSize.Size.z / 2)) && player.Translation.z <= f.Translation.z + floorSize.Size.z)
			{
				var newBarrel = newBarrelScene.Instance() as RigidBody;

				var barrelPosition = new Vector3((f.Translation.x - (floorSize.Size.x / 2)) + (int)(GD.Randi() % floorSize.Size.x), f.Translation.y + 20, f.Translation.z);

				newBarrel.Translation = barrelPosition;
				GetNode("Level" + currentLevel).AddChild(newBarrel);
			}
		}
		*/
		var floorList = GetNode("Level" + currentLevel + "/Floors").GetChildren();
		var floor = floorList[(int)(GD.Randi() % floorList.Count)] as Spatial;

		var floorMesh = floor.GetNode("MeshInstance") as MeshInstance;
		var floorSize = floorMesh.GetAabb();

		var newBarrel = newBarrelScene.Instance() as RigidBody;

		var barrelPosition = new Vector3((floor.Translation.x - (floorSize.Size.x / 2)) + (int)(GD.Randi() % floorSize.Size.x), floor.Translation.y + 20, floor.Translation.z);

		newBarrel.Translation = barrelPosition;
		GetNode("Level" + currentLevel).AddChild(newBarrel);
	}

	private void _on_barrel_timer_timeout()
	{
		spawnBarrels();
	}
}
