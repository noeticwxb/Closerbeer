// - 
// - Per-pixel

Shader "ModelColor/NoColorDecal/Transparent Cutoff/PS - Cloth Specular Double Side" {
Properties {
	_Color("Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_MainTexAlpha ("Base Alpha (A)", 2D) = "white" {}
	_MaskTex ("SpecCloth(R) SpecSkin(G) Unused(B)", 2D) = "black"{}
	_SpecColorCloth ("Specular Color Cloth", Color) = (0.2, 0.2, 0.2, 1)
	_ShininessCloth ("Shininess Cloth", float) = 8	
	_FresnelWeightAlphaC("Fresnel Weight Alpha Cloth", Vector) = (0.4,0.1, 0, 0)
	_Darkness("Cloth Dark", float) = 0
	_Lightness("Cloth Light", float) = 0
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	
	_SkinColor("Skin Color", Color) = (1.0, 1.0, 1.0, 0.0)
}

SubShader {
	Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" "LightMode"="ForwardBase"}
	Cull Off
	
	CGINCLUDE
	//#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	#pragma exclude_renderers flash
	#include "UnityCG.cginc"
	#include "ModelColorCommon.cginc"
	sampler2D	_MainTex;
	sampler2D _MainTexAlpha;
	//float4 		_MainTex_ST;
	sampler2D	_MaskTex;
	fixed		_Cutoff;
	fixed4		_Color;
	fixed4		_SpecColorCloth;
	float		_ShininessCloth;
	float		_Darkness;
	float		_Lightness;
	fixed2		_FresnelWeightAlphaC;
	
	SKIN_PARAM_DEFINES 
		
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 worldN : TEXCOORD1;
		float3 worldV : TEXCOORD2;
		half4 SHLighting: TEXCOORD3;
		half3 specCloth : TEXCOORD4;
		half3 specSkin : TEXCOORD5;
	};

	
	v2f vert (appdata_base v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		
		o.worldN = mul((float3x3)_Object2World, SCALED_NORMAL);			
		o.worldV = mul(_Object2World, v.vertex).xyz;
		
		o.SHLighting.rgb	= ShadeSH9(float4(o.worldN,1)) ;
		o.specSkin			= o.SHLighting.rgb * _SpecColorSkin.rgb * _SpecColorSkin.a;	
		o.specCloth			= o.SHLighting.rgb * _SpecColorCloth.rgb * _SpecColorCloth.a;
		
		float ldot = saturate(dot( _LightPos, o.worldN ));
		o.SHLighting.a 	= 	1.0 - _Darkness * (1.0 - ldot);
		o.SHLighting.a 	*=	1.0 + _Lightness * ldot;		

		return o;
	}
	ENDCG


	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest		
		fixed4 frag (v2f i) : COLOR
		{					
			half4	c	= tex2D(_MainTex, i.uv) * _Color;
			c.a			= tex2D(_MainTexAlpha, i.uv).r * _Color.a;
			clip( c.a - _Cutoff );
			
			fixed4	mask = tex2D(_MaskTex, i.uv);
			
			MODEL_COLOR_PS

			float3	worldN = normalize( i.worldN );
			float3	worldV = normalize( (_WorldSpaceCameraPos.xyz - i.worldV) );
			float	worldVNDot = dot( worldV, worldN);
			float3	refl = worldV - 2.0 * worldVNDot * worldN;
			fixed	RefDot	=	saturate(dot(-_LightPos.xyz, refl));
			half3	specSkin = i.specSkin * pow(RefDot, _ShininessSkin );
			half3	specCloth = i.specCloth * pow(RefDot, _ShininessCloth );			
		
			c.rgb *= i.SHLighting.rgb;
			c.rgb *= mask.g < 0.01 ? i.SHLighting.a : 1.0;
			
			// Specular & Reflect
			c.rgb += specCloth * mask.r + specSkin * mask.g;
			
			// fresnel
			FRESNEL_SKIN_CLOTH
			
			return c;
		}
		ENDCG 
	}	
}

Fallback "Hidden/LightProbes Transparent Double Side" 
}

