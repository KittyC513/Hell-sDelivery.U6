Shader "Outlines/BackfaceOutlines"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Thickness("Thickness", Float) = 0
        _Color ("Colour", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        

        Pass
        {
            Name "Outlines"
            
            //cull front faces
            Cull Front

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            //#pragma prefer_hlslcc gles
            //#pragma exlude_renderers_d3d11_9x

            #pragma vertex Vertex
            #pragma fragment Fragment

            float _Thickness;
            float4 _Color;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 normalOS : NORMAL;
            };

            struct VertexOutput
            {
                float4 positionCS : SV_POSITION;
            };

            VertexOutput Vertex(Attributes input)
            {
                VertexOutput output;

                float3 normalOS = input.normalOS;

                //extrude the object space position along a normal vector (move the vertex position outwards)
                float3 posOS = input.positionOS.xyz + normalOS * _Thickness;
                
                //grab the position in clip space
                output.positionCS = GetVertexPositionInputs(posOS).positionCS;

                return output;
            }

            float4 Fragment(VertexOutput input) : SV_TARGET
            {
                return _Color;
            }


            ENDHLSL
            
        }
    }
}
