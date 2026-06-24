shader_type spatial;

render_mode blend_mix, cull_disabled, diffuse_lambert_wrap;

uniform vec4 albedo : source_color = vec4(1.0);
uniform sampler2D texture_albedo : source_color, filter_linear, repeat_enable;



void fragment() {
	vec4 tex = texture(texture_albedo, UV);
	ALBEDO = albedo.rgb;
}