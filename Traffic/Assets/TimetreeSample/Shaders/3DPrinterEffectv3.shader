Shader "Custom/3DPrinterEffectv3"
{
    Properties
    {
        _Color ("Tint", Color) = (0, 0, 0, 1)
        _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector]_Metallic ("Metalness", Range(0, 1)) = 0
        [HideInInspector]_Smoothness ("Smoothness", Range(0, 1)) = 0.5
        [HDR] _Emission ("Emission", color) = (0,0,0)
        _WireframeColor("Wireframe Color", Color) = (0,0,0,1)
        _WireframeThickness("Wireframe Thickness", Range(0, 10)) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"="Geometry"
        }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;

        half _Smoothness;
        half _Metallic;
        half3 _Emission;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };

        half _WireframeThickness;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // 宣告原始的uv座標
            const float2 uv = IN.uv_MainTex;

            // 宣告螢幕空間uv座標
            float2 screen_uv = IN.screenPos.xy / IN.screenPos.w;

            // 調整紋理座標的X軸以符合當前屏幕的寬高比，防止圖像扭曲
            const float aspect = _ScreenParams.x / _ScreenParams.y;
            screen_uv.x = screen_uv.x * aspect;

            // 根據時間變化計算垂直方向的偏移量，實現動態滾動效果
            const float offset = frac(_Time.y * -1);
            screen_uv.y = frac(screen_uv.y + offset);

            // 將uv.y坐標轉換為對稱的鏡像漸變效果，使得紋理在垂直方向上呈現從中心向兩側的對稱漸變
            screen_uv.y = 1 - abs(screen_uv.y - 0.5) * 2;

            o.Albedo = tex2D(_MainTex, uv).rgb * _Color.rgb * screen_uv.y;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        ENDCG
        // 特定效果應用Pass
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha // 啟用基於Alpha的混合
            ZWrite Off // 通常在使用Alpha混合時關閉深度寫入
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom

            // make fog work
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            // We add our barycentric variables to the geometry struct.
            struct g2f
            {
                float4 pos : SV_POSITION;
                float3 barycentric : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                // We push the conversion to ClipPos into the geom function as we need 
                // the mesh vertex values for the edge culling.
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2f IN[3], inout TriangleStream<g2f> triStream)
            {
                const float edgeLengthX = length(IN[1].vertex - IN[2].vertex);
                const float edgeLengthY = length(IN[0].vertex - IN[2].vertex);
                const float edgeLengthZ = length(IN[0].vertex - IN[1].vertex);
                float3 modifier = float3(0.0, 0.0, 0.0);
                // We're fine using if statments it's a trivial function.
                if ((edgeLengthX > edgeLengthY) && (edgeLengthX > edgeLengthZ))
                {
                    modifier = float3(1.0, 0.0, 0.0);
                }
                else if ((edgeLengthY > edgeLengthX) && (edgeLengthY > edgeLengthZ))
                {
                    modifier = float3(0.0, 1.0, 0.0);
                }
                else if ((edgeLengthZ > edgeLengthX) && (edgeLengthZ > edgeLengthY))
                {
                    modifier = float3(0.0, 0.0, 1.0);
                }

                g2f o;
                o.pos = UnityObjectToClipPos(IN[0].vertex);
                o.barycentric = float3(1.0, 0.0, 0.0) + modifier;
                triStream.Append(o);
                o.pos = UnityObjectToClipPos(IN[1].vertex);
                o.barycentric = float3(0.0, 1.0, 0.0) + modifier;
                triStream.Append(o);
                o.pos = UnityObjectToClipPos(IN[2].vertex);
                o.barycentric = float3(0.0, 0.0, 1.0) + modifier;
                triStream.Append(o);
            }

            fixed4 _WireframeColor;
            float _WireframeThickness;

            fixed4 frag(const g2f i) : SV_Target
            {
                // Calculate the unit width based on triangle size.
                const float3 unitWidth = fwidth(i.barycentric);
                // Alias the line a bit.
                float3 aliased = smoothstep(float3(0.0, 0.0, 0.0), unitWidth * _WireframeThickness, i.barycentric);
                // Use the coordinate closest to the edge.
                float alpha = 1 - min(aliased.x, min(aliased.y, aliased.z));
                // Set to our forwards facing wireframe colour.
                return fixed4(_WireframeColor.rgb, alpha);
            }
            ENDCG
        }
    }
    FallBack "Standard"
}