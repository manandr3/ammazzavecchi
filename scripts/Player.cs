using System.Runtime.Serialization.Formatters;
using Godot;

public partial class Player : CharacterBody3D
{
	[Export] public int speed { get; set; } = 14;
	
	[Export] public int fall_acceleration { get; set; } = 75;
	
	[Export] public int jump_impulse { get; set; } = 20;
	
	[Export] public int boosted_speed { get; set; } = 40;
	
	[Export] public bool double_jump { get; set; } = true;
	
	[Export] public bool dash { get; set; } = true;
	
	[Export] public float dash_control { get; set; } = 20f;
	
	
	
	private Vector3 target_velocity = Vector3.Zero;
	private Vector3 direction = Vector3.Zero;
	
	private float target_speed = 0;
	private float target_fall_acceleration = 0;
	private static bool just_dashed;
	private static bool just_double_jumped;
	private static Node3D beam_mesh;
	private static Node3D camera_controller;
	private static Timer timer_dash;


	public override void _Ready()
	{
		//Instatiation of bean and camera_controller nodes (they are children of the player node)
		beam_mesh = GetNode<Node3D>("Beam");
		camera_controller = GetNode<Node3D>("Camera_Controller");
		timer_dash = GetNode<Timer>("TimerDash");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		beam_mesh.RotateY(camera_controller.Rotation.Y - beam_mesh.Rotation.Y - 180 * Mathf.DegToRad(1));
		direction = Vector3.Zero;

		// Vertical velocity
		if (!IsOnFloor()) // gravity
		{
			if (target_speed < speed + dash_control)
			{
				target_velocity.Y -= target_fall_acceleration * ((float)delta) / 1.5f;
			}
		}
		else
		{
			target_velocity.Y = 0;
			just_double_jumped = false;
		}
		
		if (target_speed < speed + dash_control)
		{
			target_speed = speed;
		}
			
		if(Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			//adding the direction information to the direction vector
			//this way sideway movement is enabled as well as movement cancellation when opposite movements are pressed
			if (Input.IsActionPressed("move_right"))
			{
				direction.X += 1.0f;
			}

			if (Input.IsActionPressed("move_left"))
			{
				direction.X -= 1.0f;
			}

			if (Input.IsActionPressed("move_back"))
			{
				direction.Z -= 1.0f;
			}

			if (Input.IsActionPressed("move_forward"))
			{
				direction.Z += 1.0f;
			}
			
			if (Input.IsActionPressed("jump"))
			{

				// the double jump effect only applies if the effect wasn't already applied in the same airtime,
				// the double jump effect is active and if the player is in the air 
				if(!just_double_jumped && double_jump && !IsOnFloor())
				{
					// to activate the double jump a new jump impulse to the Y direction of the target velocity
					target_velocity.Y = jump_impulse;
					just_double_jumped = true;
				}

				// the IsOnFloor() function returns true if the player node is touching the ground
				if(IsOnFloor())
				{
					// to make the player jump the terget velocity in the Y direction is setted at an impulse
					target_velocity.Y = jump_impulse;

					// if the player jups from the ground the double jump status goes to reset
					just_double_jumped = false;
				}
				else 
				{
					// the player is in the air falling
					if(target_velocity.Y < 0)
					{
						// if the player is falling and the jump button is pressed the gliding effect is atcivated by setting the fall acceleration to a small amount
						target_fall_acceleration = 5;
					}
					else
					{
						// if the player is not falling already it needs to be slowed by the fall acceleration amount
						target_fall_acceleration = fall_acceleration;
					}
				}
			}
			else
			{
				// if the player is not pressing the jump button it might be falling so the target fall acceleration is set to the fall acceleration
				target_fall_acceleration = fall_acceleration;
			}
			
			// the dash effect is activated only if the cooldown isn't finished and the dash is enabed
			if (Input.IsActionJustPressed("dash") && !just_dashed && dash)
			{
				// to activate the dash effect the spped is set to the boosted speed
				target_speed = boosted_speed;
				just_dashed = true;
				
				//the timer dash cooldown is set to 1 second
				timer_dash.Start(1f);
			}
			
			// this if is checking if the target speed is still grater then the set speed
			if (target_speed > speed)
			{
				// the target speed is decreased gradually to the speed value using the Lerp() function
				target_speed = just_dashed ? Lerp(target_speed, (float)speed, 0.008f) : speed;
			}
		}



		if (direction != Vector3.Zero)
		{
			// get camera forward and right vectors from its basis
			var basis = camera_controller.GlobalTransform.Basis;

			// flatten on XZ plane
			Vector3 forward = -basis.Z; // forward direction
			forward.Y = 0;
			forward = forward.Normalized();

			Vector3 right = basis.X; // right direction
			right.Y = 0;
			right = right.Normalized();

			// combine with input
			direction = (right * direction.X + forward * direction.Z).Normalized();
		}
		
		target_velocity.X = direction.X * target_speed;
		target_velocity.Z = direction.Z * target_speed;

		// moving the character
		Velocity = target_velocity;
		
		MoveAndSlide();
	}
	
	// captures the mouse if anything is clicked
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}
	
	public static Vector3 Lerp3(Vector3 first, Vector3 second, float amount)
	{
		float retX = Lerp(first.X, second.X, amount);
		float retY = Lerp(first.Y, second.Y, amount);
		float retZ = Lerp(first.Z, second.Z, amount);
		return new Vector3(retX, retY, retZ);
	}
	
	public static float Lerp(float first, float second, float amount)
	{
		return first * (1 - amount) + second * amount;
	}
	
	// the timeout of the dash cooldown resets the just_dashed flag
	public static void onTimerDashTimeout()
	{
		just_dashed = false;
	}
}
	
