using Godot;
using System;

public partial class Bullet : Area3D
{
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

	private void OnAreaEntered(Area3D area3d)
	{
		GD.Print(area3d);
	}

	private void OnBodyEntered(Node3D body)
	{
        GD.Print(body);
		GD.Print(this.GlobalPosition.DistanceTo(this.GetParent<MeshInstance3D>().GlobalPosition));
    }


}
