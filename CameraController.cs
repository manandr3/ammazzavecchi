using Godot;

public partial class CameraController : Node3D
{	
	[Export]
	public int camera_sensitivity { get; set; } = 100;
	
	private float _rotationY = 0f;
	
	private Vector3 camera_offset = new Vector3(0, 4, 4);
	private Vector3 camera_rotation = Vector3.Zero;
	private int max_camera_zoomout = 12;
	private int max_camera_zoomin = 1;
	private Vector3 first_person_offset = new Vector3(x: 0f, y: 3f, z: 6f);
	
	public override void _PhysicsProcess(double delta)
	{
		var Player = GetParent() as CharacterBody3D;
		var CameraTarget = GetNode<Node3D>("Camera_Target");
		
		GlobalPosition = Lerp3(GlobalPosition, Player.GlobalPosition, 0.05f);
		
		CameraTarget.Position = Lerp3(CameraTarget.Position, camera_offset, 0.05f);
		
		Rotation = new Vector3 ( 0, camera_rotation.Y, 0);
	}
	
	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventMouseMotion eventMouseMotion)
		{
			camera_rotation.Y -= eventMouseMotion.Relative.X / camera_sensitivity;
		}
		
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch (mouseEvent.ButtonIndex)
			{
				case MouseButton.WheelUp:
					if(Input.MouseMode == Input.MouseModeEnum.Captured)
					{
						if(camera_offset.Z > max_camera_zoomin)
						{
							//camera_offset.X -= 1f;
							camera_offset.Y -= 1f;
							camera_offset.Z -= 1f;
							if(camera_offset.X < max_camera_zoomin + 2)
							{
								camera_rotation.X += 0.1f;//Mathf.DegToRad(0.15f);
								//camera_rotation.Z -= 0.1f;//Mathf.DegToRad(0.15f);
							}
						}
					}
				break;
					
				case MouseButton.WheelDown:
					if(Input.MouseMode == Input.MouseModeEnum.Captured)
					{
						if(camera_offset.Z < max_camera_zoomout)
						{
							//camera_offset.X += 1f;
							camera_offset.Y += 1f;
							camera_offset.Z += 1f;
							if(camera_offset.Z <= 0)
							{
								if(camera_rotation.Z != max_camera_zoomin + 2)
								{
									camera_rotation.X -= 0.1f;//Mathf.DegToRad(0.15f);
									//camera_rotation.Z += 0.1f;//Mathf.DegToRad(0.15f);
								}
							}
						}
					}
				break;
			}
		}
	}
	
	public static Vector3 Lerp_angle(Vector3 First, Vector3 Second, float Amount)
	{		
		//float retX = Mathf.LerpAngle(First.X, Second.X, Amount);
		float retX = 0;
		
		float retY = Mathf.LerpAngle(First.Y, Second.Y, Amount);
		//float retZ = Mathf.LerpAngle(First.Z, Second.Z, Amount);
		float retZ = 0;
		return new Vector3(retX, retY, retZ);
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
