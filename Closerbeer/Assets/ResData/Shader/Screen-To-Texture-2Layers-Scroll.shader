
Shader "FX/Screen To Texture 2 Layers Scroll" {
Properties {
	_MainTex ("Base layer (RGB)", 2D) = "white" {}
	_SecondMainTex("Second Base layer (RGB)", 2D) = "white"{}
	_DetailTex ("2nd layer (RGB)", 2D) = "white" {}
	_Scroll2X ("2nd layer Scroll speed X", Float) = 1.0
	_Scroll2Y ("2nd layer Scroll speed Y", Float) = 0.0
	_SineAmplX2 ("2nd layer sine amplitude X",Float) = 0.5 
	_SineAmplY2 ("2nd layer sine amplitude Y",Float) = 0.5
	_SineFreqX2 ("2nd layer sine freq X",Float) = 10 
	_SineFreqY2 ("2nd layer sine freq Y",Float) = 10
	_Color("Color", Color) = (1,1,1,1)
	
	_MMultiplier ("Layer Multiplier", Float) = 2.0
	_TransAlpha("Trans Alpha", Float) = 1.0
	_BlendAlpha("Blend Alpha", Range(0,1)) = 0.5
}

	
SubShader {
	Tags { "RenderType"="Opaque" }	
	LOD 250	
	
	CGINCLUDE
	#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	sampler2D _SecondMainTex;
	sampler2D _DetailTex;

	float4 _MainTex_ST;
	float4 _DetailTex_ST;
	
	float _Scroll2X;
	float _Scroll2Y;
	
	float _MMultiplier;
	float _TransAlpha;
	float _BlendAlpha;	

	float _SineAmplX2;
	float _SineAmplY2;
	float _SineFreqX2;
	float _SineFreqY2;
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
		o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
		o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_DetailTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time);
				
		o.uv.z += sin(_Time * _SineFreqX2) * _SineAmplX2;
		o.uv.w += sin(_Time * _SineFreqY2) * _SineAmplY2;
		
		o.color = _Color * v.color;
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
			fixed4 tex2 = tex2D (_SecondMainTex, i.uv.xy);
			fixed4 texDetail = tex2D (_DetailTex, i.uv.zw);
			
			o.rgb = tex2.rgb * _TransAlpha + ( 1.0 - _TransAlpha ) * tex.rgb * _MMultiplier;
			o.rgb = texDetail.rgb * _BlendAlpha + ( 1.0 - _BlendAlpha ) * o.rgb;
			o.rgb *= i.color.rgb;
			o.a = i.color.a;
						
			return o;
		}
		ENDCG 
	}	
}

Fallback "Diffuse"
}
