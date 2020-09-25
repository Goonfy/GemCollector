using Godot;
using System;

public class Moving_Plataform : Spatial
{
	private Vector3 startPosition;
    private bool reverse = false;

    [Export]
    private int SPEED = 6;
    [Export]
    private int maximumHeight = 30;

    public override void _Ready()
	{
		startPosition = Translation;
    }

	public override void _PhysicsProcess(float delta)
	{
        //GD.Print((int)Translation.y);
        if (reverse == false)
        {
            Translate(GlobalTransform.basis.y.Normalized() * delta * SPEED);

            if ((int)Translation.y == maximumHeight + (int)startPosition.y)
			    reverse = true;
        }
        else
        {
            Translate(GlobalTransform.basis.y.Normalized() * delta * -SPEED);

            if ((int)Translation.y == (int)startPosition.y)
			    reverse = false;
        }
    }
}
