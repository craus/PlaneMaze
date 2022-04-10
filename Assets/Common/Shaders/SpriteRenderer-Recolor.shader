// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "SpriteRenderer/Recolor" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Red ("Red", Vector) = (1,0.5,0.5,0.5)
	_Green ("Green", Vector) = (0.5,1,0.5,0.5)
	_Blue ("Blue", Vector) = (0.5,0.5,1,0.5)
	_Alpha ("Alpha", Vector) = (0.5,0.5,0.5,1)
	_Const ("Const", Vector) = (0.5,0.5,0.5,0.5)
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
				float4 color    : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
				fixed4 color    : COLOR;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;


			fixed4 _Red;
			fixed4 _Green;
			fixed4 _Blue;
			fixed4 _Alpha;
			fixed4 _Const;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 d = tex2D(_MainTex, i.texcoord);
				fixed4 _const = _Const;
				fixed4 _red = _Red;
				fixed4 _green = _Green;
				fixed4 _blue = _Blue;
				fixed4 _alpha = _Alpha;

//				_const = fixed4(0.5,0.5,0.5,0.5);
//	            _red = fixed4(1,0.5,0.5,0.5);
//	            _green = fixed4(0.5,1,0.5,0.5);
//	            _blue = fixed4(0.5,0.5,1,0.5);
	            //_alpha = fixed4(0.5,0.5,0.5,1);

				fixed4 c = ((_const-0.5) + d.r * (_red-0.5) + d.g * (_green-0.5) + d.b * (_blue-0.5) + d.a * (_alpha-0.5)) * 2;

				//c = (fixed4(0,0,0,0)) + d.r * (fixed4(1,0,0,0)) + d.g * (fixed4(0,1,0,0)) + d.b * (fixed4(0,0,1,0)) + d.a * (fixed4(0,0,0,1));
				//c = ((fixed4(0,0,0,0)) + d.r * (fixed4(0.5,0,0,0)) + d.g * (fixed4(0,0.5,0,0)) + d.b * (fixed4(0,0,0.5,0)) + d.a * (fixed4(0,0,0,0.5))) * 2;
				//c = ((fixed4(0.5,0.5,0.5,0.5)-0.5) + d.r * (fixed4(1,0.5,0.5,0.5)-0.5) + d.g * (fixed4(0.5,1,0.5,0.5)-0.5) + d.b * (fixed4(0.5,0.5,1,0.5)-0.5) + d.a * (fixed4(0.5,0.5,0.5,1)-0.5)) * 2;

		        c.a *= d.a;
		        c.rgb *= c.a;
				return c * i.color;
            }
        ENDCG
    }
}

}
