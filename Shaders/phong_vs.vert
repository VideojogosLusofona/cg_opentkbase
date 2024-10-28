#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;

uniform vec4 MaterialColor = vec4(1,1,0,1);
<<<<<<< HEAD:Shaders/phong.vert
uniform vec4 MaterialColorEmissive = vec4(0,0,0,1);
=======
uniform vec4 EnvColor = vec4(1,1, 1, 1);
>>>>>>> 79db9ea55bd71eb28d8867739a7f78b337978b6a:Shaders/phong_vs.vert
uniform mat4 MatrixClip;
uniform mat4 MatrixWorld;
uniform vec4 EnvColor;
uniform vec4 EnvColorTop;
uniform vec4 EnvColorMid;
uniform vec4 EnvColorBottom;

const int MAX_LIGHTS = 8;
struct Light
{
    int     type;
    vec3    position;
    vec3    direction;
    vec4    color;
    float   intensity;
    vec2    spot;
};
uniform int     LightCount;
uniform Light   Lights[MAX_LIGHTS];

out vec4 fragColor;

vec3 ComputeLight(Light light, vec3 worldPos, vec3 worldNormal)
{
    if (light.type == 0)
    {
        float d = clamp(-dot(worldNormal, light.direction), 0, 1);

        return vec3(d, d, d);
    }
}

void main()
{
<<<<<<< HEAD:Shaders/phong.vert
    vec3 worldPos = (MatrixWorld * vec4(position, 1)).xyz;
    vec3 worldNormal = (MatrixWorld * vec4(normal, 0)).xyz;

    // Ambient component
    float d = dot(worldNormal, vec3(0,1,0));
    vec4  skyColor;
    if (d < 0)
        skyColor = mix(EnvColorMid, EnvColorBottom, clamp(-d, 0, 1));
    else
        skyColor = mix(EnvColorMid, EnvColorTop, clamp(d, 0, 1));

    vec4 envLighting = EnvColor * MaterialColor * skyColor;

    // Emissive component
    vec4 emissiveLighting = vec4(MaterialColorEmissive.rgb, 0);

    // Direct light
    vec3 directLight = vec3(0,0,0);
    for (int i = 0; i < LightCount; i++)
    {
        directLight += ComputeLight(Lights[i], worldPos, worldNormal);
    }    

    // Add all lighting components
    fragColor = envLighting + emissiveLighting + vec4(directLight * MaterialColor.xyz, 0);

=======
    fragColor = MaterialColor * EnvColor;
>>>>>>> 79db9ea55bd71eb28d8867739a7f78b337978b6a:Shaders/phong_vs.vert
    gl_Position = MatrixClip * vec4(position, 1.0);
}
