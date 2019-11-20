Shader "Custom/VolumeRendering"
{
    Properties
    {
        _Color ("Color", Color) = (0.5, 0.75, 1, 1)
        _Volume ("Volume", 3D) = "" {}
        _DataMap ("DataMap", 2D) = "" {}
        _DataMapScale ("DataMapScale", Float) = 0.05
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
