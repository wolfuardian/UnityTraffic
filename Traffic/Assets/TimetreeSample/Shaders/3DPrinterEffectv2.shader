Shader "Custom/3DPrinterEffectv2"
{
    Properties
    {
        _Color ("Tint", Color) = (0, 0, 0, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0, 1)) = 0
        _Metallic ("Metalness", Range(0, 1)) = 0
        [HDR] _Emission ("Emission", color) = (0,0,0)
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"="Geometry"
        }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        float4 _MainTex_ST;
        fixed4 _Color;

        half _Smoothness;
        half _Metallic;
        half3 _Emission;

        struct Input
        {
            float4 screenPos;
        };

        void surf(Input i, inout SurfaceOutputStandard o)
        {
            // 將屏幕空間位置轉換為紋理座標
            float2 uv = i.screenPos.xy / i.screenPos.w;

            // 調整紋理座標的X軸以符合當前屏幕的寬高比，防止圖像扭曲
            const float aspect = _ScreenParams.x / _ScreenParams.y;
            uv.x = uv.x * aspect;

            // 使用Unity的內建函數轉換紋理座標，以適應紋理的包裹方式
            uv = TRANSFORM_TEX(uv, _MainTex);

            // 根據時間變化計算垂直方向的偏移量，實現動態滾動效果
            const float offset = frac(_Time.y * -1);
            uv.y = frac(uv.y + offset);

            // 將uv.y坐標轉換為對稱的鏡像漸變效果，使得紋理在垂直方向上呈現從中心向兩側的對稱漸變
            uv.y = 1 - abs(uv.y - 0.5) * 2;


            o.Albedo = uv.y;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Emission = _Emission;
        }
        ENDCG

        //The second pass where we render the outlines
        Pass
        {
            Cull Front

            CGPROGRAM
            //include useful shader functions
            #include "UnityCG.cginc"

            //define vertex and fragment shader
            #pragma vertex vert
            #pragma fragment frag

            //tint of the texture
            fixed4 _OutlineColor;
            float _OutlineThickness;

            //the object data that's put into the vertex shader
            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
            };

            //the data that's used to generate fragments and can be read by the fragment shader
            struct v2f
            {
                float4 position : SV_POSITION;
            };

            //the vertex shader
            v2f vert(appdata v)
            {
                v2f o;
                //convert the vertex positions from object space to clip space so they can be rendered
                o.position = UnityObjectToClipPos(v.vertex + normalize(v.normal) * _OutlineThickness);
                return o;
            }

            //the fragment shader
            fixed4 frag(v2f i) : SV_TARGET
            {
                return _OutlineColor;
            }
            ENDCG
        }

    }
    FallBack "Standard"
}