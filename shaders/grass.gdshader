shader_type spatial;

// 'depth_draw_never' keeps the grass out of your edge detection shader.
// 'cull_disabled' ensures the grass doesn't vanish when it rotates.
render_mode blend_mix, depth_draw_never, cull_disabled, diffuse_lambert_wrap;

uniform vec4 albedo : source_color = vec4(1.0);
uniform sampler2D texture_albedo : source_color, filter_linear, repeat_enable;

uniform float wind_speed : hint_range(0.0, 10.0) = 2.0;
uniform float wind_strength : hint_range(0.0, 2.0) = 0.15;

void vertex() {
	// 1. GET WORLD POSITION AND SCALE
	vec3 world_pos = MODEL_MATRIX[3].xyz;
	float scale_x = length(MODEL_MATRIX[0].xyz);
	float scale_y = length(MODEL_MATRIX[1].xyz);
	float scale_z = length(MODEL_MATRIX[2].xyz);

	// 2. WIND SWAY
	// Applying sway BEFORE the billboard math ensures it waves in local space.
	float height_mask = VERTEX.y + 0.5; 
	float sway = sin(TIME * wind_speed + world_pos.x + world_pos.z) * wind_strength;
	VERTEX.x += sway * height_mask;

	// 3. SPHERICAL BILLBOARD MATH
	// This replaces the "parallel" rotation with a "face-the-camera" rotation.
	// We use the INV_VIEW_MATRIX to get the camera's orientation vectors.
	mat4 billboard_matrix = mat4(
		vec4(INV_VIEW_MATRIX[0].xyz * scale_x, 0.0), // Camera Right
		vec4(INV_VIEW_MATRIX[1].xyz * scale_y, 0.0), // Camera Up
		vec4(INV_VIEW_MATRIX[2].xyz * scale_z, 0.0), // Camera Back
		vec4(world_pos, 1.0)                         // Mesh Position
	);
	
	// Apply the new matrix to the model-view
	MODELVIEW_MATRIX = VIEW_MATRIX * billboard_matrix;

	// 4. SHADOW FIX
	// Pointing normals up so shadows from trees/pillars look correct on the ground.
	NORMAL = vec3(0.0, 1.0, 0.0);
}

void fragment() {
	vec4 tex = texture(texture_albedo, UV);
	ALBEDO = albedo.rgb * tex.rgb;
	ALPHA = tex.a;
}

void light() {
	// Receives shadows (ATTENUATION) while ignoring the mesh's tilt relative to the sun.
	DIFFUSE_LIGHT += LIGHT_COLOR * ALBEDO * ATTENUATION;
}