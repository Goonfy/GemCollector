using Godot;
using System;

public class Barrel : RigidBody
{
	[Export]
	private bool isInMenu = false;

	private void _on_Area_body_entered(object body)
	{
		if (!isInMenu)
		{
			var b = body as Player;
			b.Damage(1);
			GetNode<AudioStreamPlayer>("Hit_sound").Play();
		}
	}

	private void _on_Timer_timeout()
	{
		if (!isInMenu)
		{
			QueueFree();
		}
	}
}
