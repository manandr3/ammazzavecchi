shader_type spatial;
render_mode unshaded;

uniform sampler2D screen_texture : source_color, hint_screen_texture, filter_nearest;
uniform sampler2D depth_texture : source_color, hint_depth_texture, filter_nearest;
uniform sampler2D normal_texture : source_color, hint_normal_roughness_texture, filter_nearest;

uniform float color_threshold : hint_range(0,1) = 0.05;
uniform float depth_threshold : hint_range(0,1) = 0.05;
uniform float normal_threshold : hint_range(0,1) = 0.05;

uniform float darken_amount : hint_range(0, 1, 0.01) = 0.3;
uniform float lighten_amount : hint_range(0, 10, 0.01) = 1.5;

uniform float color_limitation : hint_range(2, 256) = 150.0;

uniform vec3 light_direction;

uniform int colors_count = 16;
uniform vec3 palette[16];

float get_depth(vec2 screen_uv, mat4 inv_projection_matrix)
{
	float depth = texture(depth_texture, screen_uv).r;
	vec3 ndc = vec3(screen_uv * 2.0 - 1.0, depth);
	vec4 view = inv_projection_matrix * vec4(ndc, 1.0);
	view.xyz /= view.w;
	return -view.z;
}



void vertex() {
	POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment() {

	vec3 original = texture(screen_texture, SCREEN_UV).rgb;
	vec3 normal = texture(normal_texture, SCREEN_UV).rgb * 2.0 - 1.0;
	float depth = get_depth(SCREEN_UV, INV_PROJECTION_MATRIX);
	vec2 texel_size = 1.0 / VIEWPORT_SIZE.xy;

	vec2 uvs[4];
	uvs[0] = vec2(SCREEN_UV.x, SCREEN_UV.y + texel_size.y);
	uvs[1] = vec2(SCREEN_UV.x, SCREEN_UV.y - texel_size.y);
	uvs[2] = vec2(SCREEN_UV.x + texel_size.x, SCREEN_UV.y);
	uvs[3] = vec2(SCREEN_UV.x - texel_size.x, SCREEN_UV.y);

	float depth_diff = 0.0;
	float normal_sum = 0.0;
	float color_sum = 0.0;
	float nearest_depth = depth;
	vec2 nearest_uv = SCREEN_UV;


	for(int i = 0; i < 4; i++)
	{
		float d = get_depth(uvs[i], INV_PROJECTION_MATRIX);
		depth_diff += depth - d;

		if(d < nearest_depth)
		{
			nearest_depth = d;
			nearest_uv = uvs[i];
		}

		vec3 n = texture(normal_texture, uvs[i]).xyz * 2.0 - 1.0;
		vec3 normal_diff = normal - n;

		vec3 normal_edge_bias = vec3(1,1,1);

		float normal_bias_diff = dot(normal_diff, normal_edge_bias);
		float normal_indicator = smoothstep(0.5, 1, normal_bias_diff);

		normal_sum += dot(normal_diff, normal_diff) * normal_indicator;

	}

	float depth_edge = step(depth_threshold, depth_diff);

	float indicator = sqrt(normal_sum);
	float normal_edge = step(normal_threshold, indicator);

	vec3 nearest = texture(screen_texture, nearest_uv).rgb;

	mat3 view_to_world_normal_mat = mat3(
		INV_VIEW_MATRIX[0].xyz,
		INV_VIEW_MATRIX[1].xyz,
		INV_VIEW_MATRIX[2].xyz
	);
	float ld = dot((view_to_world_normal_mat * normal), normalize(light_direction));



	vec3 edge_mix;
	if(depth_edge > 0.0)
		edge_mix = mix(original, nearest * darken_amount, depth_edge);
	else
		edge_mix = mix(original, original * (ld > 0.0 ? darken_amount : lighten_amount), normal_edge);


	//color limitation

	/*edge_mix.r = round(edge_mix.r * color_limitation) / color_limitation;
	edge_mix.g = round(edge_mix.g * color_limitation) / color_limitation;
	edge_mix.b = round(edge_mix.b * color_limitation) / color_limitation;*/




	//checking if the palette has been correctly imported
	if(palette[0] != vec3(0.0, 0.0, 0.0))
	{
		// Color quantization with the palette
		vec3 final_quantized_color;
		float min_distance_sq = 99999.0;

		// The weights for the weighted Euclidean distance
		vec3 weights = vec3(2.0, 4.0, 3.0);

		for (int i = 0; i < colors_count; ++i) {
			vec3 palette_color = palette[i];

			// Calculate the weighted squared Euclidean distance
			vec3 diff = edge_mix - palette_color;
			float distance_sq = dot(diff * weights, diff * weights);

			if (distance_sq < min_distance_sq) {
				min_distance_sq = distance_sq;
				final_quantized_color = palette_color;
			}
		}

		ALBEDO = final_quantized_color;
	}
	else
	{
		//if the palette is not provided i just clamp down the number of colors
		edge_mix.r = round((edge_mix.r + (edge_mix.r > 0.001 && edge_mix.r < 0.05 ? 0.2 * 2.0 : 0.0)) * float(colors_count / 3)) / float(colors_count / 3);
		edge_mix.g = round((edge_mix.g + (edge_mix.g > 0.001 && edge_mix.g < 0.05 ? 0.2 * 4.0 : 0.0)) * float(colors_count / 3)) / float(colors_count / 3);
		edge_mix.b = round((edge_mix.b + (edge_mix.b > 0.001 && edge_mix.b < 0.05 ? 0.2 * 3.0 : 0.0)) * float(colors_count / 3)) / float(colors_count / 3);
		ALBEDO = edge_mix;
	}

	//ALBEDO = original;
}




//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}