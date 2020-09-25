using Godot;
using System;

public class Pickup_Gem : Area
{
	[Export]
	private int SPEED = 100;

	public override void _Process(float delta)
	{
		var rotation = RotationDegrees;
		rotation.y += SPEED * delta;

		RotationDegrees = rotation;
	}

	private void _on_Area_body_entered(object body)
	{
		var m = GetTree().Root.GetNode("Main") as Main;
		m.NextLevel();
	}
}
