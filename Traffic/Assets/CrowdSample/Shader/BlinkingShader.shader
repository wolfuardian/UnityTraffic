Shader "Custom/BlinkingShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _BlinkSpeed("Blink Speed", Range(0.1,10)) = 10
        _Stencil("Stencil", Int) = 0
        
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            fixed4 color;
            float blink_speed;

            v2f vert(const appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                const float blink = sin(_Time.y * blink_speed) * 0.5 + 0.5;
                return color * blink;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}