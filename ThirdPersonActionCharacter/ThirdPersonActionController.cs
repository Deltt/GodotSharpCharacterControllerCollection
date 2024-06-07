using Godot;
using System;

public partial class ThirdPersonActionController : CharacterBody3D
{
	[Export] public float Speed = 5.0f;
	[Export] public float JumpVelocity = 8f;
	[Export] public float rotationSpeed = 5f;
	[Export] bool wasdControlEnabled = true;
	[Export] bool neverFaceBackwards = true;

	Node3D playerOrientation;
	AnimationPlayer ap;
	ThirdPersonActionCamera thirdPersonCamera;

	Vector2 inputDirection = Vector2.Zero;
	Vector3 movementDirection = Vector3.Zero;
	Vector3 smoothedDirection = Vector3.Zero;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle() * 2.5f;

    public override void _Ready()
    {
        if(IsInstanceValid(GetNode<ThirdPersonActionCamera>("ThirdPersonActionCamera")))
		{
			thirdPersonCamera = GetNode<ThirdPersonActionCamera>("ThirdPersonActionCamera");
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
		else
		{
			if(Mathf.Abs(Input.GetJoyAxis(0, JoyAxis.LeftX)) >= 0.5) inputDirection.X = Input.GetJoyAxis(0, JoyAxis.LeftX);
			else inputDirection.X = 0;
			
			if(Mathf.Abs(Input.GetJoyAxis(0, JoyAxis.LeftY)) >= 0.5) inputDirection.Y = Input.GetJoyAxis(0, JoyAxis.LeftY);
			else inputDirection.Y = 0;
		}
	}

	private void Move()
	{
		Vector3 velocity = Velocity;

		// Add the gravity
		if (!IsOnFloor()) velocity.Y -= gravity * (float)GetPhysicsProcessDeltaTime();

		// Handle Jump
		if ((Input.IsPhysicalKeyPressed(Key.Space) || Input.IsJoyButtonPressed(0, JoyButton.A)) && IsOnFloor()) velocity.Y = JumpVelocity;

		// Convert input to movementDirection
		movementDirection = (Transform.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y)).Normalized();

		// Rotate movementDirection based on camera orientation if possible
		if(IsInstanceValid(thirdPersonCamera))
		{
			movementDirection = -movementDirection.Rotated(Vector3.Up, thirdPersonCamera.GetChild<Node3D>(0).Rotation.Y);
		}

		// Smooth movement
		smoothedDirection = smoothedDirection.Lerp(movementDirection, 8f * (float)GetPhysicsProcessDeltaTime());

		// Apply movement
		if (smoothedDirection != Vector3.Zero)
		{
			velocity.X = smoothedDirection.X * Speed;
			velocity.Z = smoothedDirection.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}
		Velocity = velocity;
		MoveAndSlide();
	}

	private void RotateModel()
	{
		// Rotate player model
		if(IsInstanceValid(playerOrientation) && movementDirection != Vector3.Zero)
		{
			Vector3 rotationDirection = movementDirection;
			float rotationSpeedAdjusted = rotationSpeed;

			if(neverFaceBackwards)
			{
				float zDot = GlobalBasis.Z.Dot(movementDirection);
				if(zDot < -0.3)
				{
					rotationDirection *= -1;
					rotationSpeedAdjusted = rotationSpeed * 0.5f;
				}
			}

			float yRotation = Mathf.LerpAngle(playerOrientation.GlobalRotation.Y, Mathf.Atan2(rotationDirection.X, rotationDirection.Z), rotationSpeedAdjusted * (float)GetPhysicsProcessDeltaTime());
			Vector3 rotationSmoothed = new Vector3(playerOrientation.GlobalRotation.X, yRotation, playerOrientation.GlobalRotation.Z);
			playerOrientation.GlobalRotation = rotationSmoothed;
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
