#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;

uniform mat4 MatrixClip;
uniform mat4 MatrixWorld;

out vec3 fragNormal;

void main()
{
    fragNormal = (MatrixWorld * vec4(normal, 0)).xyz;

    gl_Position = MatrixClip * vec4(position, 1.0);
}
