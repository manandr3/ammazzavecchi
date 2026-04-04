using Godot;
using System;

public partial class Bullet : Area3D
{
	
	[Signal] public delegate void BulletHitAreaEventHandler(Area3D area3d);
	[Signal] public delegate void BulletHitBodyEventHandler(Node3D body);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnAreaEntered(Area3D area3d) //function called when bullet enters an area3d
	{
		EmitSignal(SignalName.BulletHitArea, area3d); //I emit a custom signal for the hunter to let it know that the bullet has entered an area3d
	}

	public void OnBodyEntered(Node3D body) //function called when bullet enters a body (CharacterBody and StaticBody are counted as bodies)
	{
		EmitSignal(SignalName.BulletHitBody, body); //I emit a custom signal for the hunter to let it know that the bullet has entered a body
    }
}
