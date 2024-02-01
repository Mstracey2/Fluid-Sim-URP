Shader "Universal Render Pipeline/Custom/UnlitWithDotsInstancing"
{
    Properties
    {
        _BaseMap("Base Texture", 2D) = "white" {}
        _BaseColor("Base Colour", Color) = (1, 1, 1, 1)
    }

        SubShader
        {
            Tags
            {
                "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry"
            }
                //float _size;

              /*  struct Particle
                {
                    float pressure;
                    float density;
                    float3 currentForce;
                    float3 velocity;
                    float3 position;
                };*/
            Pass
            {
                Name "Forward"
                Tags
                {
                    "LightMode" = "UniversalForward"
                }

                Cull Back

                HLSLPROGRAM
                #pragma exclude_renderers gles gles3 glcore
                #pragma target 4.5
                #pragma vertex UnlitPassVertex
                #pragma fragment UnlitPassFragment
                #pragma multi_compile_instancing
                #pragma instancing_options renderinglayer
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                 #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    StructuredBuffer<Particle> _particlesBuffer;
                #endif

                    void setup()
                    {
                    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                        float3 pos = _particlesBuffer[unity_InstanceID].position;
                        float size = _size;

                        unity_ObjectToWorld._11_21_31_41 = float4(size, 0, 0, 0);
                        unity_ObjectToWorld._12_22_32_42 = float4(0, size, 0, 0);
                        unity_ObjectToWorld._13_23_33_43 = float4(0, 0, size, 0);
                        unity_ObjectToWorld._14_24_34_44 = float4(pos.xyz, 1);
                        unity_WorldToObject = unity_ObjectToWorld;
                        unity_WorldToObject._14_24_34 *= -1;
                        unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
                    #endif
                    }

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                CBUFFER_END

                #ifdef UNITY_DOTS_INSTANCING_ENABLED
                    UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
                        UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)
                    UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
                    #define _BaseColor UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _BaseColor)
                #endif

                TEXTURE2D(_BaseMap);
                SAMPLER(sampler_BaseMap);

                Varyings UnlitPassVertex(Attributes input)
                {
                    Varyings output;

                    UNITY_SETUP_INSTANCE_ID(input);
                    UNITY_TRANSFER_INSTANCE_ID(input, output);

                    const VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                    output.positionCS = positionInputs.positionCS;
                    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                    output.color = input.color;
                    return output;
                }

                half4 UnlitPassFragment(Varyings input) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(input);
                    half4 baseMap = half4(SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv));
                    return baseMap * _BaseColor * input.color;
                }
                ENDHLSL
            }
        }
}