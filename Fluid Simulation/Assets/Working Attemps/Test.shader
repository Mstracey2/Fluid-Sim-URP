Shader "Unlit/Test"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
            LOD 100


                HLSLINCLUDE
                
                // Shader Stages
                #pragma vertex vert
                #pragma fragment frag
                // -------------------------------------
                //--------------------------------------
                // GPU Instancing
                #pragma target 4.5
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON
                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
                //#pragma instancing_options procedural:setup
                // -------------------------------------
                // Includes
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitForwardPass.hlsl"


            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _size;
            float maxVel;
            struct Particle
            {
                float3 pressure;
                float2 density;
                float3 external;
                float3 velocity;
                float3 position;
                float3 positionPrediction;
                uint3 hashData;
            };

            StructuredBuffer<Particle> _particlesBuffer;

            Texture2D<float4> ColourMap;
            SamplerState linear_clamp_sampler;
            float gradientChoice;
            struct VertexInput
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 colour : TEXCOORD1;
            };

            ENDHLSL

            Pass
            {
            
                HLSLPROGRAM

                VertexOutput vert(VertexInput v, uint instanceID : SV_InstanceID)
                {
                    float3 centreWorld = _particlesBuffer[instanceID].position;
                    float3 worldVertPos = centreWorld + mul(unity_ObjectToWorld, v.position * _size);
                    float3 objectVertPos = mul(unity_WorldToObject, float4(worldVertPos.xyz, 1));
                    VertexOutput o;
                    o.uv = v.uv;
                    o.position = TransformObjectToHClip(objectVertPos);

                    float3 gradientValue;

                    //Changing colour depending on the speed
                    if (gradientChoice == 1) 
                    {
                        gradientValue = _particlesBuffer[instanceID].velocity;
                    }
                    else if (gradientChoice == 2)
                    {
                        gradientValue = _particlesBuffer[instanceID].pressure;
                    }
                    else if (gradientChoice == 3)
                    {
                        gradientValue = _particlesBuffer[instanceID].position;
                    }

                    float speed = saturate(length(gradientValue) / maxVel);
                    o.colour = ColourMap.SampleLevel(linear_clamp_sampler, float2(speed, 0.5), 0); //float3(0,0, length(_particlesBuffer[instanceID].velocity));


                    return o;
                }

                float4 frag(VertexOutput i) : SV_Target
                {
                    //float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                     return float4(i.colour,1);
                }

                    ENDHLSL

            }
    }
}
