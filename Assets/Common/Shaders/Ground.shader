// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Ground" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100


	    ZWrite On
	    Blend SrcAlpha OneMinusSrcAlpha
	
		Pass {  
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
			
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
                    float3 worldPos : TEXCOORD1;
				};		
			
				struct Input {
					float2 uv_MainTex;
					float4 screenPos;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Color;
			
				v2f vert (appdata_t v)
				{
					v2f o;
	                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				float sqr(float x) {
					return x*x;
				}

				float sinsum(float3 a, float3 b) {
					return sin(dot(a,b));
				}

				float mul(float x, float y) {
					return x*y;
				}

				float rand(float3 i, int seed) {
					i *= 1e-2;
					return frac(sin(dot(i,float3(2.2128, 59.7781, 12.0082))+seed)*42300);
				}

				float pix(float3 p) {
					float3 f = frac(p);
					float3 i = float3(int(p.x), int(p.y), int(p.z));
					return rand(i,2);
				}

				float fun(float3 p) {
					float3 f = frac(p);
					f = f * f * (3.0 - 2.0 * f);
					float3 p000 = float3(int(p.x), int(p.y), int(p.z));
					float3 p100 = p000+float3(1,0,0);
					float3 p010 = p000+float3(0,1,0);
					float3 p110 = p000+float3(1,1,0);
					float3 p001 = p000+float3(0,0,1);
					float3 p101 = p000+float3(1,0,1);
					float3 p011 = p000+float3(0,1,1);
					float3 p111 = p000+float3(1,1,1);
					float c000 = pix(p000);
					float c100 = pix(p100);
					float c010 = pix(p010);
					float c110 = pix(p110);
					float c001 = pix(p001);
					float c101 = pix(p101);
					float c011 = pix(p011);
					float c111 = pix(p111);
					return lerp(lerp(lerp(c000, c100, f.x), lerp(c010, c110, f.x), f.y), lerp(lerp(c001, c101, f.x), lerp(c011, c111, f.x), f.y), f.z);
				}
			
				fixed4 frag (v2f i) : SV_Target
				{
					float r = fun(i.worldPos);
					half4 col = half4(r, r, r, 1);
					return col;
				}
			ENDCG
		}
	}
}
