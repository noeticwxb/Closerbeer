#ifndef	COLORDECAL_INCLUDED_1
#define	COLORDECAL_INCLUDED_1

#define	COLOR_DECAL_DEFINES \
	sampler2D	_CDMaskTex; \
	sampler2D	_Decal0; \
	fixed4		_ModelColor0; \
	float4		_DecalBrightness; \
	float		_ColorBrightness; \
	float4		_Decal0_ST; \
	fixed4		_SkinColor;
	
#define	SKIN_PARAM_DEFINES \
		static const fixed4	_SpecColorSkin = fixed4(0.196, 0.196, 0.196, 0.25);\
		static const float	_ShininessSkin = 1.0;\
		static const fixed2	_FresnelWeightAlpha = fixed2(0.9,0.5);\
		static const fixed3	_FresnelSkinColor = fixed3(0.588,0.372,0.459);\
		static const float3 _LightPos = float3(0.4,0.6,-0.7);// == normalize( float3(0.5,0.75,-1.0) );


#define	COLOR_DECAL_V2F \
	float2 uv : TEXCOORD0; \
	float2 decal : TEXCOORD1;


#define	COLOR_DECAL_VERT \
	o.uv = v.texcoord.xy; \
	o.decal = v.texcoord.xy * _Decal0_ST.xy * 7.5;


#define	COLOR_DECAL_PS_OPAQUE \
	half4	c	= tex2D(_MainTex, i.uv) * _Color; \
	fixed4	mask	= tex2D(_MaskTex, i.uv); \
	fixed4	maskcd	= tex2D(_CDMaskTex, i.uv); \
	fixed4	decal0	= tex2D(_Decal0, i.decal); \
								\
	half	c_Gray = c.r * 0.299 + c.g * 0.587 + c.b * 0.114; \
								\
	half3	col0	= c_Gray *  _ModelColor0.rgb; \
	decal0.rgb 		= c_Gray * decal0.rgb;	\
	half	c_alpha = 1.0 - maskcd.r; \
									\
	c.rgb = lerp(c.rgb, col0, _ModelColor0.a ) * maskcd.r + c.rgb * c_alpha; \
	c.rgb = lerp(c.rgb, decal0.rgb * max(1.0, _DecalBrightness.x), min(1.0, _DecalBrightness.x * decal0.a)) * maskcd.r * _ColorBrightness +  \
			c.rgb * c_alpha; \
	c.rgb	= mask.g < 0.01 ? c.rgb : lerp( c.rgb, _SkinColor.rgb * mask.g, _SkinColor.a );

#define	COLOR_DECAL_PS_TRANSPARENT \
	half4	c	= tex2D(_MainTex, i.uv) * _Color; \
	c.a	= tex2D(_MainTexAlpha, i.uv).r * _Color.a;	\
	clip( c.a - _Cutoff ); \
					\
	fixed4	mask = tex2D(_MaskTex, i.uv); \
	fixed4	maskcd	= tex2D(_CDMaskTex, i.uv); \
	fixed4	decal0	= tex2D(_Decal0, i.decal); \
								\
	half	c_Gray = c.r * 0.299 + c.g * 0.587 + c.b * 0.114; \
							\
	half3	col0	= c_Gray *  _ModelColor0.rgb;  \
	decal0.rgb 		= c_Gray * decal0.rgb;	\
	half	c_alpha = 1.0 - maskcd.r; \
									\
	c.rgb = lerp(c.rgb, col0, _ModelColor0.a ) * maskcd.r + c.rgb * c_alpha; \
	c.rgb = lerp(c.rgb, decal0.rgb * max(1.0, _DecalBrightness.x), min(1.0, _DecalBrightness.x * decal0.a)) * maskcd.r * _ColorBrightness +  \
			c.rgb * c_alpha; \
	c.rgb	= mask.g < 0.01 ? c.rgb : lerp( c.rgb, _SkinColor.rgb * mask.g, _SkinColor.a );
			
#define	FRESNEL_SKIN_CLOTH \
		float	wVNDot		= worldVNDot < 0.0 ? worldVNDot * -1.0 : worldVNDot;\
		fixed2	FresnelWA	= mask.g > 0.01 ? _FresnelWeightAlpha : _FresnelWeightAlphaC;\
		fixed3	FresnelCol	= mask.g > 0.01 ? _FresnelSkinColor : i.SHLighting.rgb;		\
		c.rgb				= lerp( c.rgb, FresnelCol, FresnelWA.y * (1.0 - saturate(wVNDot / FresnelWA.x)));
		
#define	FRESNEL_SKIN_ONLY \
		float	wVNDot	= worldVNDot < 0.0 ? worldVNDot * -1.0 : worldVNDot;	\
		c.rgb			= lerp( c.rgb, _FresnelSkinColor, _FresnelWeightAlpha.y * (1.0 - saturate(wVNDot / _FresnelWeightAlpha.x)));

#endif