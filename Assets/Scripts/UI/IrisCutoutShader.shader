Shader "Unlit/IrisCutoutShader"
{  
  Properties
    {
        _IrisSize ("Iris Size", Range(0, 1)) = 0.0000001
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
                float2 center = float2(0.5, 0.5); // Center of screen
                float dist = distance(i.texcoord, center);

                // 🔥 Create a transparent hole in a black screen
                float mask = smoothstep(_IrisSize, _IrisSize + 0.02, dist);
                return fixed4(0, 0, 0, mask); // Transparent inside, black outside
            }
            ENDCG
        }
    }
}