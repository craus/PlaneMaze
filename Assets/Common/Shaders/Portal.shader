Shader "Portal" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }
    Category {
       SubShader {
            Pass {
               SetTexture [_MainTex] {
                    constantColor [_Color]
                    Combine texture * constant, texture * constant 
                 }
            }
			CGPROGRAM

			#pragma surface surf Unlit nolightmap nodynlightmap nodirlightmap noforwardadd  nometa

			struct Input {
				float2 uv_MainTex;
				float4 screenPos;
			};

			sampler2D _MainTex;

			void surf(Input IN, inout SurfaceOutput o) {
				float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
				o.Albedo = tex2D(_MainTex, screenUV).rgb;
			}

			fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
			{
				fixed4 c;
				c.rgb = s.Albedo; 
				return c;
			}

			half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
			{
				half4 c;
				c.rgb = -s.Albedo/2; 
				return c;
			}

			ENDCG
        } 
    }
}