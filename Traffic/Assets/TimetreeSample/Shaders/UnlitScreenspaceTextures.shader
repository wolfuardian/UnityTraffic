Shader "Tutorial/039_ScreenspaceTextures/Unlit"
{
    //show values to edit in inspector
    Properties
    {
        _Color ("Tint", Color) = (0, 0, 0, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Float) = 1.0 // 控制動態效果的速度
        _GradientHeight ("Gradient Height", Float) = 0.5 // 控制漸層高度
    }

    SubShader
    {
        //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
        Tags
        {
            "RenderType"="Opaque" "Queue"="Geometry"

        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            //include useful shader functions
            #include "UnityCG.cginc"

            //define vertex and fragment shader
            #pragma vertex vert
            #pragma fragment frag

            //texture and transforms of the texture
            sampler2D _MainTex;
            float4 _MainTex_ST;

            //tint of the texture
            fixed4 _Color;
            float _Speed;
            float _GradientHeight;

            //the object data that's put into the vertex shader
            struct appdata
            {
                float4 vertex : POSITION;
            };

            //the data that's used to generate fragments and can be read by the fragment shader
            struct v2f
            {
                float4 position : SV_POSITION;
                float4 screenPosition : TEXCOORD0;
            };

            //the vertex shader
            v2f vert(appdata v)
            {
                v2f o;
                //convert the vertex positions from object space to clip space so they can be rendered
                o.position = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.position);
                return o;
            }

            //the fragment shader
            fixed4 frag(v2f i) : SV_TARGET
            {
                float2 textureCoordinate = i.screenPosition.xy / i.screenPosition.w;
                float aspect = _ScreenParams.x / _ScreenParams.y;
                textureCoordinate.x = textureCoordinate.x * aspect;

                // 使用時間和速度參數來創建循環的Y坐標偏移
                float yOffset = frac(_Time.y * _Speed);

                // 根據漸層高度調整Y坐標，實現循環漸層效果
                textureCoordinate.y = frac(textureCoordinate.y + yOffset) * _GradientHeight;

                textureCoordinate = TRANSFORM_TEX(textureCoordinate, _MainTex);
                fixed4 col = tex2D(_MainTex, textureCoordinate);
                col *= _Color;
                return col;
            }
            ENDCG
        }
    }
}