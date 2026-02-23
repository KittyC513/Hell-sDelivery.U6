#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _SHADOWS_SOFT
#pragma multi_compile _ _ADDITIONAL_LIGHTS
#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

void FetchMainLightData_float(float3 position, out float3 direction, out float3 color, out float distanceAttenuation, out float shadowAttenuation)
{
     // In Shader Graph Preview we will assume a default light direction and white color
    direction = half3(-0.3, -0.8, 0.6);
    color = half3(1, 1, 1);
    distanceAttenuation = 0.0;
    shadowAttenuation = 0.0;

    #ifndef SHADERGRAPH_PREVIEW
        float4 shadowCoord = float4(0, 0, 0, 0);

        float4 positionCS = TransformWorldToHClip(position);

        #if SHADOWS_SCREEN
            shadowCoord = ComputeScreenPos(positionCS);
        #else
            shadowCoord = TransformWorldToShadowCoord(position);
        #endif

        
        
        // GetMainLight defined in Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl
        Light mainLight = GetMainLight(shadowCoord, position, 1);
        direction = mainLight.direction;
        color = mainLight.color;
        distanceAttenuation = mainLight.distanceAttenuation;
        
        
        shadowAttenuation = mainLight.shadowAttenuation;


    #endif
}



void GetAdditionalLightData_float(float3 world_Pos, float lightID, out half3 direction, out half3 color, out float distanceAttenuation)
{
    direction = half3(0, -0.8, 0.6);
    color = half3(1.0, 1.0, 1.0);
    distanceAttenuation = 1;


    #ifndef SHADERGRAPH_PREVIEW

        #ifdef _ADDITIONAL_LIGHTS
            float lightCount = GetAdditionalLightsCount();

            if (lightID < lightCount)
            {
                Light additional = GetAdditionalLight(lightID, world_Pos, 1);
                direction = additional.direction;
                color = additional.color;
                distanceAttenuation = additional.distanceAttenuation;
            }
        #endif
     
    #endif
}

void GetAllAdditionalLights_float(float3 worldPos, float3 worldNormal, float smoothing, out float3 color)
{

    color = 0.0;

    #ifndef SHADERGRAPH_PREVIEW
    
        int lightCount = GetAdditionalLightsCount();

        for (int i = 0; i < lightCount; ++i)
        {
            Light light = GetAdditionalLight(i, worldPos);

            float3 _col = dot(light.direction, worldNormal);
            _col = smoothstep(0, smoothing, _col);
            _col *= light.color;
            _col *= light.distanceAttenuation;

            color += _col;
        }

    #endif
}




#endif