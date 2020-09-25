using Godot;
using System;

public class Player : KinematicBody
{	
	[Export]
	public float Gravity = -40.8f;
	[Export]
	public float MaxSpeed = 16.0f;
	[Export]
	public float JumpSpeed = 18.0f;
	[Export]
	public float Accel = 2.0f;
	[Export]
	public float Deaccel = 7.0f;
    [Export]
    public float MaxSlopeAngle = 40.0f;

    [Export]
	public float MaxSprintSpeed = 30.0f;
	[Export]
	public float SprintAccel = 18.0f;

    public bool isClimbing = false;

	private SpotLight flashlight;

	private Vector3 velocity = new Vector3();
	private Vector3 dir = new Vector3();
    private Vector3 hv;

	public Camera camera;

    private Spatial PlayerScene;

    private AnimationTree PlayerTreeAnimation;

	public int Health = 3;

	public RayCast look_raycast;

	private bool isMoving;
    private bool isJumping;

	public override void _Ready()
	{	
        camera = GetNode<Camera>("Camera");
		camera.SetAsToplevel(true);

        PlayerScene = GetNode<Spatial>("Rotation_Helper/Model/Robot");
        PlayerTreeAnimation = PlayerScene.GetNode<AnimationTree>("RobotArmature/AnimationTree");

        Input.SetMouseMode(Input.MouseMode.Captured);

		flashlight = GetNode<SpotLight>("Rotation_Helper/Flashlight");
	}

	public override void _PhysicsProcess(float delta)
	{
		ProcessInput();
		ProcessMovement(delta);
        ProcessAnimation();
        ProcessCamera();
		ProcessHUD();
	}

	private void ProcessInput()
	{
		//  -------------------------------------------------------------------
		//  Walking
		dir = new Vector3();

        isMoving = false;

		if (Input.IsActionPressed("movement_forward"))
		{
			//dir += -new Vector3(0,0,1);
            //isMoving = true;
		}
		if (Input.IsActionPressed("movement_backward"))
		{
			//dir += new Vector3(0,0,1);
            //isMoving = true;
		}
		if (Input.IsActionPressed("movement_left"))
		{
			dir += -new Vector3(1,0,0);
            isMoving = true;
		}
		if (Input.IsActionPressed("movement_right"))
		{
			dir += new Vector3(1,0,0);
            isMoving = true;
		}

		//  -------------------------------------------------------------------
		//  Jumping
		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("movement_jump"))
            {
                velocity.y = JumpSpeed;
                isJumping = true;
            }
		}
		//  -------------------------------------------------------------------

		//  -------------------------------------------------------------------
		//  Turning the flashlight on/off
		if (Input.IsActionJustPressed("flashlight"))
		{
			if (flashlight.IsVisibleInTree())
				flashlight.Hide();
			else
				flashlight.Show();
		}
		//  -------------------------------------------------------------------

		//  -------------------------------------------------------------------
		//  Capturing/Freeing the cursor
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (Input.GetMouseMode() == Input.MouseMode.Visible)
				Input.SetMouseMode(Input.MouseMode.Captured);
			else
				Input.SetMouseMode(Input.MouseMode.Visible);
		}
		//  -------------------------------------------------------------------
	}

	private void ProcessMovement(float delta)
	{
        dir.y = 0;
        dir = dir.Normalized();

        if (!isClimbing)
        {
            velocity.y += delta * Gravity;

            hv = velocity;
            hv.y = 0;

            var new_pos = dir * MaxSpeed;
            var accel = Deaccel;

            if (dir.Dot(hv) > 0)
                accel = Accel;

            hv = hv.LinearInterpolate(new_pos, accel * delta);

            velocity.x = hv.x;
            velocity.z = hv.z;

            if (isMoving)
            {
                var angle = Mathf.Atan2(-hv.x, -hv.z);

                var char_rot = Rotation;

                char_rot.y = angle;
                Rotation = char_rot;
            }
        }
        else
        {
            velocity.x = MaxSpeed * dir.x;
            velocity.z = 0;
            velocity.y = MaxSpeed * -dir.z;
        }

        velocity = MoveAndSlide(velocity, new Vector3(0, 1, 0), false, 4, Mathf.Deg2Rad(MaxSlopeAngle));

        if (isJumping && IsOnFloor())
            isJumping = false;

        if (Translation.y <= -50)
			KillPlayer();
	}

    private void ProcessAnimation()
    {
        if (!IsOnFloor() && !isJumping)
        {
            PlayerTreeAnimation.Set("parameters/Idle_Run/blend_amount", 0);
        }
        else
            PlayerTreeAnimation.Set("parameters/Idle_Run/blend_amount", hv.Length() / MaxSpeed);
    }

	private void ProcessCamera()
	{
        var target = new Vector3(GlobalTransform.origin.x, GlobalTransform.origin.y, 0);
        var pos = new Vector3(target.x, target.y + 10, 25);
        var up = new Vector3(0, 1, 0);
        camera.LookAtFromPosition(pos, target, up);
	}

	private void ProcessHUD()
	{
        if (Health == 2)
            GetNode<Sprite>("HUD/Heart3").Visible = false;
        else if (Health == 1)
            GetNode<Sprite>("HUD/Heart2").Visible = false;
    }

	public void Damage(int damage_value)
	{
		Health -= damage_value;
		if (Health <= 0)
			KillPlayer();
	}

	public void KillPlayer()
	{
        var newLevelScene = (PackedScene)ResourceLoader.Load("res://Assets/GameOver.tscn");
        var LevelScene = newLevelScene.Instance() as GameOver;
        LevelScene.playerWon = false;
        GetTree().Root.AddChild(LevelScene);

        Input.SetMouseMode(Input.MouseMode.Visible);

        GetTree().Root.GetChild(0).QueueFree();
    }
}