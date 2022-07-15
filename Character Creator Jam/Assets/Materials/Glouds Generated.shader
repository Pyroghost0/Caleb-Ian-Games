Shader "Unlit Master"
{
    Properties
    {
        Vector1_93D2A305("Noise Scale", Float) = 10
        Vector1_63493A51("Noise Speed", Float) = 0.1
        Vector1_E654FBA5("Noise Height", Float) = 100
        Vector4_9A9B5D6D("Noise Remap", Vector) = (0, 1, -1, 1)
        Color_61168FF1("Color Peak", Color) = (1, 1, 1, 0)
        Color_C9E504D6("Color Valley", Color) = (0, 0, 0, 0)
        Vector1_FD53EBDD("Noise Edge 1", Float) = 1
        Vector1_BFD20BC9("Noise Edge 2", Float) = 0
        Vector1_8F16B598("Noise Power", Float) = 2
        Vector1_87735C8F("Base Scale", Float) = 5
        Vector1_C61115AD("Base Speed", Float) = 0.1
        Vector1_673519DE("Base Strength", Float) = 2
        Vector1_58337357("Fresnel Power", Float) = 1
        Vector1_14CD233A("Fresnel Opacity", Float) = 1
        Vector4_9554ED07("Rotate Projection", Vector) = (1, 0, 0, 0)
        Vector1_953B234C("Curvature Radius", Float) = 0
        Vector1_D32C2203("Fade Depth", Float) = 100
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent+0"
        }
        
        Pass
        {
            Name "Pass"
            Tags 
            { 
                // LightMode: <None>
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Off
            ZTest LEqual
            ZWrite Off
            // ColorMask: <None>
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
        
            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma shader_feature _ _SAMPLE_GI
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS 
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define FEATURES_GRAPH_VERTEX
            #pragma multi_compile_instancing
            #define SHADERPASS_UNLIT
            #define REQUIRE_DEPTH_TEXTURE
            
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float Vector1_93D2A305;
            float Vector1_63493A51;
            float Vector1_E654FBA5;
            float4 Vector4_9A9B5D6D;
            float4 Color_61168FF1;
            float4 Color_C9E504D6;
            float Vector1_FD53EBDD;
            float Vector1_BFD20BC9;
            float Vector1_8F16B598;
            float Vector1_87735C8F;
            float Vector1_C61115AD;
            float Vector1_673519DE;
            float Vector1_58337357;
            float Vector1_14CD233A;
            float4 Vector4_9554ED07;
            float Vector1_953B234C;
            float Vector1_D32C2203;
            CBUFFER_END
        
            // Graph Functions
            
            void Unity_Distance_float3(float3 A, float3 B, out float Out)
            {
                Out = distance(A, B);
            }
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Power_float(float A, float B, out float Out)
            {
                Out = pow(A, B);
            }
            
            void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
            {
                Out = A * B;
            }
            
            void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
            {
                Rotation = radians(Rotation);
            
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;
                
                Axis = normalize(Axis);
            
                float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                          one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                          one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                        };
            
                Out = mul(rot_mat,  In);
            }
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }
            
            
            float2 Unity_GradientNoise_Dir_float(float2 p)
            {
                // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }
            
            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            { 
                float2 p = UV * Scale;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
                float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }
            
            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }
            
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }
            
            void Unity_Absolute_float(float In, out float Out)
            {
                Out = abs(In);
            }
            
            void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
            {
                Out = smoothstep(Edge1, Edge2, In);
            }
            
            void Unity_Add_float3(float3 A, float3 B, out float3 Out)
            {
                Out = A + B;
            }
            
            void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
            {
                Out = lerp(A, B, T);
            }
            
            void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
            {
                Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }
            
            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }
            
            void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
            
            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }
        
            // Graph Vertex
            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 WorldSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
                float3 WorldSpacePosition;
                float3 TimeParameters;
            };
            
            struct VertexDescription
            {
                float3 VertexPosition;
                float3 VertexNormal;
                float3 VertexTangent;
            };
            
            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                float _Distance_136CFD6F_Out_2;
                Unity_Distance_float3(SHADERGRAPH_OBJECT_POSITION, IN.WorldSpacePosition, _Distance_136CFD6F_Out_2);
                float _Property_B0E4A867_Out_0 = Vector1_953B234C;
                float _Divide_B00AF04_Out_2;
                Unity_Divide_float(_Distance_136CFD6F_Out_2, _Property_B0E4A867_Out_0, _Divide_B00AF04_Out_2);
                float _Power_2F3A711F_Out_2;
                Unity_Power_float(_Distance_136CFD6F_Out_2, _Divide_B00AF04_Out_2, _Power_2F3A711F_Out_2);
                float3 _Multiply_16E8E5AE_Out_2;
                Unity_Multiply_float(IN.WorldSpaceNormal, (_Power_2F3A711F_Out_2.xxx), _Multiply_16E8E5AE_Out_2);
                float _Property_C02244D9_Out_0 = Vector1_FD53EBDD;
                float _Property_711867E_Out_0 = Vector1_BFD20BC9;
                float4 _Property_355637B4_Out_0 = Vector4_9554ED07;
                float _Split_55D74EC7_R_1 = _Property_355637B4_Out_0[0];
                float _Split_55D74EC7_G_2 = _Property_355637B4_Out_0[1];
                float _Split_55D74EC7_B_3 = _Property_355637B4_Out_0[2];
                float _Split_55D74EC7_A_4 = _Property_355637B4_Out_0[3];
                float3 _RotateAboutAxis_C9F7809B_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.ObjectSpacePosition, (_Property_355637B4_Out_0.xyz), _Split_55D74EC7_A_4, _RotateAboutAxis_C9F7809B_Out_3);
                float _Property_D97BFABE_Out_0 = Vector1_63493A51;
                float _Multiply_202D6253_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_D97BFABE_Out_0, _Multiply_202D6253_Out_2);
                float2 _TilingAndOffset_A90FF587_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), (_Multiply_202D6253_Out_2.xx), _TilingAndOffset_A90FF587_Out_3);
                float _Property_6F4A6BB1_Out_0 = Vector1_93D2A305;
                float _GradientNoise_B73932B3_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_A90FF587_Out_3, _Property_6F4A6BB1_Out_0, _GradientNoise_B73932B3_Out_2);
                float2 _TilingAndOffset_FA6F530A_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), float2 (0, 0), _TilingAndOffset_FA6F530A_Out_3);
                float _GradientNoise_B33D1CE8_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_FA6F530A_Out_3, _Property_6F4A6BB1_Out_0, _GradientNoise_B33D1CE8_Out_2);
                float _Add_1C5C787E_Out_2;
                Unity_Add_float(_GradientNoise_B73932B3_Out_2, _GradientNoise_B33D1CE8_Out_2, _Add_1C5C787E_Out_2);
                float _Divide_8A52E5ED_Out_2;
                Unity_Divide_float(_Add_1C5C787E_Out_2, 2, _Divide_8A52E5ED_Out_2);
                float _Saturate_6924EE7D_Out_1;
                Unity_Saturate_float(_Divide_8A52E5ED_Out_2, _Saturate_6924EE7D_Out_1);
                float _Property_784B88B7_Out_0 = Vector1_8F16B598;
                float _Power_6F80ADBE_Out_2;
                Unity_Power_float(_Saturate_6924EE7D_Out_1, _Property_784B88B7_Out_0, _Power_6F80ADBE_Out_2);
                float4 _Property_3647695F_Out_0 = Vector4_9A9B5D6D;
                float _Split_25890EDC_R_1 = _Property_3647695F_Out_0[0];
                float _Split_25890EDC_G_2 = _Property_3647695F_Out_0[1];
                float _Split_25890EDC_B_3 = _Property_3647695F_Out_0[2];
                float _Split_25890EDC_A_4 = _Property_3647695F_Out_0[3];
                float4 _Combine_1CB8D7E7_RGBA_4;
                float3 _Combine_1CB8D7E7_RGB_5;
                float2 _Combine_1CB8D7E7_RG_6;
                Unity_Combine_float(_Split_25890EDC_R_1, _Split_25890EDC_G_2, 0, 0, _Combine_1CB8D7E7_RGBA_4, _Combine_1CB8D7E7_RGB_5, _Combine_1CB8D7E7_RG_6);
                float4 _Combine_8902940A_RGBA_4;
                float3 _Combine_8902940A_RGB_5;
                float2 _Combine_8902940A_RG_6;
                Unity_Combine_float(_Split_25890EDC_B_3, _Split_25890EDC_A_4, 0, 0, _Combine_8902940A_RGBA_4, _Combine_8902940A_RGB_5, _Combine_8902940A_RG_6);
                float _Remap_646AB0EC_Out_3;
                Unity_Remap_float(_Power_6F80ADBE_Out_2, _Combine_1CB8D7E7_RG_6, _Combine_8902940A_RG_6, _Remap_646AB0EC_Out_3);
                float _Absolute_87CB1E8D_Out_1;
                Unity_Absolute_float(_Remap_646AB0EC_Out_3, _Absolute_87CB1E8D_Out_1);
                float _Smoothstep_A5A9390F_Out_3;
                Unity_Smoothstep_float(_Property_C02244D9_Out_0, _Property_711867E_Out_0, _Absolute_87CB1E8D_Out_1, _Smoothstep_A5A9390F_Out_3);
                float _Property_3EF9693B_Out_0 = Vector1_C61115AD;
                float _Multiply_22DD546_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_3EF9693B_Out_0, _Multiply_22DD546_Out_2);
                float2 _TilingAndOffset_D941A8D1_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), (_Multiply_22DD546_Out_2.xx), _TilingAndOffset_D941A8D1_Out_3);
                float _Property_4A91F640_Out_0 = Vector1_87735C8F;
                float _GradientNoise_DEFF9FF8_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_D941A8D1_Out_3, _Property_4A91F640_Out_0, _GradientNoise_DEFF9FF8_Out_2);
                float _Property_BB1989B9_Out_0 = Vector1_673519DE;
                float _Multiply_FB828BD0_Out_2;
                Unity_Multiply_float(_GradientNoise_DEFF9FF8_Out_2, _Property_BB1989B9_Out_0, _Multiply_FB828BD0_Out_2);
                float _Add_F3FE3695_Out_2;
                Unity_Add_float(_Smoothstep_A5A9390F_Out_3, _Multiply_FB828BD0_Out_2, _Add_F3FE3695_Out_2);
                float _Add_A2D35789_Out_2;
                Unity_Add_float(1, _Property_BB1989B9_Out_0, _Add_A2D35789_Out_2);
                float _Divide_B22B7B58_Out_2;
                Unity_Divide_float(_Add_F3FE3695_Out_2, _Add_A2D35789_Out_2, _Divide_B22B7B58_Out_2);
                float3 _Multiply_67B55C3B_Out_2;
                Unity_Multiply_float(IN.ObjectSpaceNormal, (_Divide_B22B7B58_Out_2.xxx), _Multiply_67B55C3B_Out_2);
                float _Property_2AFE3B74_Out_0 = Vector1_E654FBA5;
                float3 _Multiply_372B9611_Out_2;
                Unity_Multiply_float(_Multiply_67B55C3B_Out_2, (_Property_2AFE3B74_Out_0.xxx), _Multiply_372B9611_Out_2);
                float3 _Add_EB97F6CC_Out_2;
                Unity_Add_float3(IN.ObjectSpacePosition, _Multiply_372B9611_Out_2, _Add_EB97F6CC_Out_2);
                float3 _Add_7C8BA69B_Out_2;
                Unity_Add_float3(_Multiply_16E8E5AE_Out_2, _Add_EB97F6CC_Out_2, _Add_7C8BA69B_Out_2);
                description.VertexPosition = _Add_7C8BA69B_Out_2;
                description.VertexNormal = IN.ObjectSpaceNormal;
                description.VertexTangent = IN.ObjectSpaceTangent;
                return description;
            }
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float3 WorldSpaceNormal;
                float3 WorldSpaceViewDirection;
                float3 ObjectSpacePosition;
                float3 WorldSpacePosition;
                float4 ScreenPosition;
                float3 TimeParameters;
            };
            
            struct SurfaceDescription
            {
                float3 Color;
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _Property_5F5A01F7_Out_0 = Color_C9E504D6;
                float4 _Property_3E8CB045_Out_0 = Color_61168FF1;
                float _Property_C02244D9_Out_0 = Vector1_FD53EBDD;
                float _Property_711867E_Out_0 = Vector1_BFD20BC9;
                float4 _Property_355637B4_Out_0 = Vector4_9554ED07;
                float _Split_55D74EC7_R_1 = _Property_355637B4_Out_0[0];
                float _Split_55D74EC7_G_2 = _Property_355637B4_Out_0[1];
                float _Split_55D74EC7_B_3 = _Property_355637B4_Out_0[2];
                float _Split_55D74EC7_A_4 = _Property_355637B4_Out_0[3];
                float3 _RotateAboutAxis_C9F7809B_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.ObjectSpacePosition, (_Property_355637B4_Out_0.xyz), _Split_55D74EC7_A_4, _RotateAboutAxis_C9F7809B_Out_3);
                float _Property_D97BFABE_Out_0 = Vector1_63493A51;
                float _Multiply_202D6253_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_D97BFABE_Out_0, _Multiply_202D6253_Out_2);
                float2 _TilingAndOffset_A90FF587_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), (_Multiply_202D6253_Out_2.xx), _TilingAndOffset_A90FF587_Out_3);
                float _Property_6F4A6BB1_Out_0 = Vector1_93D2A305;
                float _GradientNoise_B73932B3_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_A90FF587_Out_3, _Property_6F4A6BB1_Out_0, _GradientNoise_B73932B3_Out_2);
                float2 _TilingAndOffset_FA6F530A_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), float2 (0, 0), _TilingAndOffset_FA6F530A_Out_3);
                float _GradientNoise_B33D1CE8_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_FA6F530A_Out_3, _Property_6F4A6BB1_Out_0, _GradientNoise_B33D1CE8_Out_2);
                float _Add_1C5C787E_Out_2;
                Unity_Add_float(_GradientNoise_B73932B3_Out_2, _GradientNoise_B33D1CE8_Out_2, _Add_1C5C787E_Out_2);
                float _Divide_8A52E5ED_Out_2;
                Unity_Divide_float(_Add_1C5C787E_Out_2, 2, _Divide_8A52E5ED_Out_2);
                float _Saturate_6924EE7D_Out_1;
                Unity_Saturate_float(_Divide_8A52E5ED_Out_2, _Saturate_6924EE7D_Out_1);
                float _Property_784B88B7_Out_0 = Vector1_8F16B598;
                float _Power_6F80ADBE_Out_2;
                Unity_Power_float(_Saturate_6924EE7D_Out_1, _Property_784B88B7_Out_0, _Power_6F80ADBE_Out_2);
                float4 _Property_3647695F_Out_0 = Vector4_9A9B5D6D;
                float _Split_25890EDC_R_1 = _Property_3647695F_Out_0[0];
                float _Split_25890EDC_G_2 = _Property_3647695F_Out_0[1];
                float _Split_25890EDC_B_3 = _Property_3647695F_Out_0[2];
                float _Split_25890EDC_A_4 = _Property_3647695F_Out_0[3];
                float4 _Combine_1CB8D7E7_RGBA_4;
                float3 _Combine_1CB8D7E7_RGB_5;
                float2 _Combine_1CB8D7E7_RG_6;
                Unity_Combine_float(_Split_25890EDC_R_1, _Split_25890EDC_G_2, 0, 0, _Combine_1CB8D7E7_RGBA_4, _Combine_1CB8D7E7_RGB_5, _Combine_1CB8D7E7_RG_6);
                float4 _Combine_8902940A_RGBA_4;
                float3 _Combine_8902940A_RGB_5;
                float2 _Combine_8902940A_RG_6;
                Unity_Combine_float(_Split_25890EDC_B_3, _Split_25890EDC_A_4, 0, 0, _Combine_8902940A_RGBA_4, _Combine_8902940A_RGB_5, _Combine_8902940A_RG_6);
                float _Remap_646AB0EC_Out_3;
                Unity_Remap_float(_Power_6F80ADBE_Out_2, _Combine_1CB8D7E7_RG_6, _Combine_8902940A_RG_6, _Remap_646AB0EC_Out_3);
                float _Absolute_87CB1E8D_Out_1;
                Unity_Absolute_float(_Remap_646AB0EC_Out_3, _Absolute_87CB1E8D_Out_1);
                float _Smoothstep_A5A9390F_Out_3;
                Unity_Smoothstep_float(_Property_C02244D9_Out_0, _Property_711867E_Out_0, _Absolute_87CB1E8D_Out_1, _Smoothstep_A5A9390F_Out_3);
                float _Property_3EF9693B_Out_0 = Vector1_C61115AD;
                float _Multiply_22DD546_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_3EF9693B_Out_0, _Multiply_22DD546_Out_2);
                float2 _TilingAndOffset_D941A8D1_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), (_Multiply_22DD546_Out_2.xx), _TilingAndOffset_D941A8D1_Out_3);
                float _Property_4A91F640_Out_0 = Vector1_87735C8F;
                float _GradientNoise_DEFF9FF8_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_D941A8D1_Out_3, _Property_4A91F640_Out_0, _GradientNoise_DEFF9FF8_Out_2);
                float _Property_BB1989B9_Out_0 = Vector1_673519DE;
                float _Multiply_FB828BD0_Out_2;
                Unity_Multiply_float(_GradientNoise_DEFF9FF8_Out_2, _Property_BB1989B9_Out_0, _Multiply_FB828BD0_Out_2);
                float _Add_F3FE3695_Out_2;
                Unity_Add_float(_Smoothstep_A5A9390F_Out_3, _Multiply_FB828BD0_Out_2, _Add_F3FE3695_Out_2);
                float _Add_A2D35789_Out_2;
                Unity_Add_float(1, _Property_BB1989B9_Out_0, _Add_A2D35789_Out_2);
                float _Divide_B22B7B58_Out_2;
                Unity_Divide_float(_Add_F3FE3695_Out_2, _Add_A2D35789_Out_2, _Divide_B22B7B58_Out_2);
                float4 _Lerp_CD951A57_Out_3;
                Unity_Lerp_float4(_Property_5F5A01F7_Out_0, _Property_3E8CB045_Out_0, (_Divide_B22B7B58_Out_2.xxxx), _Lerp_CD951A57_Out_3);
                float _Property_6447DD47_Out_0 = Vector1_58337357;
                float _FresnelEffect_BEF3AB29_Out_3;
                Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_6447DD47_Out_0, _FresnelEffect_BEF3AB29_Out_3);
                float _Multiply_F2872DC3_Out_2;
                Unity_Multiply_float(_Divide_B22B7B58_Out_2, _FresnelEffect_BEF3AB29_Out_3, _Multiply_F2872DC3_Out_2);
                float _Property_92286402_Out_0 = Vector1_14CD233A;
                float _Multiply_F3528B20_Out_2;
                Unity_Multiply_float(_Multiply_F2872DC3_Out_2, _Property_92286402_Out_0, _Multiply_F3528B20_Out_2);
                float4 _Add_32C9EA3A_Out_2;
                Unity_Add_float4(_Lerp_CD951A57_Out_3, (_Multiply_F3528B20_Out_2.xxxx), _Add_32C9EA3A_Out_2);
                float _SceneDepth_CDB69BC5_Out_1;
                Unity_SceneDepth_Eye_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_CDB69BC5_Out_1);
                float4 _ScreenPosition_12CF30E4_Out_0 = IN.ScreenPosition;
                float _Split_D3F4BAC2_R_1 = _ScreenPosition_12CF30E4_Out_0[0];
                float _Split_D3F4BAC2_G_2 = _ScreenPosition_12CF30E4_Out_0[1];
                float _Split_D3F4BAC2_B_3 = _ScreenPosition_12CF30E4_Out_0[2];
                float _Split_D3F4BAC2_A_4 = _ScreenPosition_12CF30E4_Out_0[3];
                float _Subtract_C55113A3_Out_2;
                Unity_Subtract_float(_Split_D3F4BAC2_A_4, 1, _Subtract_C55113A3_Out_2);
                float _Subtract_CF132982_Out_2;
                Unity_Subtract_float(_SceneDepth_CDB69BC5_Out_1, _Subtract_C55113A3_Out_2, _Subtract_CF132982_Out_2);
                float _Property_54BD7F51_Out_0 = Vector1_D32C2203;
                float _Divide_E10625D_Out_2;
                Unity_Divide_float(_Subtract_CF132982_Out_2, _Property_54BD7F51_Out_0, _Divide_E10625D_Out_2);
                float _Saturate_9C247A28_Out_1;
                Unity_Saturate_float(_Divide_E10625D_Out_2, _Saturate_9C247A28_Out_1);
                surface.Color = (_Add_32C9EA3A_Out_2.xyz);
                surface.Alpha = _Saturate_9C247A28_Out_1;
                surface.AlphaClipThreshold = 0;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float3 normalWS;
                float3 viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float3 interp00 : TEXCOORD0;
                float3 interp01 : TEXCOORD1;
                float3 interp02 : TEXCOORD2;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyz = input.positionWS;
                output.interp01.xyz = input.normalWS;
                output.interp02.xyz = input.viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp00.xyz;
                output.normalWS = input.interp01.xyz;
                output.viewDirectionWS = input.interp02.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);
            
                output.ObjectSpaceNormal =           input.normalOS;
                output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
                output.ObjectSpaceTangent =          input.tangentOS;
                output.ObjectSpacePosition =         input.positionOS;
                output.WorldSpacePosition =          TransformObjectToWorld(input.positionOS);
                output.TimeParameters =              _TimeParameters.xyz;
            
                return output;
            }
            
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            	// must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            	float3 unnormalizedNormalWS = input.normalWS;
                const float renormFactor = 1.0 / length(unnormalizedNormalWS);
            
            
                output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
            
            
                output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
                output.WorldSpacePosition =          input.positionWS;
                output.ObjectSpacePosition =         TransformWorldToObject(input.positionWS);
                output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags 
            { 
                "LightMode" = "ShadowCaster"
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Off
            ZTest LEqual
            ZWrite On
            // ColorMask: <None>
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            // Keywords
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS 
            #define FEATURES_GRAPH_VERTEX
            #pragma multi_compile_instancing
            #define SHADERPASS_SHADOWCASTER
            #define REQUIRE_DEPTH_TEXTURE
            
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float Vector1_93D2A305;
            float Vector1_63493A51;
            float Vector1_E654FBA5;
            float4 Vector4_9A9B5D6D;
            float4 Color_61168FF1;
            float4 Color_C9E504D6;
            float Vector1_FD53EBDD;
            float Vector1_BFD20BC9;
            float Vector1_8F16B598;
            float Vector1_87735C8F;
            float Vector1_C61115AD;
            float Vector1_673519DE;
            float Vector1_58337357;
            float Vector1_14CD233A;
            float4 Vector4_9554ED07;
            float Vector1_953B234C;
            float Vector1_D32C2203;
            CBUFFER_END
        
            // Graph Functions
            
            void Unity_Distance_float3(float3 A, float3 B, out float Out)
            {
                Out = distance(A, B);
            }
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Power_float(float A, float B, out float Out)
            {
                Out = pow(A, B);
            }
            
            void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
            {
                Out = A * B;
            }
            
            void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
            {
                Rotation = radians(Rotation);
            
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;
                
                Axis = normalize(Axis);
            
                float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                          one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                          one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                        };
            
                Out = mul(rot_mat,  In);
            }
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }
            
            
            float2 Unity_GradientNoise_Dir_float(float2 p)
            {
                // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }
            
            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            { 
                float2 p = UV * Scale;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
                float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }
            
            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }
            
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }
            
            void Unity_Absolute_float(float In, out float Out)
            {
                Out = abs(In);
            }
            
            void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
            {
                Out = smoothstep(Edge1, Edge2, In);
            }
            
            void Unity_Add_float3(float3 A, float3 B, out float3 Out)
            {
                Out = A + B;
            }
            
            void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
            
            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }
        
            // Graph Vertex
            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 WorldSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
                float3 WorldSpacePosition;
                float3 TimeParameters;
            };
            
            struct VertexDescription
            {
                float3 VertexPosition;
                float3 VertexNormal;
                float3 VertexTangent;
            };
            
            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                float _Distance_136CFD6F_Out_2;
                Unity_Distance_float3(SHADERGRAPH_OBJECT_POSITION, IN.WorldSpacePosition, _Distance_136CFD6F_Out_2);
                float _Property_B0E4A867_Out_0 = Vector1_953B234C;
                float _Divide_B00AF04_Out_2;
                Unity_Divide_float(_Distance_136CFD6F_Out_2, _Property_B0E4A867_Out_0, _Divide_B00AF04_Out_2);
                float _Power_2F3A711F_Out_2;
                Unity_Power_float(_Distance_136CFD6F_Out_2, _Divide_B00AF04_Out_2, _Power_2F3A711F_Out_2);
                float3 _Multiply_16E8E5AE_Out_2;
                Unity_Multiply_float(IN.WorldSpaceNormal, (_Power_2F3A711F_Out_2.xxx), _Multiply_16E8E5AE_Out_2);
                float _Property_C02244D9_Out_0 = Vector1_FD53EBDD;
                float _Property_711867E_Out_0 = Vector1_BFD20BC9;
                float4 _Property_355637B4_Out_0 = Vector4_9554ED07;
                float _Split_55D74EC7_R_1 = _Property_355637B4_Out_0[0];
                float _Split_55D74EC7_G_2 = _Property_355637B4_Out_0[1];
                float _Split_55D74EC7_B_3 = _Property_355637B4_Out_0[2];
                float _Split_55D74EC7_A_4 = _Property_355637B4_Out_0[3];
                float3 _RotateAboutAxis_C9F7809B_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.ObjectSpacePosition, (_Property_355637B4_Out_0.xyz), _Split_55D74EC7_A_4, _RotateAboutAxis_C9F7809B_Out_3);
                float _Property_D97BFABE_Out_0 = Vector1_63493A51;
                float _Multiply_202D6253_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_D97BFABE_Out_0, _Multiply_202D6253_Out_2);
                float2 _TilingAndOffset_A90FF587_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), (_Multiply_202D6253_Out_2.xx), _TilingAndOffset_A90FF587_Out_3);
                float _Property_6F4A6BB1_Out_0 = Vector1_93D2A305;
                float _GradientNoise_B73932B3_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_A90FF587_Out_3, _Property_6F4A6BB1_Out_0, _GradientNoise_B73932B3_Out_2);
                float2 _TilingAndOffset_FA6F530A_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), float2 (0, 0), _TilingAndOffset_FA6F530A_Out_3);
                float _GradientNoise_B33D1CE8_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_FA6F530A_Out_3, _Property_6F4A6BB1_Out_0, _GradientNoise_B33D1CE8_Out_2);
                float _Add_1C5C787E_Out_2;
                Unity_Add_float(_GradientNoise_B73932B3_Out_2, _GradientNoise_B33D1CE8_Out_2, _Add_1C5C787E_Out_2);
                float _Divide_8A52E5ED_Out_2;
                Unity_Divide_float(_Add_1C5C787E_Out_2, 2, _Divide_8A52E5ED_Out_2);
                float _Saturate_6924EE7D_Out_1;
                Unity_Saturate_float(_Divide_8A52E5ED_Out_2, _Saturate_6924EE7D_Out_1);
                float _Property_784B88B7_Out_0 = Vector1_8F16B598;
                float _Power_6F80ADBE_Out_2;
                Unity_Power_float(_Saturate_6924EE7D_Out_1, _Property_784B88B7_Out_0, _Power_6F80ADBE_Out_2);
                float4 _Property_3647695F_Out_0 = Vector4_9A9B5D6D;
                float _Split_25890EDC_R_1 = _Property_3647695F_Out_0[0];
                float _Split_25890EDC_G_2 = _Property_3647695F_Out_0[1];
                float _Split_25890EDC_B_3 = _Property_3647695F_Out_0[2];
                float _Split_25890EDC_A_4 = _Property_3647695F_Out_0[3];
                float4 _Combine_1CB8D7E7_RGBA_4;
                float3 _Combine_1CB8D7E7_RGB_5;
                float2 _Combine_1CB8D7E7_RG_6;
                Unity_Combine_float(_Split_25890EDC_R_1, _Split_25890EDC_G_2, 0, 0, _Combine_1CB8D7E7_RGBA_4, _Combine_1CB8D7E7_RGB_5, _Combine_1CB8D7E7_RG_6);
                float4 _Combine_8902940A_RGBA_4;
                float3 _Combine_8902940A_RGB_5;
                float2 _Combine_8902940A_RG_6;
                Unity_Combine_float(_Split_25890EDC_B_3, _Split_25890EDC_A_4, 0, 0, _Combine_8902940A_RGBA_4, _Combine_8902940A_RGB_5, _Combine_8902940A_RG_6);
                float _Remap_646AB0EC_Out_3;
                Unity_Remap_float(_Power_6F80ADBE_Out_2, _Combine_1CB8D7E7_RG_6, _Combine_8902940A_RG_6, _Remap_646AB0EC_Out_3);
                float _Absolute_87CB1E8D_Out_1;
                Unity_Absolute_float(_Remap_646AB0EC_Out_3, _Absolute_87CB1E8D_Out_1);
                float _Smoothstep_A5A9390F_Out_3;
                Unity_Smoothstep_float(_Property_C02244D9_Out_0, _Property_711867E_Out_0, _Absolute_87CB1E8D_Out_1, _Smoothstep_A5A9390F_Out_3);
                float _Property_3EF9693B_Out_0 = Vector1_C61115AD;
                float _Multiply_22DD546_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_3EF9693B_Out_0, _Multiply_22DD546_Out_2);
                float2 _TilingAndOffset_D941A8D1_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), (_Multiply_22DD546_Out_2.xx), _TilingAndOffset_D941A8D1_Out_3);
                float _Property_4A91F640_Out_0 = Vector1_87735C8F;
                float _GradientNoise_DEFF9FF8_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_D941A8D1_Out_3, _Property_4A91F640_Out_0, _GradientNoise_DEFF9FF8_Out_2);
                float _Property_BB1989B9_Out_0 = Vector1_673519DE;
                float _Multiply_FB828BD0_Out_2;
                Unity_Multiply_float(_GradientNoise_DEFF9FF8_Out_2, _Property_BB1989B9_Out_0, _Multiply_FB828BD0_Out_2);
                float _Add_F3FE3695_Out_2;
                Unity_Add_float(_Smoothstep_A5A9390F_Out_3, _Multiply_FB828BD0_Out_2, _Add_F3FE3695_Out_2);
                float _Add_A2D35789_Out_2;
                Unity_Add_float(1, _Property_BB1989B9_Out_0, _Add_A2D35789_Out_2);
                float _Divide_B22B7B58_Out_2;
                Unity_Divide_float(_Add_F3FE3695_Out_2, _Add_A2D35789_Out_2, _Divide_B22B7B58_Out_2);
                float3 _Multiply_67B55C3B_Out_2;
                Unity_Multiply_float(IN.ObjectSpaceNormal, (_Divide_B22B7B58_Out_2.xxx), _Multiply_67B55C3B_Out_2);
                float _Property_2AFE3B74_Out_0 = Vector1_E654FBA5;
                float3 _Multiply_372B9611_Out_2;
                Unity_Multiply_float(_Multiply_67B55C3B_Out_2, (_Property_2AFE3B74_Out_0.xxx), _Multiply_372B9611_Out_2);
                float3 _Add_EB97F6CC_Out_2;
                Unity_Add_float3(IN.ObjectSpacePosition, _Multiply_372B9611_Out_2, _Add_EB97F6CC_Out_2);
                float3 _Add_7C8BA69B_Out_2;
                Unity_Add_float3(_Multiply_16E8E5AE_Out_2, _Add_EB97F6CC_Out_2, _Add_7C8BA69B_Out_2);
                description.VertexPosition = _Add_7C8BA69B_Out_2;
                description.VertexNormal = IN.ObjectSpaceNormal;
                description.VertexTangent = IN.ObjectSpaceTangent;
                return description;
            }
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float3 WorldSpacePosition;
                float4 ScreenPosition;
            };
            
            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float _SceneDepth_CDB69BC5_Out_1;
                Unity_SceneDepth_Eye_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_CDB69BC5_Out_1);
                float4 _ScreenPosition_12CF30E4_Out_0 = IN.ScreenPosition;
                float _Split_D3F4BAC2_R_1 = _ScreenPosition_12CF30E4_Out_0[0];
                float _Split_D3F4BAC2_G_2 = _ScreenPosition_12CF30E4_Out_0[1];
                float _Split_D3F4BAC2_B_3 = _ScreenPosition_12CF30E4_Out_0[2];
                float _Split_D3F4BAC2_A_4 = _ScreenPosition_12CF30E4_Out_0[3];
                float _Subtract_C55113A3_Out_2;
                Unity_Subtract_float(_Split_D3F4BAC2_A_4, 1, _Subtract_C55113A3_Out_2);
                float _Subtract_CF132982_Out_2;
                Unity_Subtract_float(_SceneDepth_CDB69BC5_Out_1, _Subtract_C55113A3_Out_2, _Subtract_CF132982_Out_2);
                float _Property_54BD7F51_Out_0 = Vector1_D32C2203;
                float _Divide_E10625D_Out_2;
                Unity_Divide_float(_Subtract_CF132982_Out_2, _Property_54BD7F51_Out_0, _Divide_E10625D_Out_2);
                float _Saturate_9C247A28_Out_1;
                Unity_Saturate_float(_Divide_E10625D_Out_2, _Saturate_9C247A28_Out_1);
                surface.Alpha = _Saturate_9C247A28_Out_1;
                surface.AlphaClipThreshold = 0;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float3 interp00 : TEXCOORD0;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyz = input.positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp00.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);
            
                output.ObjectSpaceNormal =           input.normalOS;
                output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
                output.ObjectSpaceTangent =          input.tangentOS;
                output.ObjectSpacePosition =         input.positionOS;
                output.WorldSpacePosition =          TransformObjectToWorld(input.positionOS);
                output.TimeParameters =              _TimeParameters.xyz;
            
                return output;
            }
            
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            
            
            
            
                output.WorldSpacePosition =          input.positionWS;
                output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags 
            { 
                "LightMode" = "DepthOnly"
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Off
            ZTest LEqual
            ZWrite On
            ColorMask 0
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS 
            #define FEATURES_GRAPH_VERTEX
            #pragma multi_compile_instancing
            #define SHADERPASS_DEPTHONLY
            #define REQUIRE_DEPTH_TEXTURE
            
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float Vector1_93D2A305;
            float Vector1_63493A51;
            float Vector1_E654FBA5;
            float4 Vector4_9A9B5D6D;
            float4 Color_61168FF1;
            float4 Color_C9E504D6;
            float Vector1_FD53EBDD;
            float Vector1_BFD20BC9;
            float Vector1_8F16B598;
            float Vector1_87735C8F;
            float Vector1_C61115AD;
            float Vector1_673519DE;
            float Vector1_58337357;
            float Vector1_14CD233A;
            float4 Vector4_9554ED07;
            float Vector1_953B234C;
            float Vector1_D32C2203;
            CBUFFER_END
        
            // Graph Functions
            
            void Unity_Distance_float3(float3 A, float3 B, out float Out)
            {
                Out = distance(A, B);
            }
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Power_float(float A, float B, out float Out)
            {
                Out = pow(A, B);
            }
            
            void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
            {
                Out = A * B;
            }
            
            void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
            {
                Rotation = radians(Rotation);
            
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;
                
                Axis = normalize(Axis);
            
                float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                          one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                          one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                        };
            
                Out = mul(rot_mat,  In);
            }
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }
            
            
            float2 Unity_GradientNoise_Dir_float(float2 p)
            {
                // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }
            
            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            { 
                float2 p = UV * Scale;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
                float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }
            
            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }
            
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }
            
            void Unity_Absolute_float(float In, out float Out)
            {
                Out = abs(In);
            }
            
            void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
            {
                Out = smoothstep(Edge1, Edge2, In);
            }
            
            void Unity_Add_float3(float3 A, float3 B, out float3 Out)
            {
                Out = A + B;
            }
            
            void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
            
            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }
        
            // Graph Vertex
            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 WorldSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
                float3 WorldSpacePosition;
                float3 TimeParameters;
            };
            
            struct VertexDescription
            {
                float3 VertexPosition;
                float3 VertexNormal;
                float3 VertexTangent;
            };
            
            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                float _Distance_136CFD6F_Out_2;
                Unity_Distance_float3(SHADERGRAPH_OBJECT_POSITION, IN.WorldSpacePosition, _Distance_136CFD6F_Out_2);
                float _Property_B0E4A867_Out_0 = Vector1_953B234C;
                float _Divide_B00AF04_Out_2;
                Unity_Divide_float(_Distance_136CFD6F_Out_2, _Property_B0E4A867_Out_0, _Divide_B00AF04_Out_2);
                float _Power_2F3A711F_Out_2;
                Unity_Power_float(_Distance_136CFD6F_Out_2, _Divide_B00AF04_Out_2, _Power_2F3A711F_Out_2);
                float3 _Multiply_16E8E5AE_Out_2;
                Unity_Multiply_float(IN.WorldSpaceNormal, (_Power_2F3A711F_Out_2.xxx), _Multiply_16E8E5AE_Out_2);
                float _Property_C02244D9_Out_0 = Vector1_FD53EBDD;
                float _Property_711867E_Out_0 = Vector1_BFD20BC9;
                float4 _Property_355637B4_Out_0 = Vector4_9554ED07;
                float _Split_55D74EC7_R_1 = _Property_355637B4_Out_0[0];
                float _Split_55D74EC7_G_2 = _Property_355637B4_Out_0[1];
                float _Split_55D74EC7_B_3 = _Property_355637B4_Out_0[2];
                float _Split_55D74EC7_A_4 = _Property_355637B4_Out_0[3];
                float3 _RotateAboutAxis_C9F7809B_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.ObjectSpacePosition, (_Property_355637B4_Out_0.xyz), _Split_55D74EC7_A_4, _RotateAboutAxis_C9F7809B_Out_3);
                float _Property_D97BFABE_Out_0 = Vector1_63493A51;
                float _Multiply_202D6253_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_D97BFABE_Out_0, _Multiply_202D6253_Out_2);
                float2 _TilingAndOffset_A90FF587_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), (_Multiply_202D6253_Out_2.xx), _TilingAndOffset_A90FF587_Out_3);
                float _Property_6F4A6BB1_Out_0 = Vector1_93D2A305;
                float _GradientNoise_B73932B3_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_A90FF587_Out_3, _Property_6F4A6BB1_Out_0, _GradientNoise_B73932B3_Out_2);
                float2 _TilingAndOffset_FA6F530A_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), float2 (0, 0), _TilingAndOffset_FA6F530A_Out_3);
                float _GradientNoise_B33D1CE8_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_FA6F530A_Out_3, _Property_6F4A6BB1_Out_0, _GradientNoise_B33D1CE8_Out_2);
                float _Add_1C5C787E_Out_2;
                Unity_Add_float(_GradientNoise_B73932B3_Out_2, _GradientNoise_B33D1CE8_Out_2, _Add_1C5C787E_Out_2);
                float _Divide_8A52E5ED_Out_2;
                Unity_Divide_float(_Add_1C5C787E_Out_2, 2, _Divide_8A52E5ED_Out_2);
                float _Saturate_6924EE7D_Out_1;
                Unity_Saturate_float(_Divide_8A52E5ED_Out_2, _Saturate_6924EE7D_Out_1);
                float _Property_784B88B7_Out_0 = Vector1_8F16B598;
                float _Power_6F80ADBE_Out_2;
                Unity_Power_float(_Saturate_6924EE7D_Out_1, _Property_784B88B7_Out_0, _Power_6F80ADBE_Out_2);
                float4 _Property_3647695F_Out_0 = Vector4_9A9B5D6D;
                float _Split_25890EDC_R_1 = _Property_3647695F_Out_0[0];
                float _Split_25890EDC_G_2 = _Property_3647695F_Out_0[1];
                float _Split_25890EDC_B_3 = _Property_3647695F_Out_0[2];
                float _Split_25890EDC_A_4 = _Property_3647695F_Out_0[3];
                float4 _Combine_1CB8D7E7_RGBA_4;
                float3 _Combine_1CB8D7E7_RGB_5;
                float2 _Combine_1CB8D7E7_RG_6;
                Unity_Combine_float(_Split_25890EDC_R_1, _Split_25890EDC_G_2, 0, 0, _Combine_1CB8D7E7_RGBA_4, _Combine_1CB8D7E7_RGB_5, _Combine_1CB8D7E7_RG_6);
                float4 _Combine_8902940A_RGBA_4;
                float3 _Combine_8902940A_RGB_5;
                float2 _Combine_8902940A_RG_6;
                Unity_Combine_float(_Split_25890EDC_B_3, _Split_25890EDC_A_4, 0, 0, _Combine_8902940A_RGBA_4, _Combine_8902940A_RGB_5, _Combine_8902940A_RG_6);
                float _Remap_646AB0EC_Out_3;
                Unity_Remap_float(_Power_6F80ADBE_Out_2, _Combine_1CB8D7E7_RG_6, _Combine_8902940A_RG_6, _Remap_646AB0EC_Out_3);
                float _Absolute_87CB1E8D_Out_1;
                Unity_Absolute_float(_Remap_646AB0EC_Out_3, _Absolute_87CB1E8D_Out_1);
                float _Smoothstep_A5A9390F_Out_3;
                Unity_Smoothstep_float(_Property_C02244D9_Out_0, _Property_711867E_Out_0, _Absolute_87CB1E8D_Out_1, _Smoothstep_A5A9390F_Out_3);
                float _Property_3EF9693B_Out_0 = Vector1_C61115AD;
                float _Multiply_22DD546_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_3EF9693B_Out_0, _Multiply_22DD546_Out_2);
                float2 _TilingAndOffset_D941A8D1_Out_3;
                Unity_TilingAndOffset_float((_RotateAboutAxis_C9F7809B_Out_3.xy), float2 (1, 1), (_Multiply_22DD546_Out_2.xx), _TilingAndOffset_D941A8D1_Out_3);
                float _Property_4A91F640_Out_0 = Vector1_87735C8F;
                float _GradientNoise_DEFF9FF8_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_D941A8D1_Out_3, _Property_4A91F640_Out_0, _GradientNoise_DEFF9FF8_Out_2);
                float _Property_BB1989B9_Out_0 = Vector1_673519DE;
                float _Multiply_FB828BD0_Out_2;
                Unity_Multiply_float(_GradientNoise_DEFF9FF8_Out_2, _Property_BB1989B9_Out_0, _Multiply_FB828BD0_Out_2);
                float _Add_F3FE3695_Out_2;
                Unity_Add_float(_Smoothstep_A5A9390F_Out_3, _Multiply_FB828BD0_Out_2, _Add_F3FE3695_Out_2);
                float _Add_A2D35789_Out_2;
                Unity_Add_float(1, _Property_BB1989B9_Out_0, _Add_A2D35789_Out_2);
                float _Divide_B22B7B58_Out_2;
                Unity_Divide_float(_Add_F3FE3695_Out_2, _Add_A2D35789_Out_2, _Divide_B22B7B58_Out_2);
                float3 _Multiply_67B55C3B_Out_2;
                Unity_Multiply_float(IN.ObjectSpaceNormal, (_Divide_B22B7B58_Out_2.xxx), _Multiply_67B55C3B_Out_2);
                float _Property_2AFE3B74_Out_0 = Vector1_E654FBA5;
                float3 _Multiply_372B9611_Out_2;
                Unity_Multiply_float(_Multiply_67B55C3B_Out_2, (_Property_2AFE3B74_Out_0.xxx), _Multiply_372B9611_Out_2);
                float3 _Add_EB97F6CC_Out_2;
                Unity_Add_float3(IN.ObjectSpacePosition, _Multiply_372B9611_Out_2, _Add_EB97F6CC_Out_2);
                float3 _Add_7C8BA69B_Out_2;
                Unity_Add_float3(_Multiply_16E8E5AE_Out_2, _Add_EB97F6CC_Out_2, _Add_7C8BA69B_Out_2);
                description.VertexPosition = _Add_7C8BA69B_Out_2;
                description.VertexNormal = IN.ObjectSpaceNormal;
                description.VertexTangent = IN.ObjectSpaceTangent;
                return description;
            }
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float3 WorldSpacePosition;
                float4 ScreenPosition;
            };
            
            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float _SceneDepth_CDB69BC5_Out_1;
                Unity_SceneDepth_Eye_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_CDB69BC5_Out_1);
                float4 _ScreenPosition_12CF30E4_Out_0 = IN.ScreenPosition;
                float _Split_D3F4BAC2_R_1 = _ScreenPosition_12CF30E4_Out_0[0];
                float _Split_D3F4BAC2_G_2 = _ScreenPosition_12CF30E4_Out_0[1];
                float _Split_D3F4BAC2_B_3 = _ScreenPosition_12CF30E4_Out_0[2];
                float _Split_D3F4BAC2_A_4 = _ScreenPosition_12CF30E4_Out_0[3];
                float _Subtract_C55113A3_Out_2;
                Unity_Subtract_float(_Split_D3F4BAC2_A_4, 1, _Subtract_C55113A3_Out_2);
                float _Subtract_CF132982_Out_2;
                Unity_Subtract_float(_SceneDepth_CDB69BC5_Out_1, _Subtract_C55113A3_Out_2, _Subtract_CF132982_Out_2);
                float _Property_54BD7F51_Out_0 = Vector1_D32C2203;
                float _Divide_E10625D_Out_2;
                Unity_Divide_float(_Subtract_CF132982_Out_2, _Property_54BD7F51_Out_0, _Divide_E10625D_Out_2);
                float _Saturate_9C247A28_Out_1;
                Unity_Saturate_float(_Divide_E10625D_Out_2, _Saturate_9C247A28_Out_1);
                surface.Alpha = _Saturate_9C247A28_Out_1;
                surface.AlphaClipThreshold = 0;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float3 interp00 : TEXCOORD0;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyz = input.positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp00.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);
            
                output.ObjectSpaceNormal =           input.normalOS;
                output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
                output.ObjectSpaceTangent =          input.tangentOS;
                output.ObjectSpacePosition =         input.positionOS;
                output.WorldSpacePosition =          TransformObjectToWorld(input.positionOS);
                output.TimeParameters =              _TimeParameters.xyz;
            
                return output;
            }
            
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            
            
            
            
                output.WorldSpacePosition =          input.positionWS;
                output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
            ENDHLSL
        }
        
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}
