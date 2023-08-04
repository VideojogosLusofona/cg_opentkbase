#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;

uniform vec4 MaterialColor = vec4(1,1,0,1);
uniform mat4 MatrixClip;
uniform mat4 MatrixWorld;

out vec4 fragColor;

void main()
{
    vec3 worldNormal = (MatrixWorld * vec4(normal, 0)).xyz;
    fragColor = vec4(worldNormal * 0.5 + 0.5, 1);
    gl_Position = MatrixClip * vec4(position, 1.0);
}
