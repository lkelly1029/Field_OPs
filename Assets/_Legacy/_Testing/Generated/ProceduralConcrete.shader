Shader "Custom/ProceduralConcrete"
{
    Properties
    {
        _BaseColor("Concrete Color", Color) = (0.5, 0.5, 0.5, 1)
        _GrainStrength("Grain Strength", Range(0.0, 0.5)) = 0.1
        _GrainSize("Grain Size", Float) = 50.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Minimal Includes for URP
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _GrainStrength;
                float _GrainSize;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = input.uv;
                return output;
            }

            // --- THE MATH MAGIC ---
            // A simple function to generate pseudo-random noise
            float random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            half4 frag(Varyings input) : SV_Target
            {
                // 1. Calculate basic lighting (Simulated simple light)
                Light mainLight = GetMainLight();
                float NdotL = saturate(dot(input.normalWS, mainLight.direction));
                
                // 2. Generate the "Grain"
                // We multiply UV by GrainSize to make the noise smaller/finer
                float noise = random(input.uv * _GrainSize);
                
                // 3. Mix the Color with the Noise
                // We darken the color slightly where the noise is present
                float4 finalColor = _BaseColor;
                finalColor.rgb -= noise * _GrainStrength;

                // 4. Apply Lighting
                finalColor.rgb *= (NdotL + 0.2); // +0.2 adds a little ambient light

                return finalColor;
            }
            ENDHLSL
        }
    }
}