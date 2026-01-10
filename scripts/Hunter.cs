using Godot;
using System;

public partial class Hunter : Node3D
{
	[Export] public int bullet_speed { get; set; } = 1000;
	[Export] public float tray_initial_radius { get; set; } = 0.02f;
	[Export] public float tray_target_radius { get; set; } = 0.12f;
	
	private static Node3D CollisionTarget;

	//test
	private static Node3D CollisionPoint;
	//test

	private static CharacterBody3D Player;
	private static RayCast3D player_ray_cast;
	private static MeshInstance3D bullet_tray;
	private static CapsuleMesh bullet_tray_mesh;

	public static bool ObjectHitStatus;

	private static Area3D bullet;
	private static Vector3 starting_bullet_position;
	private static Vector3 ending_bullet_position;
	private static Vector3 bullet_velocity;
	private static Vector3 new_bullet_position;
	private static Vector3 new_bullet_tray_position;
	private static Timer shooting_timer;
	private static Timer bullet_travel_timer;
	private int shooting_status; //0 waitng, 1 preparing, 2 bullet goes
	private int counter_bullet_visibility; //used to determine for how long the bullet tray is visible after passing by
	private StandardMaterial3D ausiliar_material;
	private Color tray_red_color;
	private Color tray_white_color;
	public override void _Ready()
	{
		counter_bullet_visibility = 0;

		//test
		CollisionPoint = GetNode<Node3D>("SpringArmCollision/CollisionPoint");
		//test
		CollisionTarget = GetNode<Node3D>("CollisionTarget");

		Player = GetNode<CharacterBody3D>("../Player");
		player_ray_cast = GetNode<RayCast3D>("player_ray_cast");
		shooting_timer = GetNode<Timer>("shooting_timer");
		bullet_travel_timer = GetNode<Timer>("bullet_travel_timer");
		bullet_tray = GetNode<MeshInstance3D>("bullet_tray");
		bullet = GetNode<Area3D>("bullet");

		bullet_tray.Mesh = bullet_tray.Mesh.Duplicate() as Mesh;
		bullet_tray_mesh = bullet_tray.Mesh as CapsuleMesh;

		bullet_tray_mesh.Radius = tray_initial_radius;

		bullet.Visible = false;
		LookAt(Player.GlobalPosition, Vector3.Up);
		starting_bullet_position = bullet.Position;
		shooting_timer.Start();
		shooting_status = 0;


		tray_red_color = new Color(1f, 0.1f, 0.1f, 0f);
		tray_white_color = new Color(1f, 1f, 1f, 0f);
		ausiliar_material = new StandardMaterial3D() { AlbedoColor = tray_red_color };
		ausiliar_material.EmissionEnabled = true;
		ausiliar_material.Emission = new Color(1f, 1f, 1f);
    	ausiliar_material.EmissionEnergyMultiplier = 3.0f;
		ausiliar_material.Transparency = BaseMaterial3D.TransparencyEnum.AlphaHash;
		bullet_tray.SetSurfaceOverrideMaterial(0, ausiliar_material);

	}

	public override void _Process(double delta)
	{
		switch(shooting_status)		//0 waitng, 1 preparing, 2 bullet goes
		{
			case 0:
				bullet_tray_mesh.Height = 2000;
				ausiliar_material.Emission = new Color(1f, 0f, 0f);
				ausiliar_material.EmissionEnergyMultiplier = 3.0f;
				bullet_tray_mesh.Radius = tray_initial_radius;
				bullet.Visible = false;
				tray_red_color.A = 0f;
				ausiliar_material.AlbedoColor = tray_red_color;
				ausiliar_material.EmissionEnabled = true;
				counter_bullet_visibility = 0;
				break;

			case 1:
				ausiliar_material.Emission = new Color(1f, 0.1f, 0.1f);
				ausiliar_material.EmissionEnergyMultiplier = 3.0f;
				bullet_tray_mesh.Radius = Lerp(bullet_tray_mesh.Radius, tray_target_radius, 0.005f);
				tray_white_color.A = 1f;
				bullet.Visible = false;
				tray_red_color.A = Lerp(tray_red_color.A, 0.8f, 0.007f);
				ausiliar_material.AlbedoColor = tray_red_color;
				ausiliar_material.EmissionEnabled = true;
				break;

			case 2:
				if(bullet_tray_mesh.Radius < tray_initial_radius)
				{
					bullet_tray_mesh.Radius = tray_target_radius;
				}

				if(counter_bullet_visibility == 0)
				{
					
					GD.Print("\n\n");

					bullet.Visible = true;
				}

				ausiliar_material.Emission = new Color(0f, 0f, 0f);
				ausiliar_material.EmissionEnergyMultiplier = 3f;
				bullet_tray_mesh.Radius = Lerp(bullet_tray_mesh.Radius, tray_initial_radius, 0.005f);

				tray_white_color.A = Lerp(tray_white_color.A, 0f, 0.003f);

				ausiliar_material.AlbedoColor = tray_white_color;
				ausiliar_material.EmissionEnabled = true;
				
				counter_bullet_visibility += 1;
				
				if (counter_bullet_visibility > 1000)
				{
					shooting_timer.Start();
					shooting_status = 0;
				}
				
			break;
        }
	}

	public override void _PhysicsProcess(double delta)
	{
		switch(shooting_status)		//0 waitng, 1 preparing, 2 bullet goes
		{
			case 0:
				ObjectHitStatus = false;
				bullet.Position = starting_bullet_position;
				LookAt(Player.GlobalPosition, Vector3.Up);
				break;

			case 1:
				LookAt(Player.GlobalPosition, Vector3.Up);
				break;

			case 2:
				if(!ObjectHitStatus)
				{
					bullet_velocity = Transform.Basis.Z * bullet_speed;
					new_bullet_position.X = bullet.GlobalPosition.X - (bullet_velocity.X * (float)(delta));
					new_bullet_position.Y = bullet.GlobalPosition.Y - (bullet_velocity.Y * (float)(delta));
					new_bullet_position.Z = bullet.GlobalPosition.Z - (bullet_velocity.Z * (float)(delta));

					if(player_ray_cast.IsColliding())
					{
						ending_bullet_position = player_ray_cast.GetCollisionPoint();
					}

					//test
					ending_bullet_position = CollisionPoint.GlobalPosition;
					//test


					//GD.Print((this.GlobalPosition.DistanceTo(new_bullet_position) + 25f) + "             >=            " + this.GlobalPosition.DistanceTo(ending_bullet_position));

					if((this.GlobalPosition.DistanceTo(new_bullet_position) + 25f) >= this.GlobalPosition.DistanceTo(ending_bullet_position) && player_ray_cast.IsColliding()) //trying to eliminate the hitting thru walls problem
					{
						bullet.Position = new Vector3(0f ,0f ,this.GlobalPosition.DistanceTo(ending_bullet_position));
						ObjectHitStatus = true;
					}
					else
					{
						bullet.GlobalPosition = new_bullet_position;
					}

					bullet_tray_mesh.Height = starting_bullet_position.DistanceTo(bullet.Position)*2;
					new_bullet_tray_position = bullet.Position;
					new_bullet_tray_position.Z = new_bullet_tray_position.Z / 2.0f;
					bullet_tray.Position =  new_bullet_tray_position;
				}
				else
				{
					bullet.Position = starting_bullet_position;
					bullet.Visible = false;
					bullet_tray_mesh.Height = this.GlobalPosition.DistanceTo(ending_bullet_position);
					bullet_tray.Position = new Vector3(0,0,0-(this.GlobalPosition.DistanceTo(ending_bullet_position)/2));
				}

				break;
        }
	}

	public void OnShootingTimerTimeout()
	{
		//once the bullet is fired the sequence of the bullet traveling is activated
		shooting_status = 1;
		shooting_timer.Stop();
		bullet_travel_timer.Start();
	}


	public void OnBulletTravelTimerTimeout()
	{
		shooting_status = 2;
		bullet_travel_timer.Stop();
	}


	public static float Lerp(float First, float Second, float Amount)
	{
		return First * (1 - Amount) + Second * Amount;
	}


	public void OnBulletHitArea(Area3D area3d)
	{
		GD.Print("yaaaaa");
	}

	public void OnBulletHitBody(Node3D body)
	{
		if((body is StaticBody3D) && !ObjectHitStatus)
		{
			GD.Print("colpito l'oggetto\n\n");
			ObjectHitStatus = true;
		}
		else if(body is CharacterBody3D)
		{
			GD.Print("colpito il giocatore\n\n");
		}
	}
}
