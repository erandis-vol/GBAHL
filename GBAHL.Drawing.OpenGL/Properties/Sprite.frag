#version 330

// Input
in vec2 fragmentCoords;

// Output
out vec4 fragmentColor;

// Samplers
uniform sampler2D sprite;

void main()
{
	fragmentColor = texture(sprite, fragmentCoords);
}