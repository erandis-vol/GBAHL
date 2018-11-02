#version 330

layout(location = 0) in vec2 spritePosition;
layout(location = 1) in vec2 spriteTextureCoords;

out vec2 fragmentCoords;

void main()
{
	fragmentCoords = spriteTextureCoords;
	gl_Position = vec4(spritePosition, 0.0, 1.0);
}