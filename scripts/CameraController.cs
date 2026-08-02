using Godot;

public partial class CameraController : Node3D
{	
	[Export] public float camera_sensitivity { get; set; } = 0.002f;
	[Export] public float MinPitch = Mathf.DegToRad(-89.99f);
	[Export] public float MaxPitch = Mathf.DegToRad(60);
	
	private Vector3 camera_offset = new Vector3(0, 4, 4);
	private int max_camera_zoomout = 12;
	private int max_camera_zoomin = 1;
	private Vector3 first_person_offset = new Vector3(0, 1f, 0);
	
	private float yaw = 0f;
	private float pitch = 0f;
	
	private static SpringArm3D spring_arm_ground;
	private static SpringArm3D spring_arm_objects;
	private static Node3D camera_target;
	private static Node3D camera_target_collision;
	private static Camera3D camera;
	private static Area3D camera_collisions_checker;
	private static bool camera_collided = false;
	private static CharacterBody3D player;

	
	public override void _Ready()
	{
		//Instatiation of bean and camera_controller nodes (they are children of the player node)
		spring_arm_ground = GetNode<SpringArm3D>("Spring_Arm_Ground");
		spring_arm_objects = GetNode<SpringArm3D>("Spring_Arm_Objects");
		CameraTarget = GetNode<Node3D>("Spring_Arm_Ground/Camera_Target");
		camera_target_collision = GetNode<Node3D>("Spring_Arm_Objects/Camera_Target_Collision");
		camera = GetNode<Camera3D>("Camera3D");
		camera_collisions_checker = GetNode<Area3D>("Camera_Collisions_Checker");
		player = GetParent() as CharacterBody3D;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		
		if (camera_collided && camera_target.Position == camera_target_collision.Position)
		{
			camera_collided = false;
		}
		
		spring_arm_objects.SpringLength = Lerp(spring_arm_objects.SpringLength, camera_offset.Z, 0.2f);
		spring_arm_ground.SpringLength = Lerp(spring_arm_ground.SpringLength, camera_offset.Z, 0.2f);

		if (camera_collided)
		{
			spring_arm_objects.SpringLength = Lerp(spring_arm_objects.SpringLength, camera_offset.Z, 0.2f);
			if (camera_offset.Z == max_camera_zoomin)
			{
				camera.Position = new Vector3(0, 0, 0);
				GlobalPosition = player.GlobalPosition + first_person_offset;
			}
			else
			{
				camera.Position = Lerp3(camera.Position, camera_target_collision.Position, 0.1f);
				camera_collisions_checker.Position = Lerp3(camera.Position, camera_target_collision.Position, 0.1f);
				GlobalPosition = Lerp3(GlobalPosition, player.GlobalPosition, 0.1f);
			}
		}
		else
		{
			if (camera_offset.Z == max_camera_zoomin)
			{
				camera.Position = new Vector3(0, 0, 0);
				GlobalPosition = player.GlobalPosition + first_person_offset;
			}
			else
			{
				camera.Position = Lerp3(camera.Position, camera_target.Position, 0.1f);
				camera_collisions_checker.Position = Lerp3(camera.Position, camera_target.Position, 0.1f);
				GlobalPosition = Lerp3(GlobalPosition, player.GlobalPosition, 0.1f);
			}
		}
		
		
		
		Rotation = new Vector3
		(
			pitch,
			yaw,
			0
		);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if (@event is InputEventMouseMotion eventMouseMotion)
			{
				yaw -= eventMouseMotion.Relative.X * camera_sensitivity;
				pitch = Mathf.Clamp(pitch - eventMouseMotion.Relative.Y * camera_sensitivity, MinPitch, MaxPitch);
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
		camera_collided = true;
		camera.Position = camera_target_collision.Position;
		
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