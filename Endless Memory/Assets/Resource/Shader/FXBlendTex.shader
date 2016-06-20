Shader "CustomFX/FXBlendTex"
{
	Properties
	{
		_MainTex("Texture", 2D) = "" {}
	_BlendTex("BlendTex (RGB)",2D) = "black" {}
	_BlendExponent("Blend Exponent",Range(0,1)) = 1
	}
		CGINCLUDE



#include "UnityCG.cginc"


		struct v2f
	{
		float2 uv[2] : TEXCOORD0;
		float4 pos : SV_POSITION;
	};

	sampler2D _MainTex;
	sampler2D _BlendTex;
	float _BlendExponent;
	half4 _BlendTex_TexelSize;
	half4 _MainTex_TexelSize;
	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv[0] = v.texcoord.xy;
		o.uv[1] = v.texcoord.xy;
#if UNITY_UV_STARTS_AT_TOP
		//if (_BloomTex_TexelSize.x < 0)
		o.uv[1].y = 1 - o.uv[1].y;
#endif

		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		// sample the texture
		half4 mainColor = tex2D(_MainTex, i.uv[0]);
		half4 blendColor = tex2D(_BlendTex, i.uv[1]);
		blendColor += (1-_BlendExponent)*half4(1, 1, 1, 1);
		if (blendColor.r > 1)
			blendColor.r = 1;
		if (blendColor.g > 1)
			blendColor.g = 1;
		if (blendColor.b > 1)
			blendColor.b = 1;
		half4 finalColor = blendColor*mainColor;
		return finalColor;
	}
		ENDCG

		SubShader
	{
		ZTest Always Cull Off ZWrite Off

			Pass
		{

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
			ENDCG
		}


	}
	Fallback off
}
