using Godot;
using System;

public partial class Wolf : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	[Export] public NodePath playerPath;
	[Export] public int speed = 4;
	private CharacterBody3D player;
	private NavigationAgent3D NavigationAgent;
	private RandomNumberGenerator rnd = new RandomNumberGenerator();
	private int NewTargetCounter;
	private int RndTargetCounter;

	//testing target location
	private MeshInstance3D targetMesh;
	private Vector3 targetPositionOffset;

	public override void _Ready()
	{
		player = GetNode<CharacterBody3D>(playerPath);
		NavigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		NewTargetCounter = 0;
		rnd = new RandomNumberGenerator();
		RndTargetCounter = rnd.RandiRange(-500, 500);
		targetMesh = GetNode<MeshInstance3D>("targetMesh");
		targetPositionOffset = Vector3.Zero;
	}

	public override void _PhysicsProcess(double delta)
	{
		NavigationAgent.SetTargetPosition(player.GlobalPosition);
		
		float playerDistance = GlobalPosition.DistanceTo(player.GlobalPosition);

		

		Vector3 newTarget = player.GlobalPosition;
		//if the distance to the player is grate enough the new target position is offsetted in order to avoid enemy clusters.
		if(playerDistance >= 8f)
		{


			//every 300 frame the target offset is updated to a random value
			if(NewTargetCounter >= 1200 + RndTargetCounter) 
			{
				NewTargetCounter=0;
				rnd = new RandomNumberGenerator();
				RndTargetCounter = rnd.RandiRange(-500, 500);
				targetPositionOffset.Y = 0;
				if(playerDistance > 25f)
				{
					targetPositionOffset.X= rnd.RandfRange(-15, 15);
					targetPositionOffset.Z= rnd.RandfRange(-15, 15);
				}
				else
				{
					targetPositionOffset.X= rnd.RandfRange(-5, 5);
					targetPositionOffset.Z= rnd.RandfRange(-5, 5);
				}
			}
			else
			{
				if(targetPositionOffset.X > 5)
				{
					targetPositionOffset.X= rnd.RandfRange(-5, 5);
					targetPositionOffset.Z= rnd.RandfRange(-5, 5);
				}
				NewTargetCounter++;
			}

			newTarget += targetPositionOffset;
		}
		else
		{
			NewTargetCounter =1200;
		}

		NavigationAgent.SetTargetPosition(newTarget);
		targetMesh.GlobalPosition = newTarget;

		Vector3 newPos = NavigationAgent.GetNextPathPosition();
		Vector3 velocity = Vector3.Zero;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		Velocity = velocity +( (newPos - GlobalPosition).Normalized() * speed);
		LookAt(player.GlobalPosition, Vector3.Up);
		MoveAndSlide();
	}
}
