Shader "Custom/CicleShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Feather("Feather", Range(0,0.5)) = 0.05
        _MaskColor("Outside Color", Color) = (0,0,0,1)
        _AspectX("Horizontal Scale", Float) = 1.0
        _AspectY("Vertical Scale", Float) = 1.0
        _OffsetX("Image Offset X", Range(-1,1)) = 0
        _OffsetY("Image Offset Y", Range(-1,1)) = 0
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
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
                float _OffsetX;
                float _OffsetY;

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

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // 🔥 1) Circle Mask는 오리지널 i.uv 기준으로 고정
                    float2 uvMask = i.uv * 2 - 1;

                    uvMask.x *= _AspectX;
                    uvMask.y *= _AspectY;

                    float dist = length(uvMask);
                    float mask = smoothstep(1.0 - _Feather, 1.0, dist);

                    // 🔥 2) 영상만 움직이는 Offset UV
                    float2 uvImage = i.uv;
                    uvImage.x += _OffsetX;
                    uvImage.y += _OffsetY;

                    fixed4 col = tex2D(_MainTex, uvImage);

                    return lerp(col, _MaskColor, mask);
                }
                ENDCG
            }
        }
}
