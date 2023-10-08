Shader "Unlit/BucketOutlineShader"
{
	Properties{
		_Color ("Tint", Color) = (0, 0, 0, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader{
		Tags{ 
			"RenderType"="Transparent" 
			"Queue"="Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite off
		Cull off

		Pass{

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color;
            fixed _OutlineThickness;
            fixed _OutlineAnimationSpeed;
            fixed _NumberOfDots;
            fixed _DotRadius;

			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert(appdata v){
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}



            bool IsDrawDot(fixed2 uv) {
                return length(fixed2(0.5, 0.5) - uv) < _DotRadius;
            }


			fixed4 frag(v2f i) : SV_TARGET{
                float3 scale = float3(
                    length(unity_ObjectToWorld._m00_m10_m20),
                    length(unity_ObjectToWorld._m01_m11_m21),
                    length(unity_ObjectToWorld._m02_m12_m22)
                );

                fixed2 normalizedUv = i.uv * scale;

				fixed4 col = i.color;
                col.a = 0.0;


                if(normalizedUv.y > scale.y - _OutlineThickness) {
                    fixed2 animUv = fixed2(i.uv.x + _Time.x * _OutlineAnimationSpeed, 1.0 - i.uv.y);

                    fixed dotIndex = floor(animUv.x * _NumberOfDots);
                    fixed2 dotUv = fixed2(
                        frac(animUv.x * _NumberOfDots / _OutlineThickness),
                        1.0 - frac(animUv.y * scale.y / _OutlineThickness)
                    );

                    if(IsDrawDot(dotUv)) {
                        col.rgb = 1.0;
                        col.a = 1.0;
                    }

                    //to debug uv coordinate
                    //col.gb = dotUv.xy;
                    //col.a = 1.0;
                }


                if(normalizedUv.x < _OutlineThickness
                || normalizedUv.x > scale.x - _OutlineThickness
                || normalizedUv.y < _OutlineThickness) {
                    col = i.color;
                    col.a = 1.0;
                }



				return col;
			}

			ENDCG
		}
	}
}
