/*
 * 该文件包含以下内容:global,define,源文件检测
*/
#define ZLC_CORE
#if DEBUG
#define ZLC_DEBUG
#endif
/*#region GLOBAL USING
using global::System; 全局引用
#endregion
#region PRAGMA 
#pragma checksum "Define.cs" “{}” 源文件校验和检测 
#endregion*/
using System.Runtime.CompilerServices;
//711ff50007219faa
[assembly: InternalsVisibleTo("ZLCEditor.Core")]
//[assembly: AssemblyVersion(AssemblyInfo.kAssemblyVersion)]
namespace ZLCEngine
{
    /// <summary>
    /// 程序集信息
    /// </summary>
    public static class AssemblyInfo
    {
        internal const string kAssemblyVersion = "1.0.0";
        internal const string kDocUrl = "https://liuxiao-dot.github.io/ZLCEngine";
    }
}
