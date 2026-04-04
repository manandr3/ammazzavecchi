shader_type spatial;

render_mode blend_mix, depth_draw_never, cull_disabled, diffuse_lambert_wrap;

uniform vec4 albedo : source_color = vec4(1.0);
uniform sampler2D texture_albedo : source_color, filter_linear, repeat_enable;

uniform float wind_speed : hint_range(0.0, 10.0) = 2.0;
uniform float wind_strength : hint_range(0.0, 2.0) = 0.15;
uniform float tilt_blend : hint_range(0.0, 1.0) = 0.3; // 0.0 is upright, 1.0 is full camera-face

// Controls the "organic lean" (Rotation around local Z)
uniform float random_lean_strength : hint_range(0.0, 1.0) = 0.3; 

float hash(vec2 p) {
	return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

void vertex() {
	// 1. POSITION AND SCALE
	vec3 world_pos = MODEL_MATRIX[3].xyz;
	float scale_x = length(MODEL_MATRIX[0].xyz);
	float scale_y = length(MODEL_MATRIX[1].xyz);
	float scale_z = length(MODEL_MATRIX[2].xyz);

	// 2. THE ORGANIC LEAN (Local Z-Axis Rotation)
	// This makes the blades look less like soldiers and more like nature.
	float rand = hash(world_pos.xz) * 2.0 - 1.0;
	float lean_angle = rand * random_lean_strength;
	
	float cos_a = cos(lean_angle);
	float sin_a = sin(lean_angle);
	

	// Assuming mesh bottom is at -0.5
	float pivot_y = -0.5; 
	/*
	float obj_v_y = VERTEX.y - pivot_y;
	
	// Standard 2D rotation matrix applied to X and Y (Rotating around Z)
	float lean_x = VERTEX.x * cos_a - obj_v_y * sin_a;
	float lean_y = VERTEX.x * sin_a + obj_v_y * cos_a;
	
	VERTEX.x = lean_x;
	VERTEX.y = lean_y + pivot_y;
	*/
	
	VERTEX.z = VERTEX.z * cos_a - VERTEX.z * sin_a;
	
	
	

	// 3. WIND SWAY
	float height_mask = VERTEX.y - pivot_y; 
	float sway = sin(TIME * wind_speed + world_pos.x + world_pos.z) * wind_strength;
	VERTEX.x += sway * height_mask;

	// 4. CONSTRAINED BILLBOARDING
	// We construct the basis vectors manually to ensure Y stays mostly vertical.
	vec3 right_dir = INV_VIEW_MATRIX[0].xyz; // Stay parallel to camera's horizontal
	
	// Mix world UP with camera UP to prevent flattening
	vec3 up_dir = normalize(mix(vec3(0.0, 1.0, 0.0), INV_VIEW_MATRIX[1].xyz, tilt_blend));
	
	// Generate the forward vector (Z) from Right and Up
	vec3 back_dir = normalize(cross(right_dir, up_dir));
	
	// Re-align right vector to keep everything square
	right_dir = normalize(cross(up_dir, back_dir));

	mat4 billboard_matrix = mat4(
		vec4(right_dir * scale_x, 0.0),
		vec4(up_dir * scale_y, 0.0),
		vec4(back_dir * scale_z, 0.0),
		vec4(world_pos, 1.0)
	);
	
	MODELVIEW_MATRIX = VIEW_MATRIX * billboard_matrix;

	// 5. NORMALS
	NORMAL = vec3(0.0, 1.0, 0.0);
}

void fragment() {
	vec4 tex = texture(texture_albedo, UV);
	ALBEDO = albedo.rgb* tex.rgb;
	ALPHA = tex.a;
	// Optional: discard very transparent pixels to clean up edges
	if (ALPHA < 0.1) discard; 
}