// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// shader defines with the "Shader" keyword
Shader "Custom/BasicShader"
{
	// Shader properties are declared in a separate block. Add it at the top of the shader.
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, 1) // Tint property should now show up in the properties section of our shader's inspector.
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{
		// The sub-shader has to contain at least one pass
		Pass
		{
			// The default behavior of an empty pass -> material becomes white
			// We have to indicate the start of our code with the CGPROGRAM keyword
			// And we have to terminate with the ENDCG keyword
			CGPROGRAM

			// Tell the compiler which programs to use via pragma directives
			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			// You can use the #include directive to load a different file contents into the current file
			#include "UnityCG.cginc"

			float4 _Tint;
			sampler2D _MainTex;
			float4 _MainTex_ST; // ST -> scale and transacton

			struct Interpolators
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				//float3 localPosition : TEXCOORD0;
			};

			struct VertexData
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			Interpolators MyVertexProgram(VertexData v)
			{
				Interpolators i;
				//i.localPosition = v.position.xyz;
				i.position = UnityObjectToClipPos(v.position);
				//i.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw; -> similar the next line 
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return i;
			}

			float4 MyFragmentProgram(Interpolators i) : SV_TARGET
			{
				// return _Tint;
				// return float4(i.localPosition, 1); -> dark
				//return float4(i.localPosition + 0.5, 1) * _Tint; // -> bright
				//return float4(i.uv, 1, 1);
				return tex2D(_MainTex, i.uv) * _Tint;
			}

			ENDCG
		}
	}
}
