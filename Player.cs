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
			//direction.Z += 1.0f;
		}
		if (Input.IsActionPressed("move_forward"))
		{
			direction.X -= 1.0f;
			direction.Z -= 1.0f;
			
			//direction.Z -= 1.0f;
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
		CameraController.GlobalPosition = Lerp3(CameraController.GlobalPosition,GlobalPosition,0.05f);
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
	
