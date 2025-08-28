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
	private Vector3 first_person_offset = new Vector3(0, 0.5f, 0);
	
	
	private static SpringArm3D SpringArm;
	private static Node3D Camera_Target;
	private static Camera3D Camera;
	
	public override void _Ready()
	{
		SpringArm = GetNode<SpringArm3D>("Spring_Arm");
		Camera_Target = GetNode<Node3D>("Spring_Arm/Camera_Target");
		Camera = GetNode<Camera3D>("Camera3D");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		var Player = GetParent() as CharacterBody3D;
		
		
		SpringArm.SpringLength = camera_offset.Y;
		if(camera_offset.Z == max_camera_zoomin)
		{
			Camera.Position = new Vector3(0, 0, 0);
			GlobalPosition = Player.GlobalPosition + first_person_offset;
		}
		else
		{
			Camera.Position = Lerp3(Camera.Position, Camera_Target.Position, 0.3f);
			GlobalPosition = Lerp3(GlobalPosition, Player.GlobalPosition, 0.5f);
		}
		
		Rotation = new Vector3
		(
			camera_rotation.X,
			camera_rotation.Y,
			0
		);
	}
	
	public override void _Input(InputEvent @event)
	{
		if(Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if(@event is InputEventMouseMotion eventMouseMotion)
			{
				float new_rotation = camera_rotation.Y - (eventMouseMotion.Relative.X / camera_sensitivity);
				if ( new_rotation > 2 * Mathf.Pi )
					new_rotation -= ( 2 * Mathf.Pi );
				else if ( new_rotation < 0 )
					new_rotation += ( 2 * Mathf.Pi );
				camera_rotation.Y = new_rotation;
				
				new_rotation = camera_rotation.X - (eventMouseMotion.Relative.Y / camera_sensitivity);
				if ( new_rotation > Mathf.Pi/4 )
					new_rotation = camera_rotation.X;
				else if ( new_rotation < -Mathf.Pi/2 )
					new_rotation = camera_rotation.X;
				camera_rotation.X = new_rotation;
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
									//camera_rotation.X += 0.1f;
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
										
										//camera_rotation.X -= 0.1f;
									}
								}
							}
						}
					break;
				}
			}
		}
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
