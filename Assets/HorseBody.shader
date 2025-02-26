Shader "Custom/HorseBody"
{   Properties
    {
        _OutlineWidth ("Outline Width", Range(0.001, 0.1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            Name "HorseBlackOutline"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : NORMAL;
                float4 screenPos : TEXCOORD1;
            };

            // Depth texture for outline detection
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            float _OutlineWidth;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.screenPos.xy / IN.screenPos.w;

                // Sample depth texture for edge detection
                float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
                float depthDiff = abs(depth - SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + float2(_OutlineWidth, 0)).r);

                // Detect edges only at large depth differences (avoiding internal lines)
                float edge = smoothstep(0.02, _OutlineWidth, depthDiff);

                // Normal-based outline detection (silhouettes only)
                float3 normal = normalize(IN.normalWS);
                float normalEdge = smoothstep(0.02, _OutlineWidth, abs(dot(normal, float3(0, 0, 1))));

                // Combine edge detection methods
                float outline = max(edge, normalEdge);

                // ✅ Ensure the body is **black**, and only the edges are white
                float3 bodyColor = float3(0.0, 0.0, 0.0); // Black base color
                float3 outlineColor = float3(1.0, 1.0, 1.0); // White outline color

                // Blend outline with body color
                float3 finalColor = lerp(bodyColor, outlineColor, outline);

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}
