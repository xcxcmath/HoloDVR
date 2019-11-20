﻿Shader "Custom/VolumeRendering"
{
    Properties
    {
        _Color ("Color", Color) = (0.5, 0.75, 1, 1)
        _Volume ("Volume", 3D) = "" {}
        _DataMap ("DataMap", 2D) = "" {}
        _DataMapScale ("DataMapScale", Float) = 0.05
		_Plane ("Plane", Float) = -0.5
		_PickRayPos ("PickRayPos", Vector) = (0,0,0,1)
		_PickRayDir ("PickRayDir", Vector) = (0,0,0,1)
    }
    CGINCLUDE
    ENDCG
    SubShader
    {
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #define ITERATIONS 100
            #include "./VolumeRendering.cginc"
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    FallBack "Diffuse"
}
