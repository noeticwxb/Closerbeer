#ifndef	MODEL_COLOR_COMMON
#define	MODEL_COLOR_COMMON

#define	SKIN_PARAM_DEFINES \
		static const fixed4	_SpecColorSkin = fixed4(0.196, 0.196, 0.196, 0.25);\
		static const float	_ShininessSkin = 1.0;\
		static const fixed2	_FresnelWeightAlpha = fixed2(0.9,0.5);\
		static const fixed3	_FresnelSkinColor = fixed3(0.588,0.372,0.459);\
		static const float3 _LightPos = float3(0.4,0.6,-0.7);\
		fixed4		_SkinColor;
		
#define	MODEL_COLOR_PS \
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