#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
StructuredBuffer<float4x4> _Matrices;
#endif

void ConfigureProcedural ()
{
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	unity_ObjectToWorld = _Matrices[unity_InstanceID];
#endif
}

float4 _BaseColor;
float2 _SequenceNumbers;

float4 GetFractalColor()
{
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	return frac(unity_InstanceID * _SequenceNumbers.x + _SequenceNumbers.y);
#else
	return _BaseColor;
#endif
}

void ShaderGraphFunction_float(float3 In, out float3 Out, out float4 FractalColor)
{
	Out = In;
	FractalColor = GetFractalColor();
}

void ShaderGraphFunction_half(half3 In, out half3 Out, out half4 FractalColor)
{
	Out = In;
	FractalColor = GetFractalColor();
}