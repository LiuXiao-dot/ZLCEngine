/*使第一张图片根据第二张图片的颜色改变透明度
第二张图片中黑色的部分将是全透明状态
*/
Shader "ZLC/Blit2Image"
{
    Properties
    {
        [MainColor]_BaseColor ("Color", Color) = (1,1,1,1)
        [PerRendererData][MainTexture]_MainTex("图片", 2D) = "white" {}
        _MainUVRect("图片 UVRect", Vector) = (0, 0, 1, 1)
        _MainUVScale("图片 UVScale", Vector) = (0, 0, 0, 0)
        
        _BlitMap("透明度运算图片", 2D) = "white" {}
    }

    SubShader
    {
        LOD 100
        // UsePass不需要
        // 不支持GrabPass
        Tags
        {
            // 设置为不透明渲染队列（默认UI就是这个渲染队列）
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "true"
        }

        Pass
        {
            // 该Pass的名称
            Name "Blit2Image"

            Tags
            {
                "LightMode" = "SRPDefaultUnlit"
            }

            Blend SrcAlpha OneMinusSrcAlpha
            BlendOp Add

            // 声明HLSL程序
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Common/Structs.hlsl"
            #include "Common/ShaderUtils.hlsl"

            #pragma vertex BlitVert
            #pragma fragment BlitFrag

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_BlitMap);
            SAMPLER(sampler_BlitMap);
            CBUFFER_START(UnityPerMaterial)
            // 以下行声明 _BaseMap_ST 变量，以便可以
            // 在片元着色器中使用 _BaseMap 变量。为了
            // 使平铺和偏移有效，有必要使用 _ST 后缀。
            float4 _MainTex_ST;
            float4 _BlitMap_ST;
            CBUFFER_END

            float4 _MainUVRect;
            float4 _MainUVScale;

            Varyings BlitVert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // TRANSFORM_TEX 宏执行平铺和偏移
                // 变换。
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 BlitFrag(Varyings IN) : SV_Target
            {
                // SAMPLE_TEXTURE2D 宏使用给定的采样器对纹理进行
                // 采样。
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                
                float2 center = (_MainUVRect.zw - _MainUVRect.xy) / 2;
                IN.uv = IN.uv - _MainUVRect.xy - center;
                IN.uv *= _MainUVScale;
                IN.uv += center;
                IN.uv = IN.uv / (_MainUVRect.zw - _MainUVRect.xy);
                
                half4 alpha = SAMPLE_TEXTURE2D(_BlitMap, sampler_BlitMap, IN.uv);
                color.a = 1 - alpha.r;
                return color;
            }
            ENDHLSL
        }
    }
    Fallback "Sprites/Default"
}