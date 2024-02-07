Shader "Custom/HeightBasedMaskWorldSpace"
{
    Properties
    {
        _CutoffHeight("Cutoff Height", Float) = 0.5
        _MainTex("Base (RGB)", 2D) = "white" {}
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
                float height : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CutoffHeight;

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

                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
