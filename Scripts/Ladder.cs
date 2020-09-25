using Godot;
using System;

public class Ladder : StaticBody
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	
	private void _on_Area_body_entered(object body)
	{
		var b = body as Player;
		b.isClimbing = true;
	}
	
	private void _on_Area_body_exited(object body)
	{
		var b = body as Player;
		b.isClimbing = false;
	}
}
