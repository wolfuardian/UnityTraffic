Shader "Custom/HeightBasedMaskWorldSpaceBlurWithTwoTexAndColor"
{
    Properties
    {
        _CutoffHeight("Cutoff Height", Float) = 0.5
        _BlurRange("Blur Range", Float) = 0.1
        _MainTex("Base (RGB)", 2D) = "white" {}
        _MaskTex("Mask (RGB)", 2D) = "white" {} // 用於第二個材質球的紋理
        _MaskColor("Mask Color", Color) = (1,1,1,1) // 新增：遮罩材質顏色
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        AlphaTest Greater 0.01

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

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
                float dist : TEXCOORD2;
                float alpha : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex; // 遮罩紋理
            float4 _MaskTex_ST;
            float _CutoffHeight;
            float _BlurRange;
            fixed4 _MaskColor; // 遮罩材質顏色

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.height = worldPos.y;

                // 計算高度與閾值之間的距離
                o.dist = o.height - _CutoffHeight;
                o.alpha = smoothstep(0, _BlurRange, o.dist); // 計算模糊遮罩
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 檢查高度是否低於閾值
                if (i.height < _CutoffHeight)
                {
                    discard; // 不渲染低於閾值的像素
                }

                fixed4 mainCol = tex2D(_MainTex, i.uv); // 主材質顏色
                fixed4 maskCol = tex2D(_MaskTex, i.uv) * _MaskColor; // 遮罩材質顏色

                // 混合主材質球和遮罩材質球的顏色
                fixed4 finalCol = lerp(maskCol, mainCol, i.alpha);

                return finalCol;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}