#ifndef ZLC_STRUCTS_INCLUDED
#define ZLC_STRUCTS_INCLUDED

struct Attributes
{
    float4 positionOS : POSITION;
    // uv 变量包含给定顶点的纹理上的
    // UV 坐标。
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    // uv 变量包含给定顶点的纹理上的
    // UV 坐标。
    float2 uv : TEXCOORD0;
};


#endif