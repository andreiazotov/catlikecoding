// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// shader defines with the "Shader" keyword
Shader "Custom/BasicShader"
{
	// Shader properties are declared in a separate block. Add it at the top of the shader.
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, 1) // Tint property should now show up in the properties section of our shader's inspector.
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

			struct Interpolators
			{
				float4 position : SV_POSITION;
				float3 localPosition : TEXCOORD0;
			};

			Interpolators MyVertexProgram(float4 position : POSITION)
			{
				Interpolators i;
				i.localPosition = position.xyz;
				i.position = UnityObjectToClipPos(position);
				return i;
			}

			float4 MyFragmentProgram(Interpolators i) : SV_TARGET
			{
				// return _Tint;
				// return float4(i.localPosition, 1); -> dark
				return float4(i.localPosition + 0.5, 1) * _Tint; // -> bright
			}

			ENDCG
		}
	}
}
