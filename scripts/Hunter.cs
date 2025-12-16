using Godot;
using System;

public partial class Hunter : Node3D
{
	[Export] public int bullet_speed { get; set; } = 800;
	[Export] public float tray_initial_radius { get; set; } = 0.02f;
	[Export] public float tray_target_radius { get; set; } = 0.12f;
	private static CharacterBody3D Player;
	private static RayCast3D player_ray_cast;
	private static MeshInstance3D bullet_tray;
	private static CapsuleMesh bullet_tray_mesh;

	private static Area3D bullet;
	private static Vector3 starting_bullet_position;
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

		Player = GetNode<CharacterBody3D>("../Player");
		player_ray_cast = GetNode<RayCast3D>("player_ray_cast");
		player_ray_cast.GlobalPosition = GlobalPosition;
		shooting_timer = GetNode<Timer>("shooting_timer");
		bullet_travel_timer = GetNode<Timer>("bullet_travel_timer");
		bullet_tray = GetNode<MeshInstance3D>("hunter/bullet_tray");
		bullet = GetNode<Area3D>("hunter/bullet");

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

		player_ray_cast.TargetPosition = Player.GlobalPosition;

		switch(shooting_status)		//0 waitng, 1 preparing, 2 bullet goes
		{
			case 0:
				ausiliar_material.Emission = new Color(1f, 0f, 0f);
				ausiliar_material.EmissionEnergyMultiplier = 3.0f;
				bullet_tray_mesh.Radius = tray_initial_radius;
				bullet.Visible = false;
				bullet.Position = starting_bullet_position;
				tray_red_color.A = 0f;
				ausiliar_material.AlbedoColor = tray_red_color;
				ausiliar_material.EmissionEnabled = true;
				counter_bullet_visibility = 0;
				LookAt(Player.GlobalPosition, Vector3.Up);
				break;

			case 1:
				ausiliar_material.Emission = new Color(1f, 0.1f, 0.1f);
				ausiliar_material.EmissionEnergyMultiplier = 3.0f;
				bullet_tray_mesh.Radius = Lerp(bullet_tray_mesh.Radius, tray_target_radius, 0.005f);
				GD.Print(bullet_tray_mesh.Radius);
				tray_white_color.A = 1f;
				bullet.Visible = false;
				LookAt(Player.GlobalPosition, Vector3.Up);
				tray_red_color.A = Lerp(tray_red_color.A, 0.8f, 0.007f);
				ausiliar_material.AlbedoColor = tray_red_color;
				ausiliar_material.EmissionEnabled = true;
				break;

			case 2:
				bullet_tray_mesh.Height = starting_bullet_position.DistanceTo(bullet.Position);
				new_bullet_tray_position = bullet.Position;
				new_bullet_tray_position.Z = new_bullet_tray_position.Z / 2.0f;
				bullet_tray.Position =  new_bullet_tray_position;

				if(bullet_tray_mesh.Radius < tray_initial_radius)
				{
					bullet_tray_mesh.Radius = tray_target_radius;
				}

				ausiliar_material.Emission = new Color(0f, 0f, 0f);
				ausiliar_material.EmissionEnergyMultiplier = 3f;
				bullet_tray_mesh.Radius = Lerp(bullet_tray_mesh.Radius, tray_initial_radius, 0.005f);
				GD.Print(bullet_tray_mesh.Radius);
				bullet.Visible = true;
				counter_bullet_visibility += 1;

				tray_white_color.A = Lerp(tray_white_color.A, 0f, 0.003f);

				ausiliar_material.AlbedoColor = tray_white_color;
				ausiliar_material.EmissionEnabled = true;

				bullet_velocity = Transform.Basis.Z * bullet_speed;
				new_bullet_position.X = bullet.GlobalPosition.X - (bullet_velocity.X * (float)(delta));
				new_bullet_position.Y = bullet.GlobalPosition.Y - (bullet_velocity.Y * (float)(delta));
				new_bullet_position.Z = bullet.GlobalPosition.Z - (bullet_velocity.Z * (float)(delta));
				bullet.GlobalPosition = new_bullet_position;

				
				if (counter_bullet_visibility > 1000)
				{
					shooting_timer.Start();
					shooting_status = 0;
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
}
