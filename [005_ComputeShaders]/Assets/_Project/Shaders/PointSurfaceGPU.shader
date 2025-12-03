Shader "Graph/Point Surface GPU"
{
    Properties
    {
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
	}

    SubShader
    {
        CGPROGRAM
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
        #pragma target 4.5

        #include "PointGPU.hlsl"

        struct Input
        {
			float3 worldPos;
		};

        float _Smoothness;

        void ConfigureSurface (Input input, inout SurfaceOutputStandard surface)
        {
            // Albedo means whiteness in Latin.
            // It's a measure of how much light is diffusely reflected by a surface.
            // If albedo isn't fully white then part of the light energy gets
            // absorbed instead of reflected.
            surface.Albedo = input.worldPos * 0.5 + 0.5;;
            surface.Smoothness = 0.5;
        }
		ENDCG
    }
    FallBack "Diffuse"
}
