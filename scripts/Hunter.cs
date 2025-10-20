using Godot;
using System;

public partial class Hunter : Node3D
{
	private static CharacterBody3D Player;
	public override void _Ready()
	{
		//player object to access its position
		Player = GetNode<CharacterBody3D>("Player");
	}
	
	public override void _Process(double delta)
    {
    }
}
