// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Winterland/Snow Gradient"
{
    Properties
    {
        _MainTex ("Gradient Texture", 2D) = "white" {}
        _DetailTex ("Detail Texture", 2D) = "black" {}
        _DetailStrength ("Detail Strength", Range(0,1)) = 1
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0
        _AlphaSharpness ("Alpha Sharpness", Range(0,0.999)) = 0
        _AlphaMultiplier ("Alpha Multiplier", Range(0,1)) = 1
        _NormalOffset("Normal Offset", float) = 0
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest+50" }
        Offset -1, -1
        LOD 100

        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "BRCCommon.cginc"
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD1;
                SHADOW_COORDS(2) // put shadows data into TEXCOORD1
                float2 detailuv : TEXCOORD3;
            };

            BRC_LIGHTING_PROPERTIES;
            sampler2D _MainTex;
            sampler2D _DetailTex;
            float4 _MainTex_ST;
            float4 _DetailTex_ST;
            float _NormalOffset;
            

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.xyz += v.normal * _NormalOffset;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.detailuv = TRANSFORM_TEX(mul(unity_ObjectToWorld, v.vertex).xz, _DetailTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                TRANSFER_SHADOW(o)
                return o;
            }

            float _DetailStrength;
            float _AlphaCutoff;
            float _AlphaSharpness;
            float _AlphaMultiplier;

            fixed4 frag(v2f i) : SV_Target
            {
                BRC_LIGHTING_FRAGMENT;
                fixed4 col = tex2D(_MainTex, i.uv) * BRCLighting;
                float detail = tex2D(_DetailTex, i.detailuv).r * _DetailStrength;
                col.a = pow(col.a, (-detail)+1);
                if (col.a <= _AlphaCutoff)
                    col.a = 0;
                col.a = min(1, pow(max(0, col.a), (-_AlphaSharpness) + 1));
                col.a *= _AlphaMultiplier;
                return col;
            }
            ENDCG
        }
    }
}
