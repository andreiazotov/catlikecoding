Shader "Custom/ColoredPoint" {
	Properties {
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		struct Input {
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;

		UNITY_INSTANCING_CBUFFER_START(Props)
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;
			o.Albedo.rgb = IN.worldPos.xyz * 0.5 + 0.5;
		}
		ENDCG
	}
	FallBack "Diffuse"
}