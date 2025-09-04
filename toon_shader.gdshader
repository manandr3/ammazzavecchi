shader_type spatial;

// The base color of the object. 'source_color' provides a color picker in the editor.
uniform vec4 albedo : source_color;

// The number of discrete steps/bands for the light. 
// A higher number gives a smoother transition.
uniform int steps = 2;

// The strength of the specular highlight.
uniform float specular_strength = 0.5;

// The smoothness of the specular highlight.
uniform float specular_smoothness = 16.0;

// Custom uniform for the light direction, set from a script.
uniform vec3 light_direction;

void fragment() {
    // Standard lighting calculation using the dot product of the normal and light direction.
    // This value, NdotL, ranges from -1 (fully shaded) to 1 (fully lit).
    float NdotL = dot(NORMAL, light_direction);

    // FIX: Clamp NdotL to a positive range (0.0 to 1.0) to prevent negative values from causing blackness.
    NdotL = max(0.0, NdotL);

    // --- Core Toon Shading Logic ---
    // We quantize the NdotL value to create discrete bands of light and shadow.
    // This is the "cel-shading" effect.
    float light_band = floor(NdotL * float(steps)) / float(steps);

    // Clamp the value to ensure it stays within the 0.0 to 1.0 range.
    light_band = clamp(light_band, 0.0, 1.0);

    // Apply the stepped lighting to the albedo color.
    vec3 toon_color = albedo.rgb * light_band;

    // --- Specular Highlight Logic ---
    // We calculate a simple specular highlight using the same light direction.
    vec3 view_dir = normalize(VIEW);
    vec3 half_vector = normalize(light_direction + view_dir);

    // The dot product of the normal and the half vector gives the highlight intensity.
    float specular_intensity = dot(NORMAL, half_vector);

    // Clamp and power the intensity to create a sharp, focused highlight.
    specular_intensity = pow(clamp(specular_intensity, 0.0, 1.0), specular_smoothness);

    // We also step the specular highlight to match the toon shading style.
    float specular_band = floor(specular_intensity * float(steps)) / float(steps);
    specular_band = clamp(specular_band, 0.0, 1.0);

    // Combine the stepped color with the stepped specular highlight.
    vec3 final_color = toon_color + (vec3(1.0) * specular_band * specular_strength);
    
    // Set the final albedo and roughness.
    //ALBEDO = final_color;
	ALBEDO = toon_color;
    ROUGHNESS = 1.0;
	
	ALBEDO = albedo.rgb;
}
