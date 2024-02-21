Shader "Custom/3DPrinterEffectv2"
{
    Properties
    {
        _Color ("Tint", Color) = (0, 0, 0, 1)
        _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector]_Metallic ("Metalness", Range(0, 1)) = 0
        [HideInInspector]_Smoothness ("Smoothness", Range(0, 1)) = 0.5
        [HDR] _Emission ("Emission", color) = (0,0,0)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"="Geometry"
        }
        Blend SrcAlpha OneMinusSrcAlpha
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
            o.Emission = _Emission;
        }
        ENDCG
    }
    FallBack "Standard"
}