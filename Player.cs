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
	
	[Export]
	public bool DoubleJump { get; set; } = true;
	
	[Export]
	public bool Dash { get; set; } = true;
	
	
	
	private Vector3 _targetVelocity = Vector3.Zero;
	
	private int target_speed = 0;
	private static bool just_dashed;
	private static bool just_doubleJumped;
	
	public override void _PhysicsProcess(double delta)
	{
		var direction = Vector3.Zero;
		
		if(Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if (Input.IsActionPressed("move_right"))
			direction.X += 1.0f;

			if (Input.IsActionPressed("move_left"))
				direction.X -= 1.0f;

			if (Input.IsActionPressed("move_back"))
				direction.Z -= 1.0f;

			if (Input.IsActionPressed("move_forward"))
				direction.Z += 1.0f;
			
			if (Input.IsActionPressed("jump") && IsOnFloor())
			{
				_targetVelocity.Y = JumpImpulse;
				just_doubleJumped = false;
			}
			
			if(Input.IsActionJustPressed("jump") && !just_doubleJumped && DoubleJump && !IsOnFloor())
			{
				_targetVelocity.Y = JumpImpulse;
				just_doubleJumped = true;
			}
			
			if (Input.IsActionJustPressed("dash") && !just_dashed && Dash)
			{
				target_speed = BoostedSpeed;
				just_dashed = true;
				var TimerDash = GetNode<Timer>("TimerDash");
				TimerDash.Start(1);
			}
			
			if (target_speed != Speed)
				target_speed = just_dashed ? ((int)Lerp((float)target_speed, (float)Speed, 0.05f)) : Speed;
		}

		

		if (direction != Vector3.Zero)
		{
			var cameraController = GetNode<Node3D>("Camera_Controller");

			// Get camera forward and right vectors from its basis
			var basis = cameraController.GlobalTransform.Basis;

			// Flatten on XZ plane
			Vector3 forward = -basis.Z; // forward direction
			forward.Y = 0;
			forward = forward.Normalized();

			Vector3 right = basis.X; // right direction
			right.Y = 0;
			right = right.Normalized();

			// Combine with input
			direction = (right * direction.X + forward * direction.Z).Normalized();
		}
		
		_targetVelocity.X = direction.X * target_speed;
		_targetVelocity.Z = direction.Z * target_speed;

		// Vertical velocity
		if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		
		// Moving the character
		Velocity = _targetVelocity;
		
		
		
		MoveAndSlide();
	}
	
	//mouse wheel controller to modify camera position
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
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
	
	public static void onTimerDashTimeout()
	{
		just_dashed = false;
	}
}
	
