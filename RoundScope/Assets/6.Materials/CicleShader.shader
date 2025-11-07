Shader "Custom/CicleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Feather ("Feather", Range(0,0.5)) = 0.05
        _MaskColor ("Outside Color", Color) = (0,0,0,1)
        _AspectX ("Horizontal Scale", Float) = 1.0
        _AspectY ("Vertical Scale", Float) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Feather;
            fixed4 _MaskColor;
            float _AspectX;
            float _AspectY;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2 - 1; // 중앙 기준 -1~1

                // 비율 보정 (가로, 세로 각각)
                uv.x *= _AspectX;
                uv.y *= _AspectY;

                float dist = length(uv);

                // 부드러운 전이 (Feather)
                float mask = smoothstep(1.0 - _Feather, 1.0, dist);

                fixed4 col = tex2D(_MainTex, i.uv);

                // 원 안(col), 원 밖(검정)
                return lerp(col, _MaskColor, mask);
            }
            ENDCG
        }
    }
}
