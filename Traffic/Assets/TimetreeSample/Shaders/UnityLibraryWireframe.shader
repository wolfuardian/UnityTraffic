Shader "Wireframe (Geometry Shader)"
{
    Properties
    {
        _LineColor ("LineColor", Color) = (1,1,1,1)
        _WireframeWidth ("Wire Thickness", RANGE(0, 1)) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" "Queue" = "Transparent"
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry frag
            #pragma fragment geom
            #pragma target 5.0

            struct g2f
            {
                float4 vertex : SV_Position;
                float2 barycentric : BARYCENTRIC;
            };

            void vert(inout float4 vertex:POSITION)
            {
            }

            [maxvertexcount(3)]
            void frag(triangle float4 patch[3]:SV_Position, inout TriangleStream<g2f> stream)
            {
                g2f o;
                for (uint i = 0; i < 3; i++)
                {
                    o.vertex = UnityObjectToClipPos(patch[i]);
                    o.barycentric = float2(fmod(i, 2.0), step(2.0, i));
                    stream.Append(o);
                }
                stream.RestartStrip();
            }
            float4 _LineColor;
            float _WireframeWidth;
            
            float4 geom(g2f PS) : SV_Target
            {
                float3 coord = float3(PS.barycentric, 1.0 - PS.barycentric.x - PS.barycentric.y);
                coord = smoothstep(fwidth(coord) * _WireframeWidth, fwidth(coord) * _WireframeWidth + fwidth(coord), coord);
                float4 color = _LineColor;
                color.a = 1.0 - min(coord.x, min(coord.y, coord.z));
                return color;
            }
            ENDCG
        }
    }
}