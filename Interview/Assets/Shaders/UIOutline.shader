Shader "Custom/UIOutline"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _OutlineWidth("Outline Width", float) = 1
        _OutlineColor("Outline Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _AlphaValue("Alpha Value", Range(0, 1)) = 0.1
    }
        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off Lighting Off ZWrite Off ZTest Always

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                #include "UnityUI.cginc"
                sampler2D _MainTex;
                float4 _MainTex_ST;
                half4 _MainTex_TexelSize;
                float _OutlineWidth;
                float4 _OutlineColor;
                float _AlphaValue;
                float4 _ClipRect;
                struct appdata
                {
                    float4 vertex   : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 normal : NORMAL;
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    half2 uv  : TEXCOORD0;
                    half2 left : TEXCOORD1;
                    half2 right : TEXCOORD2;
                    half2 up : TEXCOORD3;
                    half2 down : TEXCOORD5;
                    float4 worldPosition : TEXCOORD6;
                };

            v2f vert(appdata i)
            {
                v2f o;
                o.worldPosition = i.vertex;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = TRANSFORM_TEX(i.uv, _MainTex);
                o.left = o.uv + half2(-1, 0) * _MainTex_TexelSize.xy * _OutlineWidth;
                o.right = o.uv + half2(1, 0) * _MainTex_TexelSize.xy * _OutlineWidth;
                o.up = o.uv + half2(0, 1) * _MainTex_TexelSize.xy * _OutlineWidth;
                o.down = o.uv + half2(0, -1) * _MainTex_TexelSize.xy * _OutlineWidth;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float transparent = tex2D(_MainTex, i.left).a + tex2D(_MainTex, i.right).a + tex2D(_MainTex, i.up).a + tex2D(_MainTex, i.down).a;
                fixed4 col = tex2D(_MainTex, i.uv);
                float outlineFactor = step(0.8, col.a);
                fixed4 outlineColor = smoothstep(_AlphaValue, 1, transparent) * _OutlineColor;
                fixed4 finalColor = lerp(outlineColor, col, outlineFactor);
                finalColor.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                clip(finalColor.a - 0.001);
#ifdef UNITY_UI_ALPHACLIP

#endif
                return finalColor;
            }
            ENDCG
        }
        }
}