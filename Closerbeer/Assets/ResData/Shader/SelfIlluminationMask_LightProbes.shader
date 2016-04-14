Shader "Self-Illumin/SelfIlluminationMask_LightProbes"
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
					
	struct v2f
	{
		half4 pos : POSITION;
		half4 uv : TEXCOORD0;
		half4 SHLighting: TEXCOORD1;
	};
		
	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);	
		o.uv.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
			
		float3 worldN = mul((float3x3)_Object2World, SCALED_NORMAL);		
		o.SHLighting	= half4( ShadeSH9( float4(worldN, 1.0) ), 1 );
			
		return o; 
	}
		
	half4 frag(v2f i) : COLOR 
	{
		fixed4 tex = tex2D(_MainTex, i.uv.xy);
		fixed4 mask = tex2D( _MaskTex, i.uv.xy);
					
		return tex * _Color * (1.0 - mask.r) * i.SHLighting  + tex * mask.r * _SelfIllumination;
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
