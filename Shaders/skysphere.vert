#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;

uniform mat4 MatrixClip;
uniform vec4 EnvColor;
uniform vec4 EnvColorTop;
uniform vec4 EnvColorMid;
uniform vec4 EnvColorBottom;

out vec4 fragColor;

void main()
{
    // Ambient component
    float d = dot(normal, vec3(0,1,0));
    vec4  skyColor;
    if (d < 0)
        skyColor = mix(EnvColorMid, EnvColorBottom, clamp(-d, 0, 1));
    else
        skyColor = mix(EnvColorMid, EnvColorTop, clamp(d, 0, 1));

    fragColor = EnvColor * skyColor;

    gl_Position = MatrixClip * vec4(position, 1.0);
}
