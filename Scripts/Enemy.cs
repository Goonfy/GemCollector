using Godot;
using System;

public class Enemy : KinematicBody
{
	public float Gravity = -60.8f;
	public float MaxSpeed = 15.0f;
	public float JumpSpeed = 10.0f;
	public float Accel = 4.5f;
	public float Deaccel = 16.0f;
	public float MaxSlopeAngle = 40.0f;

	public int Health = 50;
	private readonly int Damage = 1;
	private readonly float attack_rate = 1.0f;

	private Vector3 _vel = new Vector3();
	private Vector3 _dir = new Vector3();

	private Timer attack_timer;

	private RayCast floor_detector;

	private Node player_node;

	public override void _Ready()
	{
		GD.Randomize();

		attack_timer = GetNode<Timer>("AttackTimer");

		floor_detector = GetNode<RayCast>("Floor_Detector");

		attack_timer.WaitTime = attack_rate;

		int[] random_rotation = new int[] {90,-90};
		RotationDegrees = new Vector3(0,random_rotation[GD.Randi()%random_rotation.Length],0);
	}

	public override void _PhysicsProcess(float delta)
	{
		SetAi(delta);
	}

	private void SetAi(float delta)
	{
		if (!floor_detector.IsColliding())
		{
			_dir.x = 0;
			_dir.z = 0;
			RotationDegrees *= new Vector3(0,-1,0);
		}
		else
		{
			_dir += -GlobalTransform.basis.z;

			_dir.y = 0;
			_dir = _dir.Normalized();

			_vel.y += delta * Gravity;

			Vector3 hvel = _vel;
			hvel.y = 0;

			Vector3 target = _dir;

			target *= MaxSpeed;

			float accel;
			if (_dir.Dot(hvel) > 0)
				accel = Accel;
			else
				accel = Deaccel;

			hvel = hvel.LinearInterpolate(target, accel * delta);
			_vel.x = hvel.x;
			_vel.z = hvel.z;
			if (player_node == null)
				_vel = MoveAndSlide(_vel, new Vector3(0, 1, 0), false, 4, Mathf.Deg2Rad(MaxSlopeAngle));
		}

		if (Translation.y <= -50)
			KillEnemy();
	}

	public void Hit(int hit_value)
	{
		Health -= hit_value;
		if (Health <= 0)
			KillEnemy();
	}

	private void KillEnemy()
	{
		//spatial p;
		//if (gd.randi()%2 == 0)
		//{
		//	p = ammodropscene.instance() as spatial;
		//	p.translation = translation;
		//	gettree().root.calldeferred("add_child", p);
		//}
		//else
		//{
		//	p = healthdropscene.instance() as spatial;
		//	p.translation = translation;
		//	gettree().root.calldeferred("add_child", p);
		//}
		QueueFree();
	}

	private void Attack(Node body, bool is_attacking)
	{
		if (is_attacking)
		{
			GetNode<AudioStreamPlayer>("Hit_sound").Play();
			attack_timer.Start();
			player_node = body;
			var p = player_node as Player;
			p.Damage(Damage);
		}
		else
		{
			player_node = null;
			attack_timer.Stop();
		}
	}

	private void _on_Hit_Area_body_entered(Node body)
	{
		Attack(body, true);
	}

	private void _on_Hit_Area_body_exited(Node body)
	{
		Attack(body, false);
	}

	private void _on_AttackTimer_timeout()
	{
		Attack(player_node, true);
	}
}
