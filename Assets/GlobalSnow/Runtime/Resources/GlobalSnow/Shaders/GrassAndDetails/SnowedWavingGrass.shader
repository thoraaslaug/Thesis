// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
Shader "GlobalSnow/TerrainEngine/Details/UniversalPipeline/WavingDoublePass"
{
    Properties
    {
        _WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
        _Cutoff ("Cutoff", float) = 0.5
    }
    SubShader
    {
        Tags {"Queue" = "Geometry+200" "RenderType" = "Grass" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "SimpleLit" }//"DisableBatching"="True"
        Cull Off
        LOD 200
        AlphaTest Greater [_Cutoff]
        ColorMask RGB

        Pass
        {
            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer

            #pragma multi_compile_fragment _ GLOBALSNOW_ZENITHAL_COVERAGE

            #pragma vertex WavingGrassVert
            #pragma fragment LitPassFragmentGrass
            #define _ALPHATEST_ON

            #include "WavingGrassInput.hlsl"
            #include "WavingGrassPasses.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #define _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "WavingGrassInput.hlsl"
            #include "WavingGrassPasses.hlsl"
            ENDHLSL
        }

        // This pass is used when drawing to a _CameraNormalsTexture texture with the forward renderer or the depthNormal prepass with the deferred renderer.
        Pass
        {
            Name "DepthNormalsOnly"
            Tags{"LightMode" = "DepthNormalsOnly"}

            ZWrite On
            Cull Off

            HLSLPROGRAM
            #pragma target 2.0

            #pragma vertex DepthNormalOnlyVertex
            #pragma fragment DepthNormalOnlyFragment

            // -------------------------------------
            // Material Keywords
            #define _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT // forward-only variant

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "WavingGrassInput.hlsl"
            #include "WavingGrassDepthNormalsPass.hlsl"
            ENDHLSL
        }
    }
}
