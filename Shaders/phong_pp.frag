#version 330 core

uniform vec4 MaterialColor = vec4(1,1,0,1);
uniform vec2 MaterialSpecular = vec2(0,1);
uniform vec4 MaterialColorEmissive = vec4(0,0,0,1);
uniform vec4 EnvColor;
uniform vec4 EnvColorTop;
uniform vec4 EnvColorMid;
uniform vec4 EnvColorBottom;
uniform vec3 ViewPos;

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

vec3 ComputeDirectional(Light light, vec3 worldPos, vec3 worldNormal)
{
    float d = clamp(-dot(worldNormal, light.direction), 0, 1);
    vec3  v = normalize(ViewPos - worldPos);
    // Light dir is from light to point, but we want the other way around, hence the V - L
    vec3  h =  normalize(v - light.direction);
    float s = MaterialSpecular.x * pow(max(dot(h, worldNormal), 0), MaterialSpecular.y);

    return clamp(d * MaterialColor.xyz + s, 0, 1) * light.color.rgb * light.intensity;
}

vec3 ComputePoint(Light light, vec3 worldPos, vec3 worldNormal)
{
    vec3  lightDir = normalize(worldPos - light.position);
    float d = clamp(-dot(worldNormal, lightDir), 0, 1);
    vec3  v = normalize(ViewPos - worldPos);
    // Light dir is from light to point, but we want the other way around, hence the V - L
    vec3  h =  normalize(v - lightDir);
    float s = MaterialSpecular.x * pow(max(dot(h, worldNormal), 0), MaterialSpecular.y);

    return clamp(d * MaterialColor.xyz + s, 0, 1) * light.color.rgb * light.intensity;
}

vec3 ComputeSpot(Light light, vec3 worldPos, vec3 worldNormal)
{
    vec3  lightDir = normalize(worldPos - light.position);
    float d = clamp(-dot(worldNormal, lightDir), 0, 1);
    float spot = (acos(dot(lightDir, light.direction)) - light.spot.x) / (light.spot.y - light.spot.x);

    d = d * mix(1, 0, clamp(spot, 0, 1));

    vec3  v = normalize(ViewPos - worldPos);
    // Light dir is from light to point, but we want the other way around, hence the V - L
    vec3  h =  normalize(v - lightDir);
    float s = MaterialSpecular.x * pow(max(dot(h, worldNormal), 0), MaterialSpecular.y);
    
    return clamp(d * MaterialColor.xyz + s, 0, 1) * light.color.rgb * light.intensity;
}

vec3 ComputeLight(Light light, vec3 worldPos, vec3 worldNormal)
{
    if (light.type == 0)
    {
        return ComputeDirectional(light, worldPos, worldNormal);
    }
    else if (light.type == 1)
    {
        return ComputePoint(light, worldPos, worldNormal);
    }
    else if (light.type == 2)
    {
        return ComputeSpot(light, worldPos, worldNormal);
    }
}

in vec3 fragPos;
in vec3 fragNormal;

out vec4 OutputColor;

void main()
{
    vec3 worldPos = fragPos.xyz;
    vec3 worldNormal = normalize(fragNormal.xyz);

    // Ambient component
    float d = dot(worldNormal, vec3(0,1,0));
    vec4  skyColor;
    if (d < 0)
        skyColor = mix(EnvColorMid, EnvColorBottom, clamp(-d, 0, 1));
    else
        skyColor = mix(EnvColorMid, EnvColorTop, clamp(d, 0, 1));

    vec3 envLighting = EnvColor.xyz * MaterialColor.xyz * skyColor.xyz;

    // Emissive component
    vec3 emissiveLighting = MaterialColorEmissive.rgb;

    // Direct light
    vec3 directLight = vec3(0,0,0);
    for (int i = 0; i < LightCount; i++)
    {
        directLight += ComputeLight(Lights[i], worldPos, worldNormal);
    }    

    // Add all lighting components
    OutputColor = vec4(envLighting + emissiveLighting + directLight, 1);
}
