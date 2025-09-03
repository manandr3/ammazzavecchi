using Godot;
using System;

public partial class Main : Node
{
	[Export] public DirectionalLight3D DirectionalLight;
	[Export] public MeshInstance3D TargetMesh; // where the shader is applied
	[Export] private Shader toonShader; // the shader reference that applies the toon effect to all the meshes

	private Vector3 light_rotation = Vector3.Zero;
	private DirectionalLight3D sunlight;

	//update light direction for shader usage
	public override void _Process(double delta)
	{

	}

	public override void _Ready()
	{
		light_rotation.X = 25;
		light_rotation.Y = 0;
		light_rotation.Z = 25;

		sunlight = GetNode<DirectionalLight3D>("DirectionalLight3D");

		Input.MouseMode = Input.MouseModeEnum.Captured;
		SetProcess(true);

		ConvertAllMeshes(GetTree().Root);
	}

	public override void _PhysicsProcess(double delta)
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


		light_rotation.Y += 0.001f;
		sunlight.Rotation = light_rotation;
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionPressed("menu"))
			Input.MouseMode = Input.MouseModeEnum.Visible;
	}
	
	private void ConvertAllMeshes(Node node)
    {
        // Se il nodo è una MeshInstance3D, convertilo
        if (node is MeshInstance3D mesh)
        {
			if (mesh.Mesh != null)
			{
				var originalMaterial = mesh.GetSurfaceOverrideMaterial(0);
				if (originalMaterial is StandardMaterial3D standardMat)
				{
					// Prendi il colore originale
					Color originalColor = standardMat.AlbedoColor;

					// Crea il nuovo ShaderMaterial
					ShaderMaterial shaderMat = new ShaderMaterial();
					shaderMat.Shader = toonShader;
					shaderMat.SetShaderParameter("base_color", originalColor);

					// Applica il nuovo materiale
					mesh.SetSurfaceOverrideMaterial(0, shaderMat);

					GD.Print($"Convertito: {mesh.Name} con colore {originalColor}");
				}
			}
        }

        // Scansiona ricorsivamente tutti i figli
        foreach (Node child in node.GetChildren())
        {
            ConvertAllMeshes(child);
        }
    }
}
