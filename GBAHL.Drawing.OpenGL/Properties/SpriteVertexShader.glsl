#version 330

// Input
layout(location = 0) in vec2 inXY;
layout(location = 1) in vec2 inUV;

// Output
out vec2 fragmentCoords;

// Uniform
uniform mat4 mvp;

void main()
{
	// Pass the texture UV onto the fragment shader
	fragmentCoords = inUV;

	// Apply the model-view-projection matrix to the vertex
	gl_Position = mvp * vec4(inXY, 0.0, 1.0);
}