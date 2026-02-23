void CalculateLightData_float(float4 colour, float4 ambientColour, float4 light, float4 additionalLights, float4 specular, float4 tex,  float4 rimLight, out float4 colourOutput)
{
    colourOutput = float4(1.0, 1.0, 1.0, 1.0);
    
    #ifndef SHADERGRAPH_PREVIEW
        
        colourOutput = colour * tex * (ambientColour + light + additionalLights + specular + rimLight);
        
    #endif
    
}
