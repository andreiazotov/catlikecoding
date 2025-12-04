#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
StructuredBuffer<float4x4> _Matrices;
#endif

void ConfigureProcedural ()
{
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	unity_ObjectToWorld = _Matrices[unity_InstanceID];
#endif
}

void ShaderGraphFunction_float(float3 In, out float3 Out)
{
	Out = In;
}

void ShaderGraphFunction_half(half3 In, out half3 Out)
{
	Out = In;
}