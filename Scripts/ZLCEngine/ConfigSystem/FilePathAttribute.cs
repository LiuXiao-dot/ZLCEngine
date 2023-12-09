using System;
using System.Reflection;
using ZLCEngine.Utils;
namespace ZLCEngine.ConfigSystem
{

    /// <summary>
    /// 文件路径
    /// </summary>
    public class FilePathAttribute : Attribute
    {
        /// <summary>
        /// 用户使用的运行时路径
        /// </summary>
        public const string XWPATH = "Assets/ZLC_Configs/Configs";
        /// <summary>
        /// 用户使用的编辑器路径
        /// </summary>
        public const string XWEDITORPATH = "Assets/ZLC_Configs/EditorConfigs";
        /// <summary>
        /// 框架内置运行时路径
        /// </summary>
        internal const string IN_XWPATH = "Assets/ZLC_Configs_Internal/Configs";
        /// <summary>
        /// 框架内置编辑器路径
        /// </summary>
        internal const string IN_XWEDITORPATH = "Assets/ZLC_Configs_Internal/EditorConfigs";
        /// <summary>
        /// 路径类型
        /// </summary>
        public enum PathType
        {
            /// <summary>
            /// XW的EditorResources目录
            /// </summary>
            XWEditor,
            /// <summary>
            /// XW的Resources目录
            /// </summary>
            XW,
            /// <summary>
            /// 相对与项目的绝对路径
            /// </summary>
            Absolute
        }
        /// <summary>
        /// 文件路径
        /// </summary>
        public PathType pathType;
        public bool isInternal;

        public FilePathAttribute(PathType pathType = PathType.Absolute, bool isInternal = false)
        {
            this.pathType = pathType;
            this.isInternal = isInternal;
        }

        /// <summary>
        /// 获取内部路径
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="pathType">路径类型</param>
        /// <returns></returns>
        internal static string GetInternalPath(string filePath, PathType pathType)
        {
            switch (pathType) {
                case PathType.XWEditor:
                    FileHelper.CheckDirectory(IN_XWEDITORPATH);
                    return $"{IN_XWEDITORPATH}/{filePath}";
                case PathType.XW:
                    FileHelper.CheckDirectory(IN_XWPATH);
                    return $"{IN_XWPATH}/{filePath}";
                case PathType.Absolute:
                    return filePath;
                default:
                    return filePath;
            }
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <returns></returns>
        public static string GetPath(Type type)
        {
            var attribute = type.GetCustomAttribute<FilePathAttribute>();
            if (attribute == null) return String.Empty;
            if (attribute.isInternal) {
                return GetInternalPath($"{type.Name}.asset", attribute.pathType);
            }
            return GetPath($"{type.Name}.asset", attribute.pathType);
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="pathType"></param>
        /// <returns></returns>
        private static string GetPath(string filePath, PathType pathType)
        {
            switch (pathType) {
                case PathType.XWEditor:
                    FileHelper.CheckDirectory(XWEDITORPATH);
                    return $"{XWEDITORPATH}/{filePath}";
                case PathType.XW:
                    FileHelper.CheckDirectory(XWPATH);
                    return $"{XWPATH}/{filePath}";
                case PathType.Absolute:
                    return filePath;
                default:
                    return filePath;
            }
        }
    }
}