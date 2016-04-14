// - 
// - Per-pixel specular

Shader "ModelColor/ColorDecal/Opaque - 3/PS - Specular - CD3" 
{
Properties {
	_Color("Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_MaskTex ("Unused(R) SpecSkin(G) Unused(B)", 2D) = "black"{}
	
	_CDMaskTex ("Color Decal Mask (RGB)", 2D) = "black" {}
	_Decal0 ("Decal1 (RGBA)", 2D) = "black" {}
	_Decal1 ("Decal2 (RGBA)", 2D) = "black" {}
	_Decal2 ("Decal3 (RGBA)", 2D) = "black" {}
	_ModelColor0 ("Model Color1", Color) = (1.0, 1.0, 1.0, 0.0)
	_ModelColor1 ("Model Color2", Color) = (1.0, 1.0, 1.0, 0.0)
	_ModelColor2 ("Model Color3", Color) = (1.0, 1.0, 1.0, 0.0)
	_DecalBrightness("Decal Brightness", Vector) = (1.0, 1.0, 1.0, 1.0)
	_ColorBrightness("Color Brightness", Float) = 1.0
	
	_SkinColor("Skin Color", Color) = (1.0, 1.0, 1.0, 0.0)
}

SubShader {
	Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
	
	
	CGINCLUDE
	//#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	#pragma exclude_renderers flash
	#pragma target 3.0
	#include "UnityCG.cginc"
	#include "ColorDecal_3.cginc"
	sampler2D	_MainTex;
	//float4 		_MainTex_ST;
	sampler2D	_MaskTex;
	fixed4		_Color;
	
	// Model color & decal
	COLOR_DECAL_DEFINES 
	
	SKIN_PARAM_DEFINES 
		
	struct v2f {
		float4 pos : SV_POSITION;
		
		// Model color & decal
		COLOR_DECAL_V2F 
		
		float3 worldN : TEXCOORD2;
		float3 worldV : TEXCOORD3;
		half4 SHLighting: TEXCOORD4;
		half3 specSkin : TEXCOORD5;
	};

	
	v2f vert (appdata_base v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		
		// Model color & decal
		COLOR_DECAL_VERT 
		
		o.worldN = mul((float3x3)_Object2World, SCALED_NORMAL);			
		o.worldV = mul(_Object2World, v.vertex).xyz;
		
		o.SHLighting.rgb	= ShadeSH9(float4(o.worldN,1)) ;
		o.specSkin			= o.SHLighting.rgb * _SpecColorSkin.rgb * _SpecColorSkin.a;	
		
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
			// Model color & decal
			COLOR_DECAL_PS_OPAQUE 

			float3	worldN = normalize( i.worldN );
			float3	worldV = normalize( (_WorldSpaceCameraPos.xyz - i.worldV) );
			float	worldVNDot = dot( worldV, worldN);
			float3	refl = worldV - 2.0 * worldVNDot * worldN;
			fixed	RefDot	=	saturate(dot(-_LightPos.xyz, refl));
			half3	specSkin = i.specSkin * pow(RefDot, _ShininessSkin );
			
			c.rgb *= i.SHLighting.rgb;
			
			// Specular
			c.rgb += specSkin * mask.g;
			
			// fresnel
			FRESNEL_SKIN_ONLY
			
			return c;
		}
		ENDCG 
	}	
}

Fallback "Hidden/LightProbes Diffuse - CD3"
}

