Shader "Custom/UIDissolveShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NoiseTex("Texture", 2D) = "white" {}
        _ThresholdAlpha("ThresholdAlpha",Range(0,1)) = 0

        _DissolveColor("DissolveColor",Color) = (1,1,1,1)
        _DissolveAlpha("DissolveAlpha",Range(0,0.3)) = 0
        _DissolveLight("DissolveLight",Range(1,5)) = 1

    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                Cull Off Lighting Off ZWrite Off ZTest Always
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 noiseUV : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float _ThresholdAlpha;

            float4 _DissolveColor;
            float _DissolveAlpha;
            float _DissolveLight;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.noiseUV = TRANSFORM_TEX(v.uv,_NoiseTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
            fixed4 col = tex2D(_MainTex, i.uv);
            fixed4 noiseCol = tex2D(_NoiseTex,i.noiseUV);
            clip(noiseCol.a - _ThresholdAlpha);
            float progress = (noiseCol.a - _ThresholdAlpha) / _DissolveAlpha;
            progress = saturate(progress);  
            float4 result = lerp(_DissolveColor * _DissolveLight,col,progress);
            result.a = col.a;
            UNITY_APPLY_FOG(i.fogCoord, col);
            return result;
        }
        ENDCG
    }
        }
}