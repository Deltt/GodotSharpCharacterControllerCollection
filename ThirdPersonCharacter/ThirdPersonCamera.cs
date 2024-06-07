using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class ThirdPersonCamera : Node3D
{
	Node3D rotationGizmo;
	SpringArm3D cameraArm;
	Camera3D camera;

	[Export] Vector3 cameraOffset = new Vector3(0, 0.25f, 0);
	[Export] bool invertX = false;
	[Export] bool invertY = false;
	[Export] float mouseSensitivity = 6f;
	[Export] float controllerSensitivity = 2f;
	[Export] float clampUpAt = 45;
	[Export] float clampDownAt = 20;
	[Export] float controllerCameraStiffness = 12f;
	[Export] bool cameraSway = true;

	Vector2 mouseInput;
	Vector2 smoothedControllerInput = Vector2.Zero;

	public override void _Ready()
	{
		rotationGizmo = GetNode<Node3D>("RotationGizmo");
		cameraArm = rotationGizmo.GetNode<SpringArm3D>("SpringArm3D");
		camera = cameraArm.GetNode<Camera3D>("Camera3D");
		camera.Current = true;
		TopLevel = true;
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);
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
		
		// Smoothly lerps camera position to player if parented to a node with a position property
		if(IsInstanceValid(GetParent<Node3D>()))
		{
			GlobalPosition = GlobalPosition.Lerp(GetParent<Node3D>().GlobalPosition + cameraOffset, 5 * (float)delta);
		}

		RotateCamera();

		if(cameraSway) SwayCamera();
    }

	private void RotateCamera()
	{
		// Rotates camera either by mouse or by secondary controller analog stick
		// If both devices are used simultaneously, mouse has priority

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
		else
		{
			Vector2 controllerInput = Vector2.Zero;

			if(Mathf.Abs(Input.GetJoyAxis(0, JoyAxis.RightX)) >= 0.5) controllerInput.X = -Input.GetJoyAxis(0, JoyAxis.RightX);
			else controllerInput.X = 0;
			
			if(Mathf.Abs(Input.GetJoyAxis(0, JoyAxis.RightY)) >= 0.5) controllerInput.Y = Input.GetJoyAxis(0, JoyAxis.RightY);
			else controllerInput.Y = 0;

			if(invertX) controllerInput.X *= -1;
			if(invertY) controllerInput.Y *= -1;

			smoothedControllerInput = smoothedControllerInput.Lerp(controllerInput, controllerCameraStiffness * (float)GetPhysicsProcessDeltaTime());
			newCameraRotationDegrees.X -= smoothedControllerInput.Y * controllerSensitivity;
			newCameraRotationDegrees.X = Mathf.Clamp(newCameraRotationDegrees.X, -clampUpAt, clampDownAt);

			newRotationDegrees.Y += smoothedControllerInput.X * controllerSensitivity;
			newRotationDegrees.Y = Mathf.Wrap(newRotationDegrees.Y, 0, 360);

			rotationGizmo.RotationDegrees = newRotationDegrees;
			cameraArm.RotationDegrees = newCameraRotationDegrees;
		}
	}

	private void SwayCamera()
	{
		// Automatically sways the camera when moving left to right from camera view (like older platforming games do; only works with CharacterBody3D parent
		if(IsInstanceValid(GetParent<CharacterBody3D>()))
		{
			float dotProduct = GetParent<CharacterBody3D>().Velocity.Dot(rotationGizmo.GlobalBasis.X);

			if(dotProduct != 0 && mouseInput.X == 0 && Mathf.Abs(Input.GetJoyAxis(0, JoyAxis.RightX)) < 0.5)
			{
				rotationGizmo.RotateY(-(dotProduct / 6 * (float)GetPhysicsProcessDeltaTime()));
			}
		}
	}
}
