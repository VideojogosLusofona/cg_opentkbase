#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 uv;

uniform mat4        MatrixClip;
uniform mat4        MatrixWorld;

out vec3 fragPos;
out vec3 fragNormal;
out vec2 fragUV;

void main()
{
    fragPos = (MatrixWorld * vec4(position, 1)).xyz;
    fragNormal = (MatrixWorld * vec4(normal, 0)).xyz;
    fragUV = uv;

    gl_Position = MatrixClip * vec4(position, 1.0);
}
