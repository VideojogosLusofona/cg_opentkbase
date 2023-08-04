#version 330 core
layout (location = 0) in vec3 position;

uniform vec4 MaterialColor = vec4(1,1,0,1);
uniform mat4 MatrixClip;

out vec4 fragColor;

void main()
{
    fragColor = MaterialColor;
    gl_Position = MatrixClip * vec4(position, 1.0);
}
