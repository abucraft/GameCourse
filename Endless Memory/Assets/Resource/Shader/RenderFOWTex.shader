Shader "CustomFX/RenderFOWTex"
{
	Properties
	{
		_InSightColor("In Sight Color", Color) = (1,1,1,1)
		_VisitedColor("Visited Color", Color) = (0.3,0.3,0.3,1)
		_HideColor("Hide Color", Color) = (0,0,0,1)
		_MainTex("Albedo(RGB)", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always
		Tags{ "RenderType" = "MapBlockHide" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _HideColor;
			fixed4 frag (v2f i) : SV_Target
			{
				return float4(0,0,0,1);
			}
			ENDCG
		}
	}
	SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always
		Tags{ "RenderType" = "MapBlockVisited" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			float4 _VisitedColor;
			fixed4 frag(v2f i) : SV_Target
			{
				return float4(0.3,0.3,0.3,1);
			}
			ENDCG
		}
	}
	SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always
		Tags{ "RenderType" = "MapBlockInSight" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			float4 _InSightColor;
			fixed4 frag(v2f i) : SV_Target
			{
				return float4(1,1,1,1);
			}
			ENDCG
		}
	}
}
