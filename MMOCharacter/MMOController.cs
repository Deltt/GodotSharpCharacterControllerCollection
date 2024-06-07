using Godot;
using System;

public partial class MMOController : CharacterBody3D
{
	[Export] public float Speed = 5.0f;
	[Export] public float JumpVelocity = 8f;
	[Export] public float rotationSpeed = 5f;
	[Export] bool wasdControlEnabled = true;

	Node3D playerOrientation;
	AnimationPlayer ap;
	MMOCamera mmoCamera;

	Vector2 inputDirection = Vector2.Zero;
	Vector3 movementDirection = Vector3.Zero;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle() * 2.5f;

    public override void _Ready()
    {
        if(IsInstanceValid(GetNode<MMOCamera>("MMOCamera")))
		{
			mmoCamera = GetNode<MMOCamera>("MMOCamera");
		}

		if(IsInstanceValid(GetNode<Node3D>("PlayerOrientation")))
		{
			playerOrientation = GetNode<Node3D>("PlayerOrientation");
		}

		if(IsInstanceValid(GetNode<AnimationPlayer>("AnimationPlayer")))
		{
			ap = GetNode<AnimationPlayer>("AnimationPlayer");
		}
	}

    public override void _PhysicsProcess(double delta)
	{
		GetInput();
		Move();
		RotateModel();
		Animate();
	}

	private void GetInput()
	{
		// Get inputs
		if(Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down").LengthSquared() > 0)
		{
			inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		}
		else if(wasdControlEnabled && (Input.IsPhysicalKeyPressed(Key.W) || Input.IsPhysicalKeyPressed(Key.A) || Input.IsPhysicalKeyPressed(Key.S) || Input.IsPhysicalKeyPressed(Key.D)))
		{
			inputDirection = new Vector2(Convert.ToInt32(Input.IsPhysicalKeyPressed(Key.D)) - Convert.ToInt32(Input.IsPhysicalKeyPressed(Key.A)),
			Convert.ToInt32(Input.IsPhysicalKeyPressed(Key.S)) - Convert.ToInt32(Input.IsPhysicalKeyPressed(Key.W)));
		}
		else if(Input.IsMouseButtonPressed(MouseButton.Right) && Input.IsMouseButtonPressed(MouseButton.Left))
		{
			inputDirection = new Vector2(0, -1);
		}
		else inputDirection = Vector2.Zero;
	}

	private void Move()
	{
		Vector3 velocity = Velocity;

		// Add the gravity
		if (!IsOnFloor()) velocity.Y -= gravity * (float)GetPhysicsProcessDeltaTime();

		// Handle Jump
		if ((Input.IsPhysicalKeyPressed(Key.Space) || Input.IsJoyButtonPressed(0, JoyButton.A)) && IsOnFloor()) velocity.Y = JumpVelocity;

		// Handle movement based on strafe-cam or free-cam (left or right mouse button)
		if(!Input.IsMouseButtonPressed(MouseButton.Right))
		{
			GetNode<Node3D>("PlayerOrientation").RotationDegrees = GetNode<Node3D>("PlayerOrientation").RotationDegrees + new Vector3(0, inputDirection.X * -3, 0);
			inputDirection.X = 0;
		}

		// Convert input to movementDirection
		Vector3 movementDirection = -(GetNode<Node3D>("PlayerOrientation").Transform.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y)).Normalized();

		// Apply movement
		velocity.X = movementDirection.X * Speed;
		velocity.Z = movementDirection.Z * Speed;
		Velocity = velocity;
		MoveAndSlide();
	}

	private void RotateModel()
	{
		if(IsInstanceValid(playerOrientation) && Input.IsMouseButtonPressed(MouseButton.Right))
		{
			playerOrientation.RotationDegrees = new Vector3(playerOrientation.RotationDegrees.X, mmoCamera.GetNode<Node3D>("RotationGizmo").RotationDegrees.Y, playerOrientation.RotationDegrees.Z);
		}
	}

	private void Animate()
	{
		// Handle animations
		if(IsOnFloor() && Velocity.Length() >= 3)
		{
			if(ap.CurrentAnimation != "walk")
			{
				ap.Play("walk", 0.1, 2.5f);
			}
		}
		else
		{
			if(ap.CurrentAnimation != "idle")
			{
				ap.Play("idle", 0.1, 1);
			}
		}
	}
}
