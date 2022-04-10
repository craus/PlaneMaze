// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/White Noise" {
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
					float4 worldPos : TEXCOORD1;
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
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
	                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					//o.scrPos = ComputeScreenPos(o.vertex);
					return o;
				}

				float rand(float3 co)
				{
					co = round(co*10+0.501)/100;
				    return frac(sin( dot(float4(frac(sin(co.x*co.x*co.x*12174)), frac(sin(co.y*co.y*co.y*67274)),frac(sin(co.z*co.z*co.z*39961)), _Time.x), float4(52.9898,28.233, 91.221,72.178)) ) * 13758.5453);
				}
			
				fixed4 frag (v2f i) : SV_Target
				{
					float r = rand(i.worldPos.xyz);
					half4 col = half4(r, r, r, _Color.a);
					return col;
				}
			ENDCG
		}
	}
}
