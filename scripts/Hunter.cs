using Godot;
using System;

public partial class Hunter : Node3D
{
	private static CharacterBody3D Player;
	private static MeshInstance3D bullet_tray;
	private static Timer shooting_timer;
	private static Timer bullet_travel_timer;
	private int shooting_status; //0 waitng, 1 preparing, 2 bullet goes
	private int counter_bullet_visibility; //used to determine for how long the bullet tray is visible after passing by
	private StandardMaterial3D ausiliar_material;
	private Color ausiliar_color;
	public override void _Ready()
	{
		Player = GetNode<CharacterBody3D>("../Player");
		counter_bullet_visibility = 0;
		shooting_timer = GetNode<Timer>("shooting_timer");
		bullet_travel_timer = GetNode<Timer>("bullet_travel_timer");
		bullet_tray = GetNode<MeshInstance3D>("hunter/bullet_tray");
		shooting_timer.Start();
		shooting_status = 0;


		ausiliar_color = new Color(1f, 1f, 1f, 0f);
		ausiliar_material = new StandardMaterial3D() { AlbedoColor = ausiliar_color };
		ausiliar_material.EmissionEnabled = false;
		ausiliar_material.Emission = new Color(1f, 1f, 1f);
    	ausiliar_material.EmissionEnergyMultiplier = 3.0f;
		ausiliar_material.Transparency = BaseMaterial3D.TransparencyEnum.AlphaHash;
		bullet_tray.SetSurfaceOverrideMaterial(0, ausiliar_material);

	}

	public override void _Process(double delta)
	{
		switch(shooting_status)
		{
			case 0:
				ausiliar_color.A = 0f;
				ausiliar_material.AlbedoColor = ausiliar_color;
				ausiliar_material.EmissionEnabled = false;
				counter_bullet_visibility = 0;
				LookAt(Player.GlobalPosition, Vector3.Up);
				break;

			case 1:
				LookAt(Player.GlobalPosition, Vector3.Up);
				ausiliar_color.A = Lerp(ausiliar_color.A, 1f, 0.0005f);
				ausiliar_material.AlbedoColor = ausiliar_color;
				ausiliar_material.EmissionEnabled = false;
				break;

			case 2:
				counter_bullet_visibility += 1;
				if (counter_bullet_visibility > 200)
				{
					ausiliar_color.A = 1f;
					ausiliar_material.AlbedoColor = ausiliar_color;
					ausiliar_material.EmissionEnabled = true;
				}
				else
				{
					ausiliar_color.A = Lerp(ausiliar_color.A, 1f, 0.0005f);
					ausiliar_material.AlbedoColor = ausiliar_color;
					ausiliar_material.EmissionEnabled = false;
				}
				
				if (counter_bullet_visibility > 400)
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
