Shader "ModelColor/Test/HueColor Transparent DoubleSide For Test" {
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1) 
		_HValue("H Value", Range(-180,180)) = 0
		_SValue("S Value", Range(-1,1)) = 0 
		_BValue("B Value", Range(-1,1)) = 0
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex("Mask (RGB)", 2D) = "black" {}
	} 
	
	CGINCLUDE
	#include "UnityCG.cginc"	
	
	sampler2D	_MainTex;
	sampler2D	_MaskTex; 
	half4		_MainTex_ST;
	half		_HValue;
	half		_SValue;
	half		_BValue;
	fixed4		_Color;
	fixed		_Cutoff;
	
	struct v2f 
	{
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half3 shLight : TEXCOORD1;
	};
	
	v2f vert(appdata_full v) 
	{
		v2f o;
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		
		float4 worldNormal = float4(mul((float3x3)_Object2World, SCALED_NORMAL), 1.0);		
        o.shLight = ShadeSH9( worldNormal );
		return o; 
	}

	fixed4 frag(v2f i) : COLOR0 
	{
		fixed4 hsb = tex2D (_MainTex, i.uv);
		clip( hsb.a - _Cutoff);

		fixed4 mask = tex2D (_MaskTex, i.uv); 

		//-------------------------------------------
		// RGB 2 HSB 
		int3	valueindex = int3(0,1,2);  // x minIndex, y midIndex, z maxIndex 
		half3	ordervalue = hsb.rgb;
		//
		if( hsb.r < hsb.b && hsb.b < hsb.g )
		{
			ordervalue = half3(hsb.r, hsb.b, hsb.g);
			valueindex = int3(0, 2, 1);
		}
		else if( hsb.g < hsb.r && hsb.r < hsb.b )
		{
			ordervalue = half3(hsb.g, hsb.r, hsb.b);
			valueindex = int3(1, 0, 2);
		}
		else if( hsb.g < hsb.b && hsb.b < hsb.r )
		{
			ordervalue = half3(hsb.g, hsb.b, hsb.r);
			valueindex = int3(1, 2, 0);
		}
		else if( hsb.b < hsb.r && hsb.r < hsb.g )
		{
			ordervalue = half3(hsb.b, hsb.r, hsb.g);
			valueindex = int3(2, 0, 1);
		}
		else if( hsb.b < hsb.g && hsb.g < hsb.r )
		{
			ordervalue = half3(hsb.b, hsb.g, hsb.r);
			valueindex = int3(2, 1, 0);
		}
		
		half3	hsbcol = 0.0;
		// B
		hsbcol.b = ordervalue.b;
		// S
		hsbcol.g = max(0.0001, 1.0 - ordervalue.r / ordervalue.b);
		// H
		hsbcol.r = valueindex.z * 120.0 + 60.0* (ordervalue.y/hsbcol.g/ordervalue.z+(1.0-1.0/hsbcol.g)) *((valueindex.z-valueindex.x+3)%3==1?1:-1);
		//
		hsbcol.r += 360.0;

		//-------------------------------------------
		// HSB 2 RGB
		fixed4	col		= 0.0;
		int		val		= hsbcol.r + _HValue;
		int3	hsbval	= int3( val + 240, val + 120, val ); 
		
		half3 x = abs( half3((hsbval % 360) - 240) );
		col.rgb = saturate( 1.0 - ( x - 60.0 ) / 60.0 );

		// s
		col.rgb += ( 1.0 - col.rgb ) * ( 1.0 - (hsbcol.g *(1.0 + _SValue)) ); // 
		// b
		col.rgb *= (hsbcol.b * (1.0 + _BValue)); // 

		// Final color
		col.rgb = (mask.r * col.rgb + (1.0 - mask.r) * hsb.rgb) * i.shLight;
		col.a = hsb.a;
		col *= _Color;
		return col;
	}
	
	ENDCG
	
	SubShader 
	{
		Cull Off
		AlphaTest Greater [_Cutoff]
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" "LightMode"="ForwardBase"}
		LOD 200
 
 		Pass 
	 	{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag 
			#pragma fragmentoption ARB_precision_hint_fastest
				
			ENDCG 
		}
	} 
	FallBack "Diffuse"
}
