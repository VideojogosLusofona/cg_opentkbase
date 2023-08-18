#version 330 core
layout (location = 0) in vec3 position;

uniform mat4        MatrixClip;

void main()
{
    gl_Position = MatrixClip * vec4(position, 1.0);
}
