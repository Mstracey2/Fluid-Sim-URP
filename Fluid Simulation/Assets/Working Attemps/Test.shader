Shader "Unlit/Test"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100


                HLSLINCLUDE
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                // -------------------------------------
                #pragma target 2.0
                // -------------------------------------
                // Shader Stages
                #pragma vertex UnlitPassVertex
                #pragma fragment UnlitPassFragment
                // -------------------------------------
                // Material Keywords
                #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
                #pragma shader_feature_local_fragment _ALPHATEST_ON
                #pragma shader_feature_local_fragment _ALPHAMODULATE_ON
                // -------------------------------------
                // Unity defined keywords
                #pragma multi_compile_fog
                #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
                #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
                #pragma multi_compile _ DEBUG_DISPLAY
                #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
                //--------------------------------------
                // GPU Instancing
                #pragma multi_compile_instancing
                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
                #pragma instancing_options procedural:setup
                // -------------------------------------
                // Includes
                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitForwardPass.hlsl"


            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _size;

            struct Particle
            {
                float pressure;
                float density;
                float3 currentForce;
                float3 velocity;
                float3 position;
            };

            struct VertexInput
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            ENDHLSL

            Pass
            {
            
                HLSLPROGRAM

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

                float4 frag(VertexOutput i) : SV_Target
                {
                    float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                    return baseTex * _BaseColor;
                }

                    ENDHLSL

            }
    }
}
