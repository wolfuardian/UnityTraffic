Shader "Alan Zucconi/Builder" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_ConstructColor("Construct Color", Color) = (1,1,1,1)
		_ConstructY ("Construct Y", Float) = 0
		_ConstructGap("Construct Gap", Float) = 0.25

		//_Margin("Margin", Range(-1,1)) = 0

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		//#pragma surface surf Standard fullforwardshadows
		#pragma surface surf Custom fullforwardshadows
		#include "UnityPBSLighting.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0	
		
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			//float3 viewDir;
			float vface : VFACE; // A negative value faces backwards (-1), while a positive value (+1) faces the camera (requires ps_3_0)
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float _ConstructY;
		float _ConstructGap;
		fixed4 _ConstructColor;

		// If 1, we are building
		// 0 otherwise
		int building;
		fixed3 viewDir;

		//float _Margin;

		//http://forum.unity3d.com/threads/surface-shader-calling-standard-lighting-function-from-custom-lighting-function.401182/
		inline half4 LightingCustom(SurfaceOutputStandard s, half3 lightDir, UnityGI gi)
		{
			if (building)
				return _ConstructColor;
			//if (dot(s.Normal, viewDir) < 0 +_Margin)
			//	return _ConstructColor;

			return LightingStandard(s, lightDir, gi);
			
		}
		inline void LightingCustom_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
		{
			LightingStandard_GI(s, data, gi);		
		}
		// http://git.ma-dev.nl/proef-pvb-groep-4/proef-pvb/raw/master/Assets/Shaders/CGIncludes/UnityPBSLighting.cginc


		void surf (Input IN, inout SurfaceOutputStandard o) {
			float s = +sin((IN.worldPos.x * IN.worldPos.z) * 60 + _Time[3] + o.Normal) / 120;

			if (IN.worldPos.y > _ConstructY + s + _ConstructGap)
				discard;

			//viewDir = IN.viewDir;

			fixed4 c = c;
			if (IN.worldPos.y > _ConstructY +s ||
				IN.vface <= 0)
			{
				c = _ConstructColor;
				building = 1;
			}
			else
			{
				c = tex2D(_MainTex, IN.uv_MainTex);
				building = 0;
			}
			o.Albedo = c.rgb * _Color;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
