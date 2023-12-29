/*工具方法*/
#ifndef ZLC_SHADERUTILS_INCLUDED
#define ZLC_SHADERUTILS_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

/*Varyings Vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
    // TRANSFORM_TEX 宏执行平铺和偏移
    // 变换。
    OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
    return OUT;
}

half4 Frag(Varyings IN) : SV_Target
  {
      // SAMPLE_TEXTURE2D 宏使用给定的采样器对纹理进行
      // 采样。
      half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
      return color;
  }*/
#endif