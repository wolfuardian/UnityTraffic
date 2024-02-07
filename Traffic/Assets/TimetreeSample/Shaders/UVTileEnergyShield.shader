Shader "Custom/UVTileEnergyShield"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Color ("Shield Color", Color) = (1,1,1,1)
        _Strength ("Strength", Float) = 1
        _SpeedX ("Noise SpeedX", Float) = 1
        _SpeedY ("Noise SpeedY", Float) = 1
        _TileX ("TileX", Float) = 1
        _TileY ("TileY", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Strength;
            float _SpeedX;
            float _SpeedY;
            float _TileX;
            float _TileY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _MainTex_ST.xy;
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                // Apply UV tiling
                uv.x *= _TileX;
                uv.y *= _TileY;
                
                // Apply noise based on time
                float2 noiseUV = uv + float2(_SpeedX * _Time.y, _SpeedY * _Time.y);
                float noise = tex2D(_NoiseTex, noiseUV).r;

                // Combine texture with noise and color
                float4 col = tex2D(_MainTex, uv) * _Color * noise * _Strength;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
