using Godot;

public partial class Player : CharacterBody3D
{
	[Export]
	public int Speed { get; set; } = 14;
	
	[Export]
	public int FallAcceleration { get; set; } = 75;
	
	[Export]
	public int JumpImpulse { get; set; } = 20;
	
	[Export]
	public int BoostedSpeed { get; set; } = 40;
	
	private Vector3 _targetVelocity = Vector3.Zero;
	private Vector3 camera_offset = Vector3.Zero;
	private Vector3 camera_rotation = Vector3.Zero;
	private int max_camera_zoomout = 12;
	private int max_camera_zoomin = -3;
	private Vector3 first_person_offset = new Vector3(x: -3.2f, y: -1f, z: -3.2f);
	
	private int target_speed = 0;
	private static bool dashed;
	private static bool doubleJumped;
	
	public override void _PhysicsProcess(double delta)
	{
		var direction = Vector3.Zero;
		
		if(Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if (Input.IsActionPressed("move_right"))
			{
				direction.X += 1.0f;
				direction.Z -= 1.0f;
			}
			
			if (Input.IsActionPressed("move_left"))
			{
				direction.X -= 1.0f;
				direction.Z += 1.0f;
			}
			
			if (Input.IsActionPressed("move_back"))
			{
				direction.Z += 1.0f;
				direction.X += 1.0f;
			}
			
			if (Input.IsActionPressed("move_forward"))
			{
				direction.X -= 1.0f;
				direction.Z -= 1.0f;
			}
			
			if (Input.IsActionJustPressed("jump"))
			{
				if(IsOnFloor())
				{
					_targetVelocity.Y = JumpImpulse;
					doubleJumped = false;
				}
				else if(!doubleJumped)
				{
					GD.Print("whatever");
					_targetVelocity.Y = JumpImpulse;
					doubleJumped = true;
				}
					
			}
			
			if (Input.IsActionJustPressed("dash") && !dashed)
			{
				target_speed = BoostedSpeed;
				dashed = true;
				var TimerDash = GetNode<Timer>("TimerDash");
				TimerDash.Start(1);
			}
			
			if (target_speed != Speed)
				target_speed = dashed ? ((int)Lerp((float)target_speed, (float)Speed, 0.05f)) : Speed;
		}

		

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
		}
		
		_targetVelocity.X = direction.X * target_speed;
		_targetVelocity.Z = direction.Z * target_speed;

		// Vertical velocity
		if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}
		// Moving the character
		Velocity = _targetVelocity;
		
		
		
		MoveAndSlide();
		
		//camera movement
		var CameraController = GetNode<Node3D>("Camera_Controller");
		CameraController.GlobalPosition = Lerp3(CameraController.GlobalPosition, GlobalPosition + ( camera_offset.X < -2 ? first_person_offset : camera_offset), 0.5f*((max_camera_zoomout - max_camera_zoomin)-(camera_offset.X)) / (max_camera_zoomout - max_camera_zoomin));
		CameraController.Rotation = Lerp_angle(CameraController.Rotation, camera_rotation, 0.2f);
	}
	
	//mouse wheel controller to modify camera position
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch (mouseEvent.ButtonIndex)
			{
				case MouseButton.WheelUp:
					if(Input.MouseMode == Input.MouseModeEnum.Captured)
					{
						if(camera_offset.X > max_camera_zoomin)
						{
							camera_offset.X -= 1f;
							camera_offset.Y -= 1f;
							camera_offset.Z -= 1f;
							if(camera_offset.X < 0)
							{
								camera_rotation.X += 0.1f;//Mathf.DegToRad(0.15f);
								camera_rotation.Z -= 0.1f;//Mathf.DegToRad(0.15f);
							}
						}
					}
				   	break;
					
				case MouseButton.WheelDown:
					if(Input.MouseMode == Input.MouseModeEnum.Captured)
					{
						if(camera_offset.X < max_camera_zoomout)
						{
							camera_offset.X += 1f;
							camera_offset.Y += 1f;
							camera_offset.Z += 1f;
							if(camera_offset.X <= 0)
							{
								if(camera_rotation.X != 0)
								{
									camera_rotation.X -= 0.1f;//Mathf.DegToRad(0.15f);
									camera_rotation.Z += 0.1f;//Mathf.DegToRad(0.15f);
								}
							}
						}
					}
					break;
				
				case MouseButton.Left:
					Input.MouseMode = Input.MouseModeEnum.Captured;
					break;
			}
		}
		
		//if(@event is InputEventMouseMotion eventMouseMotion)
	}
	
	public static Vector3 Lerp3(Vector3 First, Vector3 Second, float Amount)
	{
		float retX = Lerp(First.X, Second.X, Amount);
		float retY = Lerp(First.Y, Second.Y, Amount);
		float retZ = Lerp(First.Z, Second.Z, Amount);
		return new Vector3(retX, retY, retZ);
	}
	
	public static Vector3 Lerp_angle(Vector3 First, Vector3 Second, float Amount)
	{		
		float retX = Mathf.LerpAngle(First.X, Second.X, Amount);
		float retY = Mathf.LerpAngle(First.Y, Second.Y, Amount);
		float retZ = Mathf.LerpAngle(First.Z, Second.Z, Amount);
		return new Vector3(retX, retY, retZ);
	}
	
	public static float Lerp(float First, float Second, float Amount)
	{
		return First * (1 - Amount) + Second * Amount;
	}
	
	public static void onTimerDashTimeout()
	{
		dashed = false;
	}
}
	
