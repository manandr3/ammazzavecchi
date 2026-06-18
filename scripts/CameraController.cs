using Godot;

public partial class CameraController : Node3D
{	
	[Export] public float camera_sensitivity { get; set; } = 0.002f;
	[Export] public float MinPitch = Mathf.DegToRad(-89.99f);
	[Export] public float MaxPitch = Mathf.DegToRad(60);
	
	private float _rotationY = 0f;
	
	private Vector3 camera_offset = new Vector3(0, 4, 4);
	private int max_camera_zoomout = 12;
	private int max_camera_zoomin = 1;
	private Vector3 first_person_offset = new Vector3(0, 1f, 0);
	
	private float _yaw = 0f;
	private float _pitch = 0f;
	
	private static SpringArm3D SpringArmGround;
	private static SpringArm3D SpringArmObjects;
	private static Node3D CameraTarget;
	private static Node3D CameraTargetCollision;
	private static Camera3D Camera;
	private static Area3D CameraCollisionsChecker;
	private bool CameraCollided = false;

	public override void _Ready()
	{
		SpringArmGround = GetNode<SpringArm3D>("Spring_Arm_Ground");
		SpringArmObjects = GetNode<SpringArm3D>("Spring_Arm_Objects");
		CameraTarget = GetNode<Node3D>("Spring_Arm_Ground/Camera_Target");
		CameraTargetCollision = GetNode<Node3D>("Spring_Arm_Objects/Camera_Target_Collision");
		Camera = GetNode<Camera3D>("Camera3D");
		CameraCollisionsChecker = GetNode<Area3D>("CameraCollisionsChecker");
	}
	
	public override void _PhysicsProcess(double delta)
	{

		var Player = GetParent() as CharacterBody3D;

		if (CameraCollided && CameraTarget.Position == CameraTargetCollision.Position)
			CameraCollided = false;
		
		SpringArmObjects.SpringLength = Lerp(SpringArmObjects.SpringLength, camera_offset.Z, 0.2f);
		SpringArmGround.SpringLength = Lerp(SpringArmGround.SpringLength, camera_offset.Z, 0.2f);

		if (CameraCollided)
		{
			SpringArmObjects.SpringLength = Lerp(SpringArmObjects.SpringLength, camera_offset.Z, 0.2f);
			if (camera_offset.Z == max_camera_zoomin)
			{
				Camera.Position = new Vector3(0, 0, 0);
				GlobalPosition = Player.GlobalPosition + first_person_offset;
			}
			else
			{
				Camera.Position = Lerp3(Camera.Position, CameraTargetCollision.Position, 0.1f);
				CameraCollisionsChecker.Position = Lerp3(Camera.Position, CameraTargetCollision.Position, 0.1f);
				GlobalPosition = Lerp3(GlobalPosition, Player.GlobalPosition, 0.1f);
			}
		}
		else
		{
			if (camera_offset.Z == max_camera_zoomin)
			{
				Camera.Position = new Vector3(0, 0, 0);
				GlobalPosition = Player.GlobalPosition + first_person_offset;
			}
			else
			{
				Camera.Position = Lerp3(Camera.Position, CameraTarget.Position, 0.1f);
				CameraCollisionsChecker.Position = Lerp3(Camera.Position, CameraTarget.Position, 0.1f);
				GlobalPosition = Lerp3(GlobalPosition, Player.GlobalPosition, 0.1f);
			}
		}
		
		
		
		Rotation = new Vector3
		(
			_pitch,
			_yaw,
			0
		);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if (@event is InputEventMouseMotion eventMouseMotion)
			{
				_yaw -= eventMouseMotion.Relative.X * camera_sensitivity;
				_pitch = Mathf.Clamp(_pitch - eventMouseMotion.Relative.Y * camera_sensitivity, MinPitch, MaxPitch);
			}

			if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
			{
				if (Input.MouseMode == Input.MouseModeEnum.Captured)
				{
					switch (mouseEvent.ButtonIndex)
					{
						case MouseButton.WheelUp:
							if (camera_offset.Z != max_camera_zoomin)
							{
								camera_offset.Z -= 1f;
							}
							break;

						case MouseButton.WheelDown:
							if (camera_offset.Z != max_camera_zoomout)
							{
								camera_offset.Z += 1f;
							}
							break;
					}
				}
			}
		}
	}

	private void OnCameraCollisionsCheckerAreaShapeEntered(Node3D body)
	{
		CameraCollided = true;
		Camera.Position = CameraTargetCollision.Position;
		
	}
	
	public static Vector3 Lerp3(Vector3 First, Vector3 Second, float Amount)
	{
		float retX = Lerp(First.X, Second.X, Amount);
		float retY = Lerp(First.Y, Second.Y, Amount);
		float retZ = Lerp(First.Z, Second.Z, Amount);
		return new Vector3(retX, retY, retZ);
	}
	
	public static float Lerp(float First, float Second, float Amount)
	{
		return First * (1 - Amount) + Second * Amount;
	}
}