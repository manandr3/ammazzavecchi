using Godot;
using System;

public partial class Main : Node
{
	[Export] public DirectionalLight3D DirectionalLight;
	[Export] public MeshInstance3D TargetMesh; // Dove hai applicato lo shader
	
	private Vector3 light_rotation = Vector3.Zero;
	
	//update light direction for shader usage
	public override void _Process(double delta)
	{
		if (DirectionalLight == null || TargetMesh == null)
			return;

		// Ottieni la direzione della luce (asse -Z del transform)
		Vector3 dir = -DirectionalLight.GlobalTransform.Basis.Z.Normalized();

		// Aggiorna il parametro nello shader
		var mat = TargetMesh.GetActiveMaterial(0) as ShaderMaterial;
		if (mat != null)
		{
			mat.SetShaderParameter("light_direction", dir);
		}
		
		var sunlight = GetNode<DirectionalLight3D>("DirectionalLight3D");
		light_rotation.Y += 0.0001f;
		sunlight.Rotation = light_rotation;
	}
	
	public override void _Ready()
	{
		light_rotation.X = 25;
		light_rotation.Y = 0;
		light_rotation.Z = 25;
		
		Input.MouseMode = Input.MouseModeEnum.Captured;
		SetProcess(true);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionPressed("menu"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}
}
