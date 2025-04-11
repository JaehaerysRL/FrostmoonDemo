Shader "Custom/Terrain"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _SnowBlend ("Snow Coverage", Range(0,1)) = 0
        _SnowColor ("Snow Color", Color) = (0.92,0.96,1,1)    // 冷蓝色调雪
        _HeightOffset ("Height Brightness", Range(0,5)) = 0
        _HeightColor ("Height Color", Color) = (1,0.8,0.6,1) // 暖色调
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _SnowBlend;
            fixed4 _SnowColor;
            float _HeightOffset;
            fixed4 _HeightColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv) * i.color;
                // 高度效果 - 暖色叠加
                fixed3 heightEffect = lerp(
                    baseColor.rgb, 
                    _HeightColor.rgb * 2, // 暖色表现
                    saturate(_HeightOffset / 5.0) // 高度影响范围
                );
                // 雪覆盖 - 冷色混合
                fixed3 snowEffect = lerp(
                    heightEffect,
                    _SnowColor.rgb,
                    _SnowBlend * smoothstep(0.3, 0.7, _SnowBlend) // 添加过渡曲线
                );

                // 最终颜色合成
                return fixed4(snowEffect, baseColor.a);
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}
