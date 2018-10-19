#version 330

// Input
in vec2 fragmentCoords;

// Output
out vec4 fragmentColor;

// Samplers
uniform sampler2D imageTexture;
uniform sampler2D colorTexture;

void main()
{
	// Determine the color of the pixel
	vec4 index = texture(imageTexture, fragmentCoords);
	vec4 pixel = texture(colorTexture, index.xy);

	// Pass on the color
	fragmentColor = pixel;
}