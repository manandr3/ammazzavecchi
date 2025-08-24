using Godot;

public partial class Player : CharacterBody3D
{
	[Export]
	public int Speed { get; set; } = 14;
	
	[Export]
	public int FallAcceleration { get; set; } = 75;
	
	[Export]
	public int JumpImpulse { get; set; } = 20;
	
	private Vector3 _targetVelocity = Vector3.Zero;
	private Vector3 camera_offset = Vector3.Zero;
	private int max_camera_zoomout = 12;
	private int max_camera_zoomin = -2;
	public override void _PhysicsProcess(double delta)
	{
		var direction = Vector3.Zero;

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

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
		}
		
		_targetVelocity.X = direction.X * Speed;
		_targetVelocity.Z = direction.Z * Speed;

		// Vertical velocity
		if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		// Moving the character
		Velocity = _targetVelocity;
		
		if (IsOnFloor() && Input.IsActionJustPressed("jump"))
		{
			_targetVelocity.Y = JumpImpulse;
		}
		
		MoveAndSlide();
		
		//camera movement
		var CameraController = GetNode<Node3D>("Camera_Controller");
		CameraController.GlobalPosition = Lerp3(CameraController.GlobalPosition,GlobalPosition + camera_offset, 0.01f*((max_camera_zoomout - max_camera_zoomin)-(camera_offset.X)));
	}
	
	//mouse wheel controller to modify camera position
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch (mouseEvent.ButtonIndex)
			{
				case MouseButton.WheelUp:
					if(camera_offset.X > max_camera_zoomin)
					{
						camera_offset.X -= 1f;
						camera_offset.Y -= 1f;
						camera_offset.Z -= 1f;
					}
				   	break;
				case MouseButton.WheelDown:
					if(camera_offset.X < max_camera_zoomout)
					{
						camera_offset.X += 1f;
						camera_offset.Y += 1f;
						camera_offset.Z += 1f;
					}
					break;
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
	
	public static Vector2 Lerp2(Vector2 First, Vector2 Second, float Amount)
	{
		float retX = Lerp(First.X, Second.X, Amount);
		float retY = Lerp(First.Y, Second.Y, Amount);
		return new Vector2(retX, retY);
	}
	
	public static float Lerp(float First, float Second, float Amount)
	{
		return First * (1 - Amount) + Second * Amount;
	}
}
	
