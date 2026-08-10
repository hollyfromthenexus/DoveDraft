// Thanks to https://www.youtube.com/watch?v=07A_6dLihic
shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
varying vec4 vertex_color;

void vertex() {
	vertex_color = COLOR;
}

void fragment() {
	vec4 texture_color = texture(TEXTURE, UV) * vertex_color;
	vec4 inverted_color = texture(SCREEN_TEXTURE, SCREEN_UV);
	COLOR = vec4(1.0 - inverted_color.rgb, texture_color.a);
}
