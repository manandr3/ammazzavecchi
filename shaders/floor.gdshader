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

	// 5. NORMALS
	NORMAL = vec3(0.0, 1.0, 0.0);

}

void fragment() {
	vec4 tex = texture(texture_albedo, UV);
	ALBEDO = albedo.rgb;
	ALPHA = tex.a;
	// Optional: discard very transparent pixels to clean up edges
	if (ALPHA < 0.1) discard;
}