using System;
using Godot;

public partial class StrategyCamera : Node3D
{
	[Export] float cameraSpeed = 25;
	[Export] float minCameraDistance = 2;
	[Export] float maxCameraDistance = 20;
	[Export] float zoomSensitivity = 40;
	[Export] float cameraAngle = -55;

	Camera3D camera;

	Vector3 inputDirection;
	Vector3 movementDirection;
	float cameraDistance = 18;
	float previousMouseX = 0;

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("Camera3D");
		camera.RotationDegrees = new Vector3(cameraAngle, camera.RotationDegrees.Y, camera.RotationDegrees.Z);
		camera.Current = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		//Escp to quit
		if(Input.IsPhysicalKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}
		
		RotateCamera();
		MoveCamera();
		ZoomCamera();
	}

	private void RotateCamera()
	{
		if(Input.IsMouseButtonPressed(MouseButton.Middle))
		{
			float mouseXDifference = previousMouseX - GetViewport().GetMousePosition().X;
			Vector3 newRotationDegrees = RotationDegrees;

			if(mouseXDifference != 0)
			{
				newRotationDegrees.Y -= mouseXDifference * -0.2f * (1920 / (int)ProjectSettings.GetSetting("display/window/size/viewport_width"));
				newRotationDegrees.Y = Mathf.Wrap(newRotationDegrees.Y, 0, 360);
				RotationDegrees = newRotationDegrees;
			}
		}
		previousMouseX = GetViewport().GetMousePosition().X;
	}

	private void MoveCamera()
	{
		inputDirection = new Vector3(Convert.ToInt32(Input.IsPhysicalKeyPressed(Key.D)) - Convert.ToInt32(Input.IsPhysicalKeyPressed(Key.A)),
		0,
		Convert.ToInt32(Input.IsPhysicalKeyPressed(Key.S)) - Convert.ToInt32(Input.IsPhysicalKeyPressed(Key.W)));

		float cameraRotationY = GlobalTransform.Basis.GetEuler().Y;
		movementDirection = inputDirection.Normalized().Rotated(Vector3.Up, cameraRotationY);
		GlobalPosition = GlobalPosition.Lerp(GlobalPosition + movementDirection, cameraSpeed * (float)GetPhysicsProcessDeltaTime());
	}

	private void ZoomCamera()
	{
		if(InputMap.HasAction("zoom_out") && InputMap.HasAction("zoom_in"))
		{
			if(Input.IsActionJustReleased("zoom_out"))
			{
				cameraDistance += zoomSensitivity * (float)GetPhysicsProcessDeltaTime();
			}
			else if(Input.IsActionJustReleased("zoom_in"))
			{
				cameraDistance -= zoomSensitivity * (float)GetPhysicsProcessDeltaTime();
			}
		}
		else
		{
			InputMap.AddAction("zoom_out", 0.5f);
			InputEventMouseButton eventZoomOut = new InputEventMouseButton
			{
				ButtonIndex = MouseButton.WheelDown
			};
			InputMap.ActionAddEvent("zoom_out", eventZoomOut);

			InputMap.AddAction("zoom_in", 0.5f);
			InputEventMouseButton eventZoomIn = new InputEventMouseButton
			{
				ButtonIndex = MouseButton.WheelUp
			};
			InputMap.ActionAddEvent("zoom_in", eventZoomIn);
		}

		cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);
		camera.Position = camera.Position.Lerp(camera.Basis.Z * cameraDistance, 10 * (float)GetPhysicsProcessDeltaTime());
	}
}
