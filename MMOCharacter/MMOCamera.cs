using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class MMOCamera : Node3D
{
	Node3D rotationGizmo;
	SpringArm3D cameraArm;
	public Camera3D camera;
	Node3D playerOrientation;

	//[Export] Vector3 cameraOffset = new Vector3(0, 0.25f, 0);
	[Export] bool invertX = false;
	[Export] bool invertY = false;
	[Export] float mouseSensitivity = 6f;
	[Export] float controllerSensitivity = 2f;
	[Export] float clampUpAt = 45;
	[Export] float clampDownAt = 20;
	[Export] float controllerCameraStiffness = 12f;
	[Export] bool cameraSway = false;
	[Export] float autoRotateSpeed = 5f;

	Vector2 mouseInput;
	Vector2 smoothedControllerInput = Vector2.Zero;

	public override void _Ready()
	{
		rotationGizmo = GetChild<Node3D>(0);
		cameraArm = rotationGizmo.GetChild<SpringArm3D>(0);
		camera = cameraArm.GetChild<Camera3D>(0);
		camera.Current = true;
		TopLevel = true;
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Confined);
	}

	public override void _Input(InputEvent @event)
    {
		if(@event is InputEventMouseMotion mouseEvent)
		{	
			mouseInput = mouseEvent.Relative;
		}
    }

    public override void _PhysicsProcess(double delta)
    {
		//Escp to quit
		if(Input.IsPhysicalKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}

		// Smoothly lerps camera position to player
		if(IsInstanceValid(GetParent<Node3D>()))
		{
			//GlobalPosition = GlobalPosition.Lerp(GetParent<Node3D>().GlobalPosition + cameraOffset, 5 * (float)delta);
			GlobalPosition = GetParent<Node3D>().GlobalPosition;
		}

		if(Input.IsMouseButtonPressed(MouseButton.Right) || Input.IsMouseButtonPressed(MouseButton.Left))
		{
			RotateCamera();
			DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);
		}
		else if(IsInstanceValid(GetParent<CharacterBody3D>()) && GetParent<CharacterBody3D>().Velocity.LengthSquared() > 0.2)
		{
			AutoRotate();
			DisplayServer.MouseSetMode(DisplayServer.MouseMode.Confined);
		}
		else DisplayServer.MouseSetMode(DisplayServer.MouseMode.Confined);
    }

	private void RotateCamera()
	{
		// Rotates camera either by mouse

		Vector3 newRotationDegrees = rotationGizmo.RotationDegrees;
		Vector3 newCameraRotationDegrees = cameraArm.RotationDegrees;

		if(mouseInput != Vector2.Zero)
		{
			newCameraRotationDegrees.X -= mouseInput.Y * mouseSensitivity * (1920 / DisplayServer.WindowGetSize().X) * (float)GetPhysicsProcessDeltaTime();
			newCameraRotationDegrees.X = Mathf.Clamp(newCameraRotationDegrees.X, -clampUpAt, clampDownAt);

			newRotationDegrees.Y -= mouseInput.X * mouseSensitivity * (1080 / DisplayServer.WindowGetSize().Y) * (float)GetPhysicsProcessDeltaTime();
			newRotationDegrees.Y = Mathf.Wrap(newRotationDegrees.Y, 0, 360);

			rotationGizmo.RotationDegrees = newRotationDegrees;
			cameraArm.RotationDegrees = newCameraRotationDegrees;
			mouseInput = Vector2.Zero;
		}
	}

	private void AutoRotate()
	{
		if(IsInstanceValid(GetParent<Node3D>()))
		{
			float yRotation = Mathf.LerpAngle(rotationGizmo.Rotation.Y, Mathf.Atan2(GetParent<Node3D>().GetNode<Node3D>("PlayerOrientation").GlobalBasis.Z.X, GetParent<Node3D>().GetNode<Node3D>("PlayerOrientation").GlobalBasis.Z.Z), autoRotateSpeed * (float)GetPhysicsProcessDeltaTime());
			Vector3 rotationSmoothed = new Vector3(rotationGizmo.Rotation.X, yRotation, rotationGizmo.Rotation.Z);
			rotationGizmo.Rotation = rotationSmoothed;
		}
	}
}
