Shader "Custom/Flashing"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _Color ("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
        _Speed ("Speed", Float) = 10
        _Stencil ("Stencil", Float) = 0
        _StencilOp ("StencilOp", Float) = 0
        _StencilComp ("StencilComp", Float) = 0
        _StencilReadMask ("StencilReadMask", Float) = 0
        _StencilWriteMask ("StencilWriteMask", Float) = 0
        _ColorMask ("ColorMask", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Opaque"
        }

        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Speed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color;
                col *= sin(_Time.y * _Speed) * 0.5 + 0.5;
                return col;
            }
            ENDCG
        }
    }
}