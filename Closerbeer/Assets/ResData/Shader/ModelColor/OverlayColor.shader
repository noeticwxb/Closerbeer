Shader "ModelColor/Test/OverlayColor For Test" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_LayerColor("Layer 1 Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex ("Mask (RGB)", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _MaskTex;
		fixed4 _Color;
		fixed4 _LayerColor;

		struct Input {
			float2 uv_MainTex;
		};
		
		half3 Overlay( half3 v1, half3 v2 )
		{
			return (v2 < 0.5) ? (2.0 * v1 * v2 ):(1.0 - 2.0 * (1.0 - v1) * (1.0 - v2));
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 col = tex2D (_MainTex, IN.uv_MainTex);
			half4 mask = tex2D (_MaskTex, IN.uv_MainTex);
			
			o.Albedo = (col.rgb * (1.0 - mask.r) + mask.r * Overlay( _LayerColor.rgb, col.rgb )) * _Color.rgb;
			//
			o.Alpha = col.a * _Color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

