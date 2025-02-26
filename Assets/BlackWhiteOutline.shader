Shader "Custom/BlackWhiteOutline"
{
    Properties
    {
        _OutlineWidth ("Outline Width", Range(0.001, 0.1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            Name "BlackWhiteOutline"
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

            // Depth texture declaration (for URP)
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

                // Sample depth from camera texture
                float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;

                // Edge detection using depth difference
                float depthDiff = abs(depth - SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + float2(_OutlineWidth, 0)).r);
                float edge = smoothstep(0.0, _OutlineWidth, depthDiff);

                // Edge detection using normal variation
                float3 normal = normalize(IN.normalWS);
                float normalEdge = smoothstep(0.0, _OutlineWidth, abs(dot(normal, float3(0, 0, 1))));

                // Combine edges and create white outlines on black objects
                float outline = max(edge, normalEdge);
                return half4(outline, outline, outline, 1.0); // White outline, black object fill
            }
            ENDHLSL
        }
    }
}
