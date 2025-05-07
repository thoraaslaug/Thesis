Shader "Unlit/IrisCutoutShader"
{
    Properties
    {
        _IrisSize ("Iris Size", Range(0, 2)) = 0.0001
        _IrisCenter ("Iris Center", Vector) = (0.5, 0.5, 0, 0)
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Lighting Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _IrisSize;
            float4 _IrisCenter;
            //float4 _ScreenParams;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.texcoord;

                // Correct for screen aspect ratio
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 center = _IrisCenter.xy;
                float2 diff = uv - center;
                diff.x *= aspect;

                float dist = length(diff);

                // Hard cutoff (no black fringe)
                float alpha = dist > _IrisSize ? 1.0 : 0.0;

                return fixed4(0, 0, 0, alpha);
            }
            ENDCG
        }
    }
}
