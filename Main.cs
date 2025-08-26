using Godot;
using System;

public partial class Main : Node
{
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
}
