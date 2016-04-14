
Shader "MADFINGER/Environment/Scroll 1 Layers Sine NoAlpha" {
Properties {
	_MainTex ("Base layer (RGB)", 2D) = "white" {}
	_ScrollX ("Base layer Scroll speed X", Float) = 1.0
	_ScrollY ("Base layer Scroll speed Y", Float) = 0.0
	_SineAmplX ("Base layer sine amplitude X",Float) = 0.5 
	_SineAmplY ("Base layer sine amplitude Y",Float) = 0.5
	_SineFreqX ("Base layer sine freq X",Float) = 10 
	_SineFreqY ("Base layer sine freq Y",Float) = 10
	_Color("Color", Color) = (1,1,1,1)
	
	_MMultiplier ("Layer Multiplier", Float) = 2.0
}

	
SubShader {
	Tags { "IgnoreProjector"="True" "RenderType"="Opaque" }
	
	Lighting Off
	
	LOD 100	
	
	
	CGINCLUDE
	#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	#pragma exclude_renderers molehill    
	#include "UnityCG.cginc"
	sampler2D _MainTex;

	float4 _MainTex_ST;
	
	float _ScrollX;
	float _ScrollY;
	float _MMultiplier;
	
	float _SineAmplX;
	float _SineAmplY;
	float _SineFreqX;
	float _SineFreqY;

	float4 _Color;

	struct v2f {
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
		fixed4 color : TEXCOORD1;
	};

	
	v2f vert (appdata_full v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time);
		
		o.uv.x += sin(_Time * _SineFreqX) * _SineAmplX;
		o.uv.y += sin(_Time * _SineFreqY) * _SineAmplY;
		
		o.color = _MMultiplier * _Color * v.color;
		return o;
	}
	ENDCG


	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
//		#pragma fragmentoption ARB_precision_hint_fastest		
		fixed4 frag (v2f i) : COLOR
		{
			fixed4 o;
			fixed4 tex = tex2D (_MainTex, i.uv.xy);
			
			o = tex * i.color;
						
			return o;
		}
		ENDCG 
	}	
}
}
