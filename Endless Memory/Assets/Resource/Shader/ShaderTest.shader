Shader "Custom/ShaderTest" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Bump("Bump",2D) = "bump"{}
		_Snow ("Snow Level", Range(0,1) ) = 0
		_SnowColor ("Snow Color",Color) = (1.0,1.0,1.0,1.0)
		_SnowDirection ("Snow Direction", Vector) = (0,1,0)
		_SnowDepth("Snow Depth",Range(0,1)) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM

		#pragma vertex vert
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		
		sampler2D _MainTex;
		sampler2D _Bump;
		float _Snow;
		float4 _SnowColor;
		float4 _SnowDirection;
		float _SnowDepth;

		struct Input {
			float2 uv_MainTex;

			//uv coordinates for the bump map
			float2 uv_Bump;

			float3 worldNormal;
			INTERNAL_DATA
		};

		fixed4 _Color;

		void vert(inout appdata_full v) {
			//convert the normal from world coordinates to object coordinates
			float4 sn = mul(UNITY_MATRIX_IT_MV, _SnowDirection);
			if (dot(v.normal, sn.xyz) >= lerp(1, 01, (_Snow * 2) / 3)) {
				v.vertex.xyz += (sn.xyz + v.normal)* _SnowDepth * _Snow;
			}
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			// Metallic and smoothness come from slider variables
			o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));

			//lerp 插值函数，返回（1*(1-_Snow)+(-1)*_Snow)
			if (dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz) >= lerp(1, -1, _Snow))
				o.Albedo = _SnowColor.rgb;
			else
				o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
