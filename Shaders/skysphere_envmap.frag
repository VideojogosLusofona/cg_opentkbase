#version 330 core

in vec3 fragNormal;

uniform samplerCube EnvTextureCubeMap;

out vec4 OutputColor;

void main()
{
    OutputColor = texture(EnvTextureCubeMap, fragNormal);
}
