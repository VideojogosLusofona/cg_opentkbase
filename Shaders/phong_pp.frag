#version 330 core

uniform vec4    MaterialColor = vec4(1,1,0,1);
uniform vec2    MaterialSpecular = vec2(0,1);
uniform vec4    MaterialColorEmissive = vec4(0,0,0,1);
uniform vec4    EnvColor;
uniform vec4    EnvColorTop;
uniform vec4    EnvColorMid;
uniform vec4    EnvColorBottom;
uniform float   EnvFogDensity;
uniform vec4    EnvFogColor;
uniform vec3    ViewPos;

uniform sampler2D   TextureBaseColor;
uniform sampler2D   TextureNormalMap;
uniform samplerCube EnvTextureCubeMap;

uniform bool        HasTextureBaseColor;
uniform bool        HasTextureNormalMap;

const int MAX_LIGHTS = 8;
struct Light
{
    int             type;
    vec3            position;
    vec3            direction;
    vec4            color;
    float           intensity;
    vec4            spot;
    float           range;
    bool            shadowmapEnable;
    //sampler2DShadow shadowmap;
    sampler2D       shadowmap;
    mat4            shadowMatrix;
};
uniform int     LightCount;
uniform Light   Lights[MAX_LIGHTS];

float saturate(float v)
{
    return clamp(v, 0, 1);
}

float ComputeAttenuation(Light light, vec3 worldPos)
{
    float d = length(worldPos - light.position) / light.range;

    return saturate(saturate(5 * (1 - d)) / (1 + 25 * d * d));
}

vec3 ComputeDirectional(Light light, vec3 worldPos, vec3 worldNormal, vec4 materialColor)
{
    float d = clamp(-dot(worldNormal, light.direction), 0, 1);
    vec3  v = normalize(ViewPos - worldPos);
    // Light dir is from light to point, but we want the other way around, hence the V - L
    vec3  h =  normalize(v - light.direction);
    float s = MaterialSpecular.x * pow(max(dot(h, worldNormal), 0), MaterialSpecular.y);

    return clamp(d * materialColor.xyz + s, 0, 1) * light.color.rgb * light.intensity;
}

vec3 ComputePoint(Light light, vec3 worldPos, vec3 worldNormal, vec4 materialColor)
{
    vec3  lightDir = normalize(worldPos - light.position);
    float d = clamp(-dot(worldNormal, lightDir), 0, 1);
    vec3  v = normalize(ViewPos - worldPos);
    // Light dir is from light to point, but we want the other way around, hence the V - L
    vec3  h =  normalize(v - lightDir);
    float s = MaterialSpecular.x * pow(max(dot(h, worldNormal), 0), MaterialSpecular.y);

    return clamp(d * materialColor.xyz + s, 0, 1) * light.color.rgb * light.intensity * ComputeAttenuation(light, worldPos);
}

vec3 ComputeSpot(Light light, vec3 worldPos, vec3 worldNormal, vec4 materialColor)
{
    vec3  lightDir = normalize(worldPos - light.position);
    float cosAngle = dot(lightDir, light.direction);
    float spot = clamp((cosAngle - light.spot.w) / (light.spot.z - light.spot.w), 0, 1);

    float d = spot * clamp(-dot(worldNormal, lightDir), 0, 1);

    vec3  v = normalize(ViewPos - worldPos);
    // Light dir is from light to point, but we want the other way around, hence the V - L
    vec3  h =  normalize(v - lightDir);
    float s = spot * max(0, spot * MaterialSpecular.x * pow(max(dot(h, worldNormal), 0), MaterialSpecular.y));

    // Compute shadow
    vec4 shadowProj = light.shadowMatrix * vec4(worldPos, 1);
    // Perpective divide
    shadowProj = shadowProj / shadowProj.w;
    // Convert to UV (NDC range [-1,1] to tex coord range [0, 1])
    shadowProj = shadowProj * 0.5 + 0.5;
    // Fetch depth while comparing
    float bias = 0.0;
    float sampleDepth = texture(light.shadowmap, shadowProj.xy).x;
    float currentDepth = shadowProj.z + bias;
    float shadowFactor = (currentDepth > sampleDepth) ? (0.0) : (1.0);
   
    return shadowFactor * clamp(d * materialColor.xyz + s, 0, 1) * light.color.rgb * light.intensity * ComputeAttenuation(light, worldPos);
}

vec3 ComputeLight(Light light, vec3 worldPos, vec3 worldNormal, vec4 materialColor)
{
    if (light.type == 0)
    {
        return ComputeDirectional(light, worldPos, worldNormal, materialColor);
    }
    else if (light.type == 1)
    {
        return ComputePoint(light, worldPos, worldNormal, materialColor);
    }
    else if (light.type == 2)
    {
        return ComputeSpot(light, worldPos, worldNormal, materialColor);
    }
}

in vec3 fragPos;
in vec3 fragNormal;
in vec4 fragTangent;
in vec2 fragUV;

out vec4 OutputColor;

void main()
{
    vec3 worldPos = fragPos.xyz;
    vec3 worldNormal;
    if (HasTextureNormalMap)
    {
        vec3 n = normalize(fragNormal);
        vec3 t = normalize(fragTangent.xyz);

        // Create tangent space
        vec3 binormal = cross(n, t) * fragTangent.w;
        mat3 TBN = mat3(t, binormal, n);
        vec3 normalMap = texture(TextureNormalMap, fragUV).xyz * 2 - 1;

        worldNormal = TBN * normalMap;
    }
    else
    {
        worldNormal = normalize(fragNormal.xyz);
    }

    // Compute material color
    vec4 matColor = MaterialColor;
    if (HasTextureBaseColor) matColor *= texture(TextureBaseColor, fragUV);

    // Ambient component - get data from 4th mipmap of the cubemap (effective hardware blur)
    vec3 envLighting = EnvColor.xyz * MaterialColor.xyz * textureLod(EnvTextureCubeMap, worldNormal, 8).xyz;

    // Emissive component
    vec3 emissiveLighting = MaterialColorEmissive.rgb;

    // Direct light
    vec3 directLight = vec3(0,0,0);
    for (int i = 0; i < LightCount; i++)
    {
        directLight += ComputeLight(Lights[i], worldPos, worldNormal, matColor);
    }    

    // Add all lighting components
    OutputColor = vec4(envLighting + emissiveLighting + directLight, 1);

    // Fog
    float distToCamera = length(worldPos - ViewPos);
    float fogFactor = 1 / pow(2, EnvFogDensity * distToCamera * distToCamera);
    OutputColor = mix(EnvFogColor, OutputColor, fogFactor);
}
