
Shader "RenrenMusic/Particle/MultiplyConst" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_TintColor ("TintColor", Color) = (1.0, 1.0, 1.0, 1.0)	
		_MulConst( "Mul Const", float ) = 1	
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4	_TintColor;		
		half4	_MainTex_ST;
		float	_MulConst;
						
		struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			fixed4 vertexColor : COLOR;
		};

		v2f vert(appdata_full v) {
			v2f o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.vertexColor = v.color * _TintColor;
					
			return o; 
		}
		
		float4 frag( v2f i ) : COLOR {	
			float4 Col = tex2D (_MainTex, i.uv.xy) * i.vertexColor;
			Col.rgb *= _MulConst;
			return Col;
		}
	
	ENDCG
	
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" } //
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend DstColor OneMinusSrcAlpha
		
	Pass {
	
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}
				
	} 
	FallBack "Diffuse"
}
