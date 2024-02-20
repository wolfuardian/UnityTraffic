Shader "Custom/3DPrinterEffect"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _ConstructColor("Construct Color", Color) = (1,1,1,1)
        _ConstructY ("Construct Y", Float) = 0
        _ConstructGap("Construct Gap", Float) = 0.25

        _UseScanline ("Use Scanline", Range(0,1)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200
        Cull Off
        CGPROGRAM
        #pragma surface surf Custom fullforwardshadows
        #include "UnityPBSLighting.cginc"

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float vface : VFACE;
            // A negative value faces backwards (-1), while a positive value (+1) faces the camera (requires ps_3_0)
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float _ConstructY;
        float _ConstructGap;
        fixed4 _ConstructColor;

        float _UseScanline;

        int building;
        fixed3 viewDir;

        inline half4 LightingCustom(SurfaceOutputStandard s, half3 lightDir, UnityGI gi)
        {
            if (building == 2)
                return _ConstructColor;

            // ScanlineMaterial
            if (building == 1)
                return _ConstructColor;

            if (building == 0)
                return LightingStandard(s, lightDir, gi);

            return LightingStandard(s, lightDir, gi);
        }


        void surf(Input i, inout SurfaceOutputStandard o)
        {
            const float noise = sin((i.worldPos.x * i.worldPos.z) * 60 + _Time[3] + o.Normal) / 120;

            fixed4 c;


            if (i.worldPos.y > _ConstructY + noise || i.vface <= 0)
            {
                c = _ConstructColor;
                building = 1;
            }
            else
            {
                c = tex2D(_MainTex, i.uv_MainTex);
                building = 0;
            }

            if (i.worldPos.y > _ConstructY + noise + _ConstructGap && _UseScanline == 0)
                discard;


            o.Albedo = c.rgb * _Color;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }

        inline void LightingCustom_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
        {
            LightingStandard_GI(s, data, gi);
        }
        ENDCG

        // 添加一個額外的Pass用於螢幕空間效果
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _RampTex; // Ramp紋理

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy / _ScreenParams.xy;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 根據螢幕空間坐標讀取Ramp紋理
                fixed4 rampColor = tex2D(_RampTex, i.uv);
                // 將UV坐標直接用作顏色值
                fixed4 color;
                color.r = i.uv.x; // X坐標映射到紅色通道
                color.g = i.uv.y; // Y坐標映射到綠色通道
                color.b = 0; // 藍色通道設定為0
                color.a = 1; // Alpha通道設定為完全不透明
                
                return color;
            }
            ENDCG
        }






    }
    FallBack "Diffuse"
}