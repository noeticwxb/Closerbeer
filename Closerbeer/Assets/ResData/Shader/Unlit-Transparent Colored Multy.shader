Shader "Unlit/Transparent Colored Multy" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGBA)", 2D) = "white" {}	
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4	_Color;		
		half4	_MainTex_ST;
		
		struct appdata_t
		{
			float4 vertex : POSITION;
			half4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};
						
		struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			half4 col: TEXCOORD1;
		};

		v2f vert(appdata_t v) {
			v2f o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.col = v.color;
					
			return o; 
		}
		
		float Overlay( float v2, float v1 )
		{
			return (v2 < 0.5) ? (2 * v1 * v2 ):(1 - 2 * (1 - v1) * (1 - v2));
		}
		
		float4 frag( v2f i ) : COLOR 
		{
			float4 Col = tex2D (_MainTex, i.uv) * i.col;
			Col.r = Overlay( Col.r, _Color.r );
			Col.g = Overlay( Col.g, _Color.g );
			Col.b = Overlay( Col.b, _Color.b );
			Col.a *= _Color.a;
			return Col;
		}
	
	ENDCG
	
	SubShader {
		Tags  {	"Queue" = "Transparent"	"IgnoreProjector" = "True"	"RenderType" = "Transparent"	}
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		AlphaTest Greater .01
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMaterial AmbientAndDiffuse
		
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
