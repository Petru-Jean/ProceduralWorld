// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Stencil"
{
    Properties
    {
        _OutlineColor("Outline color", Color) = (0,0,0,1)
        _OutlineWidth("Outline width", Range(-2.0, 2.0)) = 0
        _Angle("Switch shader on angle", Range(0.0, 180.0)) = 89
        _DiffDebug("Diff", Range(0, 100)) = 0
        _Offset("Offset", Vector) = (0,0,0,0)
    }

        SubShader
    {
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "DisableBatching" = "True"}
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        ZWrite Off

        Pass
        {
            Stencil
            {
                Ref 255
                Comp always
                Pass keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 vert(appdata_base v) : SV_POSITION {
                return UnityObjectToClipPos(v.vertex);
            }
            void frag() { return; }
            ENDCG
        }

        Pass
        {
            Stencil
            {
                Ref 0
                Comp Equal
            }

            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma target 4.0
            #pragma vertex vert
            #pragma fragment frag
            uniform float4 _OutlineColor;
            uniform float _OutlineWidth;
            uniform float _Angle;
            uniform float _DiffDebug;
            uniform float2 _Offset;

            struct appdata {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
            };
            struct v2f {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v) {
                appdata original = v;

                //This shader consists of 2 ways of generating outline that are dynamically switched based on demiliter angle
                //If vertex normal is pointed away from object origin then custom outline generation is used (based on scaling along the origin-vertex vector)
                //Otherwise the old-school normal vector scaling is used
                //This way prevents weird artifacts from being created when using either of the methods
                v.vertex.xyz *= _OutlineWidth;

                v.vertex.x -= _Offset.x;
                v.vertex.y -= _Offset.y;
                /*float diff = _OutlineWidth - 1;
                float halfDiff = diff / 2.0;

                v.vertex.x -= halfDiff * 32.0;
                v.vertex.y -= halfDiff * 32.0;*/

              /*  if (degrees(acos(dot(scaleDir.xyz, v.normal.xyz))) > _Angle) {
                    v.vertex.xyz += normalize(v.normal.xyz) * _OutlineWidth;
                }
                else {
                v.vertex.xyz += scaleDir * _OutlineWidth;
                }*/

               // v.vertex.xyz *= _OutlineWidth;
                //v.vertex.x -= _DiffDebug;
                //v.vertex.y -= _DiffDebug;
               // float diff = _OutlineWidth - 1;
               // float halfDiff = diff / 2.0;

               // v.vertex.x *= _DiffDebug;
   
               //// v.vertex.y *= _DiffDebug;
               // //v.vertex.xyz -= float3(_DiffDebug, _DiffDebug, 0);

               // if (diff > 0)
               // {

               //     v.vertex.xyz *= _OutlineWidth;
  

               //     //_DiffDebug = diff;
               // }

                

                //v.vertex.x += 1;

                //v.vertex.xyz *= _OutlineWidth;

                //v.vertex.xyz += scaleDir * _OutlineWidth;
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }
            half4 frag(v2f i) : SV_Target {
                return _OutlineColor;
            }
            ENDCG
        }

        Pass
        {
            Stencil
            {
                Ref 0
                Comp always
                Pass replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 vert(appdata_base v) : SV_POSITION {
                return UnityObjectToClipPos(v.vertex);
            }
            void frag() { return; }
            ENDCG
        }
    }
}