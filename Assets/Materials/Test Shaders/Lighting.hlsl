void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, 
	out float DistanceAtten, out float ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = normalize(float3(0.5f, 0.5f, 0.25f));
    Color = float3(1.0f, 1.0f, 1.0f);
    DistanceAtten = 1.0f;
    ShadowAtten = 1.0f;
#else
    //functions that access directional lights and output the direction, colour and attenuation of the light
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    Light mainLight = GetMainLight(shadowCoord);
 
    //these are for use in the shader graph to manipulate the light from the graph
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
#endif
}

void MainLight_half(half3 WorldPos, out half3 Direction, out half3 Color, 
	out half DistanceAtten, out half ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = normalize(half3(0.5f, 0.5f, 0.25f));
    Color = half3(1.0f, 1.0f, 1.0f);
    DistanceAtten = 1.0f;
    ShadowAtten = 1.0f;
#else
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    Light mainLight = GetMainLight(shadowCoord);
 
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
#endif
}

void AdditionalLight_float(float3 WorldPos, int Index, out float3 Direction, 
	out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
    //set default values
    Direction = normalize(float3(0.5f, 0.5f, 0.25f));
    Color = float3(0.0f, 0.0f, 0.0f);
    DistanceAtten = 0.0f;
    ShadowAtten = 0.0f;

#ifndef SHADERGRAPH_PREVIEW
    //this function returns the amount of additional lights
    int pixelLightCount = GetAdditionalLightsCount();
    //check if the index is < the amount of lights acting on the object
    //the index is the number of the additional light, we are checking if that additional light is valid/within our index value
    if(Index < pixelLightCount)
    {
        //get the light acting on our object 
        Light light = GetAdditionalLight(Index, WorldPos);
        
        //set our direction, color etc. based on that light
        Direction = light.direction;
        Color = light.color;
        DistanceAtten = light.distanceAttenuation;
        ShadowAtten = light.shadowAttenuation;
    }
#endif
}

void AdditionalLight_half(half3 WorldPos, int Index, out half3 Direction,
	out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
	Direction = normalize(half3(0.5f, 0.5f, 0.25f));
	Color = half3(0.0f, 0.0f, 0.0f);
	DistanceAtten = 0.0f;
	ShadowAtten = 0.0f;

#ifndef SHADERGRAPH_PREVIEW
	int pixelLightCount = GetAdditionalLightsCount();
	if (Index < pixelLightCount)
	{
		Light light = GetAdditionalLight(Index, WorldPos);

		Direction = light.direction;
		Color = light.color;
		DistanceAtten = light.distanceAttenuation;
		ShadowAtten = light.shadowAttenuation;
	}
#endif
}
