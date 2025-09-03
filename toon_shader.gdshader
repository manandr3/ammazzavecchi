shader_type spatial;
//render_mode unshaded;

uniform vec4 base_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform int steps = 8;

void fragment() {
    ALBEDO = base_color.rgb;
}

void light() {
    float intensity = max(dot(NORMAL, LIGHT), 0.0);
    float toon = floor(intensity * float(steps)) / float(steps);
    DIFFUSE_LIGHT = base_color.rgb * toon * LIGHT_COLOR.rgb;
}