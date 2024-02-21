Shader "Custom/3DPrinterEffectv8"
{
    Properties
    {
        _Color ("Tint", Color) = (0, 0, 0, 1)
        _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector]_Metallic ("Metalness", Range(0, 1)) = 0
        [HideInInspector]_Smoothness ("Smoothness", Range(0, 1)) = 0.5
        [HDR] _Emission ("Emission", color) = (0,0,0)

        _WireframeColorA("Wireframe Color A", Color) = (0,0,0,1)
        _WireframeColorB("Wireframe Color B", Color) = (0,0,0,1)
        _WireframeThickness("Wireframe Thickness", Range(0, 10)) = 0.1

        [HDR] _ConstructColor("Construct Color", Color) = (1,1,1,1)
        _ConstructY ("Construct Y", Float) = 0
        _ConstructGap("Construct Gap", Float) = 0.25
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"="Geometry"
        }
        LOD 200
        // NB: We add the blend mode in so that we an alias our wireframe.
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
            float3 worldPos;
            float vface : VFACE;
            // A negative value faces backwards (-1), while a positive value (+1) faces the camera (requires ps_3_0)
        };

        fixed4 _Color;

        half _Smoothness;
        half _Metallic;
        half3 _Emission;

        float _ConstructY;
        float _ConstructGap;
        fixed4 _ConstructColor;

        int building;
        fixed3 viewDir;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // 宣告原始的uv座標
            const float2 uv = IN.uv_MainTex;

            // // 宣告螢幕空間uv座標
            // float2 screen_uv = IN.screenPos.xy / IN.screenPos.w;
            //
            // // 調整紋理座標的X軸以符合當前屏幕的寬高比，防止圖像扭曲
            // const float aspect = _ScreenParams.x / _ScreenParams.y;
            // screen_uv.x = screen_uv.x * aspect;
            //
            // // 根據時間變化計算垂直方向的偏移量，實現動態滾動效果
            // const float offset = frac(_Time.y * -1);
            // screen_uv.y = frac(screen_uv.y + offset);
            //
            // // 將uv.y坐標轉換為對稱的鏡像漸變效果，使得紋理在垂直方向上呈現從中心向兩側的對稱漸變
            // screen_uv.y = 1 - abs(screen_uv.y - 0.5) * 2;

            const float noise = sin((IN.worldPos.x * IN.worldPos.z) * 60 + _Time[3] + o.Normal) / 120;

            if (IN.worldPos.y > _ConstructY + noise || IN.vface <= 0)
            {
                building = 0;
            }
            else
            {
                building = 1;
            }

            if (IN.worldPos.y > _ConstructY + noise + _ConstructGap)
                discard;

            // o.Albedo = tex2D(_MainTex, uv).rgb * _Color.rgb * screen_uv.y;
            o.Albedo = tex2D(_MainTex, uv).rgb * _Color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Emission = lerp(_ConstructColor, _Emission, building);
            // o.Alpha = c.a;
        }
        ENDCG
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom

            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            // We add our barycentric variables to the geometry struct.
            struct g2f
            {
                float4 pos : SV_POSITION;
                float3 barycentric : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                o.worldPos = mul(UNITY_MATRIX_M, o.vertex).xyz;
                o.screenPos = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
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
                o.worldPos = IN[0].worldPos;
                o.screenPos = IN[0].screenPos;
                o.barycentric = float3(1.0, 0.0, 0.0) + modifier;
                triStream.Append(o);
                o.pos = UnityObjectToClipPos(IN[1].vertex);
                o.worldPos = IN[1].worldPos;
                o.screenPos = IN[1].screenPos;
                o.barycentric = float3(0.0, 1.0, 0.0) + modifier;
                triStream.Append(o);
                o.pos = UnityObjectToClipPos(IN[2].vertex);
                o.worldPos = IN[2].worldPos;
                o.screenPos = IN[2].screenPos;
                o.barycentric = float3(0.0, 0.0, 1.0) + modifier;
                triStream.Append(o);
            }

            fixed4 _WireframeColorA;
            fixed4 _WireframeColorB;
            float _WireframeThickness;

            float _ConstructY;
            float _ConstructGap;
            fixed4 _ConstructColor;

            fixed4 frag(const g2f i) : SV_Target
            {
                const float3 unitWidth = fwidth(i.barycentric);
                float3 aliased = smoothstep(float3(0.0, 0.0, 0.0), unitWidth * _WireframeThickness, i.barycentric);
                float lineMask = 1 - min(aliased.x, min(aliased.y, aliased.z));

                // 宣告原始的uv座標
                const float2 uv = i.barycentric;

                // 宣告螢幕空間uv座標
                float2 screen_uv = i.screenPos.xy / i.screenPos.w;

                // 調整紋理座標的X軸以符合當前屏幕的寬高比，防止圖像扭曲
                const float aspect = _ScreenParams.x / _ScreenParams.y;
                screen_uv.x = screen_uv.x * aspect;

                // 根據時間變化計算垂直方向的偏移量，實現動態滾動效果
                const float offset = frac(_Time.y * -1);
                screen_uv.y = frac(screen_uv.y + offset);

                // 將uv.y坐標轉換為對稱的鏡像漸變效果，使得紋理在垂直方向上呈現從中心向兩側的對稱漸變
                screen_uv.y = 1 - abs(screen_uv.y - 0.5) * 2;

                float mask = lineMask;

                if (i.worldPos.y > _ConstructY)
                {
                    mask *= .8;
                }
                else
                {
                    mask *= .2;
                }

                mask *= screen_uv.y;

                float4 newColor = lerp(_WireframeColorA, _WireframeColorB, screen_uv.y) * lineMask;


                return newColor;
            }
            ENDCG
        }
    }
    FallBack "Standard"
}