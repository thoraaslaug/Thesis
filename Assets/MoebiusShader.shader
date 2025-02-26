Shader "Custom/MoebiusShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,0,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _FresnelPower ("Fresnel Power", Range(1, 15)) = 10
        _StepThreshold ("Step Threshold", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            fixed4 _BaseColor;
            fixed4 _OutlineColor;
            float _FresnelPower;
            float _StepThreshold;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Cel shading
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float diff = dot(i.worldNormal, lightDir);
                diff = step(_StepThreshold, diff);

                // Fresnel effect for outlines
                float fresnel = pow(1.0 - dot(i.viewDir, i.worldNormal), _FresnelPower);

                // Combine base color and outline
                fixed4 col = _BaseColor * diff;
                col = lerp(col, _OutlineColor, fresnel);

                return col;
            }
            ENDCG
        }
    }
}
