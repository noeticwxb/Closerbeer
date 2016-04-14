#ifndef	COLOR_INCLUDED_3
#define	COLOR_INCLUDED_3

#define	COLOR_DECAL_DEFINES \
	sampler2D	_CDMaskTex; \
	fixed4		_ModelColor0; \
	fixed4		_ModelColor1; \
	fixed4		_ModelColor2; \
	float		_ColorBrightness; \
	fixed4		_SkinColor;
	
#define	SKIN_PARAM_DEFINES \
		static const fixed4	_SpecColorSkin = fixed4(0.196, 0.196, 0.196, 0.25);\
		static const float	_ShininessSkin = 1.0;\
		static const fixed2	_FresnelWeightAlpha = fixed2(0.9,0.5);\
		static const fixed3	_FresnelSkinColor = fixed3(0.588,0.372,0.459);\
		static const float3 _LightPos = float3(0.4,0.6,-0.7);// == normalize( float3(0.5,0.75,-1.0) );


#define	MODEL_COLOR_PS \
	fixed4	maskcd	= tex2D(_CDMaskTex, i.uv); \
								\
	half	c_Gray = c.r * 0.299 + c.g * 0.587 + c.b * 0.114; \
								\
	half3	col0 = c_Gray *  _ModelColor0.rgb; \
	half3	col1 = c_Gray *  _ModelColor1.rgb; \
	half3	col2 = c_Gray *  _ModelColor2.rgb; \
	half	c_alpha = 1.0 - min(1.0, maskcd.r + maskcd.g + maskcd.b); \
									\
	c.rgb = (lerp(c.rgb, col0, _ModelColor0.a) * maskcd.r +   \
			lerp(c.rgb, col1, _ModelColor1.a) * maskcd.g +  \
			lerp(c.rgb, col2, _ModelColor2.a) * maskcd.b) * _ColorBrightness +  \
			c.rgb * c_alpha;  \
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