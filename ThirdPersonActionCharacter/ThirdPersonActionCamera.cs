using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class ThirdPersonActionCamera : Node3D
{
	Node3D rotationGizmo;
	SpringArm3D cameraArm;
	Camera3D camera;

	[Export] Vector3 cameraOffset = new Vector3(-1, 0.35f, -1f);
	[Export] bool invertX = false;
	[Export] bool invertY = false;
	[Export] float mouseSensitivity = 6f;
	[Export] float controllerSensitivity = 2f;
	[Export] float clampUpAt = 45;
	[Export] float clampDownAt = 20;
	[Export] float controllerCameraStiffness = 12f;

	Vector2 mouseInput;
	Vector2 smoothedControllerInput = Vector2.Zero;

	public override void _Ready()
	{
		rotationGizmo = GetNode<Node3D>("RotationGizmo");
		cameraArm = rotationGizmo.GetNode<SpringArm3D>("SpringArm3D");
		camera = cameraArm.GetNode<Camera3D>("Camera3D");
		camera.Current = true;
		cameraArm.Position = cameraOffset;
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
		
		RotateCamera();
    }

	private void RotateCamera()
	{
		// Rotates camera either by mouse or by secondary controller analog stick
		// If both devices are used simultaneously, mouse has priority

		if(!IsInstanceValid(GetParent<Node3D>()))
		{
			GD.PrintErr("[Third Person Camera]: This camera type requires a parent object with position properties (Node3D or inherited)!");
			SetPhysicsProcess(false);
			return;
		}

		Vector3 newRotationDegrees = GetParent<Node3D>().RotationDegrees;
		Vector3 newCameraRotationDegrees = cameraArm.RotationDegrees;

		if(mouseInput != Vector2.Zero)
		{
			newCameraRotationDegrees.X -= mouseInput.Y * mouseSensitivity * (1920 / DisplayServer.WindowGetSize().X) * (float)GetPhysicsProcessDeltaTime();
			newCameraRotationDegrees.X = Mathf.Clamp(newCameraRotationDegrees.X, -clampUpAt, clampDownAt);

			newRotationDegrees.Y -= mouseInput.X * mouseSensitivity * (1080 / DisplayServer.WindowGetSize().Y) * (float)GetPhysicsProcessDeltaTime();
			newRotationDegrees.Y = Mathf.Wrap(newRotationDegrees.Y, 0, 360);

			GetParent<CharacterBody3D>().RotationDegrees = newRotationDegrees;
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

			GetParent<CharacterBody3D>().RotationDegrees = newRotationDegrees;
			cameraArm.RotationDegrees = newCameraRotationDegrees;
		}
	}
}
