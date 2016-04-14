Shader "Self-Illumin/SelfIlluminationMask_LightMap"
 {
Properties 
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base", 2D) = "white" {}
	_MaskTex ("Illumin(R) Unused(G,B)", 2D) = "white"{}
	_SelfIllumination ("Self Illumination", Range(0.0,4.0)) = 1.0
}

SubShader 
{
	Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
	
	CGINCLUDE
	#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	#include "UnityCG.cginc"
		
	fixed4 _Color;
	sampler2D _MainTex;
	half4 _MainTex_ST;
	sampler2D _MaskTex;
	half4 _MaskTex_ST;
	half _SelfIllumination;
		
#ifndef LIGHTMAP_OFF
	float4 unity_LightmapST;
	sampler2D unity_Lightmap;
#endif
	
	struct appdata_lm
	{
	    float4 vertex : POSITION;
	    float4 texcoord : TEXCOORD0;
	    float4 texcoord1 : TEXCOORD1;
	};
		
	struct v2f
	{
		half4 pos : POSITION;
		half4 uv : TEXCOORD0;
	};
		
	v2f vert(appdata_lm v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);	
		o.uv.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
			
#ifndef LIGHTMAP_OFF
		o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
			
		return o; 
	}
		
	half4 frag(v2f i) : COLOR 
	{
		fixed4 tex = tex2D(_MainTex, i.uv.xy);
		fixed4 mask = tex2D( _MaskTex, i.uv.xy);
		
		fixed4 lm = fixed4(1,1,1,1);
		#ifndef LIGHTMAP_OFF
		lm.rgb = DecodeLightmap (tex2D(unity_Lightmap, i.uv.zw));
		#endif
			
		return tex * _Color * (1.0 - mask.r) * lm  + tex * mask.r * _SelfIllumination;
	}
		
	ENDCG
			
	Pass
	 {
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		ENDCG
	}
}

FallBack "Diffuse"
}
