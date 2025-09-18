using Godot;
using System;

public partial class AmbienteProva : Node
{

    [Export] public DirectionalLight3D DirectionalLight;
	[Export] public MeshInstance3D TargetMesh; // where the shader is applied
	[Export] private Shader toonShader; // the shader reference that applies the toon effect to all the meshes

	private Vector3 light_rotation = Vector3.Zero;
	private DirectionalLight3D sunlight;


    public override void _Ready()
	{

		var palette = new Godot.Collections.Array<Color>()
		{
			/*
			// Greens (from the grass and trees)
			new Color(0.0f, 0.03f, 0.0f),
            new Color(0.0f, 0.1f, 0.0f),
            new Color(0.0f, 0.2f, 0.0f),
            new Color(0.0f, 0.35f, 0.0f),
            new Color(0.15f, 0.5f, 0.15f),
            new Color(0.25f, 0.6f, 0.25f),

            // Browns (from the tree trunks)
            new Color(0.03f, 0.02f, 0.0f),
            new Color(0.3f, 0.15f, 0.0f),
            new Color(0.4f, 0.25f, 0.1f),

            // Grays (from the stone pillars and stairs)
            new Color(0.2f, 0.2f, 0.2f),
            new Color(0.3f, 0.3f, 0.3f),
            new Color(0.5f, 0.5f, 0.5f),
            new Color(0.7f, 0.7f, 0.7f),
            new Color(0.9f, 0.9f, 0.9f),

            // Accent/Highlight colors
            new Color(0.9f, 0.8f, 0.6f),
            new Color(0.6f, 0.7f, 0.9f),
            
            // Core colors for contrast
            //new Color(0.0f, 0.0f, 0.0f),
            //new Color(1.0f, 1.0f, 1.0f)
			clanker*/

			//foliage
			/*new Color(0.0f, 0.21f, 0.08f),
			new Color(0.0f, 0.81f, 0.38f),
			new Color(0.0f, 0.91f, 0.58f),*/

			new Color(0.0f, 0.24f, 0.09f),
			new Color(0.0f, 0.46f, 0.21f),
			new Color(0.0f, 0.6f, 0.3f),
			new Color(0.0f, 1.0f, 0.48f),


			//trunks
			new Color(0.10f, 0.03f, 0.0f),
			new Color(0.41f, 0.19f, 0.0f),
			new Color(0.82f, 0.41f, 0.0f),

			//marble
			new Color(0.19f, 0.21f, 0.24f),
			new Color(0.66f, 0.75f, 0.81f),
			new Color(0.78f, 0.88f, 0.94f),

			//stone
			new Color(0.3f, 0.3f, 0.3f),
			new Color(0.5f, 0.5f, 0.5f),
			new Color(0.66f, 0.66f, 0.66f),

			//ground
			new Color(0.007f, 0.05f, 0.0f),
			new Color(0.07f, 0.26f, 0.0f),
			new Color(0.23f, 0.66f, 0.0f),
			new Color(0.35f, 1.0f, 0.0f)
        };
		
		//if the shader is correctly detected i pass it the palette
		var mat = TargetMesh.GetActiveMaterial(0) as ShaderMaterial;
		if (mat != null)
		{
			mat.SetShaderParameter("palette", palette);
		}

		//
		sunlight = GetNode<DirectionalLight3D>("DirectionalLight3D");

		//when the game is started the mouse is captured
		Input.MouseMode = Input.MouseModeEnum.Captured;

		SetProcess(true);

		//ConvertAllMeshes(GetTree().Root);
		
		light_rotation.X = -30 * (MathF.PI/180);
		light_rotation.Y = -23 * (MathF.PI/180);
		light_rotation.Y = 32 * (MathF.PI/180);
	}
}
