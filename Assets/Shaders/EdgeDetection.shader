Shader "Custom/EdgeDetection"
{ Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _ColorThreshold("Color Threshold", Range(0.01, 10)) = 0.1
        _DepthThreshold("Depth Threshold", Range(0.001, 10)) = 1.0
        _LineThickness("Line Thickness", Range(0.1, 10)) = 1.0
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }
        LOD 100

        Pass
        {
            Name "EdgeDetection"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            float4 _MainTex_ST;
            float _ColorThreshold;
            float _DepthThreshold;
            float _LineThickness;
            float4 _OutlineColor;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y) * _LineThickness;

                float3 centerColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rgb;
                float centerDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv).r;

                float colorDiff = 0;
                float depthDiff = 0;

                for (int y = -1; y <= 1; y++) {
                    for (int x = -1; x <= 1; x++) {
                        float2 offset = float2(x, y) * texelSize;
                        float3 neighborColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + offset).rgb;
                        float neighborDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv + offset).r;

                        colorDiff += distance(centerColor, neighborColor);
                        depthDiff += abs(centerDepth - neighborDepth);
                    }
                }

                float edge = step(_ColorThreshold, colorDiff) + step(_DepthThreshold, depthDiff);
                return edge > 0 ? _OutlineColor : float4(centerColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack Off
}

