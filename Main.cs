using Godot;
using System;

public partial class Main : Node
{
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionPressed("menu"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}
}
