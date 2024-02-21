Shader "Custom/3DPrinterEffectv12"
{
    Properties
    {
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector]_Metallic ("Metalness", Range(0, 1)) = 0
        [HideInInspector]_Smoothness ("Smoothness", Range(0, 1)) = 0.5

        _Speed1("Speed 1",Float) = 1.25
        _Frequency1("Frequency 1",Float) = 0.075
        _Speed2("Speed 2",Float) = 1
        _Frequency2("Frequency 2",Float) = 0.5

        _WobblySpeed("Wobbly Speed",Float) = 1
        _WobblyFrequency("Wobbly Frequency",Float) = 0.05
        _WobblyAmplitude("Wobbly Amplitude",Float) = 0.5

        _WireframeColorLightA1("Wireframe Color Light A 1", Color) = (0, 1, 0, 1)
        _WireframeColorDarkA1("Wireframe Color Dark A 1", Color) = (0, 0.5, 0, 1)
        _WireframeColorLightA2("Wireframe Color Light A 2", Color) = (0, 1, 0, 1)
        _WireframeColorDarkA2("Wireframe Color Dark A 2", Color) = (0, 0.5, 0, 1)
        _WireframeColorLightB1("Wireframe Color Light B 1", Color) = (0, 1, 0, 1)
        _WireframeColorDarkB1("Wireframe Color Dark B 1", Color) = (0, 0.25, 0, 0)
        _WireframeColorLightB2("Wireframe Color Light B 2", Color) = (0, 0, 0, 0)
        _WireframeColorDarkB2("Wireframe Color Dark B 2", Color) = (0, 0, 0, 0)
        _WireframeThicknessA("Wireframe Thickness A", Range(0, 10)) = 2
        _WireframeThicknessB("Wireframe Thickness B", Range(0, 10)) = 1

        _ConstructBrightnessA("Construct Brightness A", Range(0, 1)) = 0.18
        _ConstructBrightnessB("Construct Brightness B", Range(0, 1)) = 1
        [HDR] _ConstructEmissionA("Construct Emission A", Color) = (0,0.2,0,1)
        [HDR] _ConstructEmissionB ("Construct Emission B", color) = (0, 0, 0, 1)

        [HDR] _MeltingEmission("Melting Emission", Color) = (0.71, 1, 0, 1)
        _ConstructY ("Construct Y", Float) = 1
        _ConstructGap("Construct Gap", Float) = 0.5

        _ConstructCullA("Construct Cull A", Range(0, 1)) = 0
        _ConstructCullB("Construct Cull B", Range(0, 1)) = 0
        _WireframeAlphaA("Wireframe Alpha A", Range(0, 1)) = 1
        _WireframeAlphaB("Wireframe Alpha B", Range(0, 1)) = 1
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
        half3 _ConstructEmissionB;

        float _WobblySpeed;
        float _WobblyFrequency;
        float _WobblyAmplitude;

        int _ConstructCullA;
        int _ConstructCullB;

        float _ConstructBrightnessA;
        float _ConstructBrightnessB;

        fixed4 _ConstructEmissionA;
        fixed4 _MeltingEmission;
        float _ConstructY;
        float _ConstructGap;

        int state;
        fixed3 viewDir;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // 宣告原始的uv座標
            const float2 uv = IN.uv_MainTex;
            const float time = _Time[3] * _WobblySpeed;
            const float amplitude = 0.1 * _WobblyAmplitude;
            const float noise = sin(IN.worldPos.x * IN.worldPos.z * _WobblyFrequency + time + o.Normal) * amplitude;

            if (IN.worldPos.y > _ConstructY + noise + _ConstructGap)
                state = 0; // 0: 未建造
            if (IN.worldPos.y > _ConstructY + noise && IN.worldPos.y <= _ConstructY + noise + _ConstructGap)
                state = 1; // 1: 建造中
            if (IN.worldPos.y <= _ConstructY + noise)
                state = 2; // 2: 已建造

            float blend = 0;
            float brightness = 0;
            float3 emissionColor = _ConstructEmissionB.rgb;

            if (state == 0) // 0: 未建造
            {
                if (_ConstructCullA == 1)
                {
                    discard;
                }
                blend = 0;
                brightness = _ConstructBrightnessA;
                emissionColor = _ConstructEmissionA;
            }
            if (state == 1) // 1: 建造中
            {
                blend = 1;
                brightness = 0.0;
            }
            if (state == 2) // 2: 已建造
            {
                if (_ConstructCullB == 1)
                {
                    discard;
                }
                blend = 0;
                brightness = _ConstructBrightnessB;
                emissionColor = _ConstructEmissionB;
            }
            o.Albedo = tex2D(_MainTex, uv).rgb * _Color.rgb * brightness;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Emission = lerp(emissionColor, _MeltingEmission, blend);
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
                if (edgeLengthX > edgeLengthY && edgeLengthX > edgeLengthZ)
                {
                    modifier = float3(1.0, 0.0, 0.0);
                }
                else if (edgeLengthY > edgeLengthX && edgeLengthY > edgeLengthZ)
                {
                    modifier = float3(0.0, 1.0, 0.0);
                }
                else if (edgeLengthZ > edgeLengthX && edgeLengthZ > edgeLengthY)
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

            float _Speed1;
            float _Frequency1;
            float _Speed2;
            float _Frequency2;

            fixed4 _WireframeColorLightA1;
            fixed4 _WireframeColorDarkA1;
            fixed4 _WireframeColorLightA2;
            fixed4 _WireframeColorDarkA2;
            fixed4 _WireframeColorLightB1;
            fixed4 _WireframeColorDarkB1;
            fixed4 _WireframeColorLightB2;
            fixed4 _WireframeColorDarkB2;
            float _WireframeThicknessA;
            float _WireframeThicknessB;
            float _WireframeAlphaA;
            float _WireframeAlphaB;

            float _ConstructY;
            float _ConstructGap;
            fixed4 _MeltingEmission;

            int state;

            fixed4 frag(const g2f i) : SV_Target
            {
                float thickness = 0;

                if (i.worldPos.y > _ConstructY + _ConstructGap)
                {
                    state = 0; // 0: 未建造
                    thickness = _WireframeThicknessA;
                }
                if (i.worldPos.y > _ConstructY && i.worldPos.y <= _ConstructY + _ConstructGap)
                {
                    state = 1; // 1: 建造中
                    thickness = 0;
                }
                if (i.worldPos.y <= _ConstructY)
                {
                    state = 2; // 2: 已建造
                    thickness = _WireframeThicknessB;
                }

                const float3 unitWidth = fwidth(i.barycentric);
                float3 aliased = smoothstep(float3(0.0, 0.0, 0.0), unitWidth * thickness,
                                            i.barycentric);
                float wireframe = 1 - min(aliased.x, min(aliased.y, aliased.z));

                const float offset1 = frac(_Time.y * -_Speed1);
                const float offset2 = frac(_Time.y * -_Speed2);
                float phase1 = frac(i.worldPos.y * _Frequency1 + offset1);
                float phase2 = frac(i.worldPos.y * _Frequency2 + offset2);

                // 將uv.y坐標轉換為對稱的鏡像漸變效果，使得紋理在垂直方向上呈現從中心向兩側的對稱漸變
                phase1 = 1 - abs(phase1 - 0.5) * 2;
                phase2 = 1 - abs(phase2 - 0.5) * 2;

                float mask1 = wireframe;
                float mask2 = wireframe;

                mask1 *= phase1;
                mask2 *= phase2;

                float4 newColor1;
                float4 newColor2;

                if (state == 0) // 0: 未建造
                {
                    wireframe *= _WireframeAlphaA;
                    newColor1 = lerp(_WireframeColorLightA1, _WireframeColorDarkA1, mask1);
                    newColor2 = lerp(_WireframeColorLightA2, _WireframeColorDarkA2, mask2);
                }
                if (state == 1) // 1: 建造中
                {
                    wireframe *= 0;
                    newColor1 = float4(0.0, 0.0, 0.0, 0.0);
                    newColor2 = float4(0.0, 0.0, 0.0, 0.0);
                }
                if (state == 2) // 2: 已建造
                {
                    wireframe *= _WireframeAlphaB;
                    newColor1 = lerp(_WireframeColorLightB1, _WireframeColorDarkB1, mask1);
                    newColor2 = lerp(_WireframeColorLightB2, _WireframeColorDarkB2, mask2);
                }

                newColor1.a *= wireframe;
                newColor2.a *= wireframe;

                float4 newColor = lerp(newColor1, newColor2, 0.5);
                return newColor;
            }
            ENDCG
        }
    }
    FallBack "Standard"
}