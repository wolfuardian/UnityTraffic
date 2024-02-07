Shader "Custom/HeightBasedMaskWorldSpaceBlur"
{
    Properties
    {
        _CutoffHeight("Cutoff Height", Float) = 0.5
        _BlurRange("Blur Range", Float) = 0.1
        _MainTex("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        AlphaTest Greater 0.01
        
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
                float height : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CutoffHeight;
            float _BlurRange;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                // 將頂點位置轉換到世界空間
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.height = worldPos.y; // 使用 Y 軸作為高度
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 檢查高度是否低於閾值
                if (i.height < _CutoffHeight)
                {
                    discard; // 不渲染低於閾值的像素
                }
                
                // 計算高度與閾值之間的距離
                float dist = i.height - _CutoffHeight;
                
                // 使用 smoothstep 函數來計算混合因子
                float alpha = smoothstep(0, _BlurRange, dist);

                fixed4 col = tex2D(_MainTex, i.uv);
                col.a *= alpha; // 應用透明度

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
