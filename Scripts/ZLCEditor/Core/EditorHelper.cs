using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using ZLCEditor.Utils;
using ZLCEngine.Utils;
using Object = UnityEngine.Object;
using SearchUtils = UnityEditor.Search.SearchUtils;
namespace ZLCEditor
{
    public sealed class EditorHelper
    {

        /*public static void CombineWindows()
        {
            Type sceneViewType = typeof(SceneView);
            //创建最外层容器
            object containerInstance = ContainerWindowWrap.CreateInstance();
            //创建分屏容器
            object splitViewInstance = SplitViewWrap.CreateInstance();
            //设置根容器
            ContainerWindowWrap.SetRootView(containerInstance, splitViewInstance);
    
            //tool面板与timeline面板分割面板
            object window_sceneSplitView = SplitViewWrap.CreateInstance();
            SplitViewWrap.SetPosition(window_sceneSplitView, new Rect(0, 0, 1920, 1080));
            //设置垂直状态
            SplitViewWrap.SetVertical(window_sceneSplitView, false);
            object sceneDockArea = DockAreaWrap.CreateInstance();
            var sceneWidth = 1080 * 1080 / 1920;
            DockAreaWrap.SetPosition(sceneDockArea, new Rect(0, 0, sceneWidth, 1080));
            var sceneWindow = ScriptableObject.CreateInstance(sceneViewType) as SceneView;
            sceneWindow.orthographic = true;
            sceneWindow.in2DMode = true;
            
            DockAreaWrap.AddTab(sceneDockArea, sceneWindow);
            SplitViewWrap.AddChild(window_sceneSplitView, sceneDockArea);
    
            //添加timeline窗体
            object windowDock = DockAreaWrap.CreateInstance();
            DockAreaWrap.SetPosition(windowDock, new Rect(sceneWidth, 0, 1920 - sceneWidth, 1080));
            EditorWindow windowEditorWindow = (EditorWindow)ScriptableObject.CreateInstance(typeof(WindowEditorWindow));
            //windowEditorWindow.minSize = new Vector2(1920 - sceneWidth, 1080);
            DockAreaWrap.AddTab(windowDock, windowEditorWindow);
            SplitViewWrap.AddChild(window_sceneSplitView, windowDock);
    
            //添加tool_timeline切割窗体
            SplitViewWrap.AddChild(splitViewInstance, window_sceneSplitView);
    
            EditorWindowWrap.MakeParentsSettingsMatchMe(sceneWindow);
            EditorWindowWrap.MakeParentsSettingsMatchMe(windowEditorWindow);
    
            ContainerWindowWrap.SetPosition(containerInstance, new Rect(0, 0, 1920, 1080));
            SplitViewWrap.SetPosition(splitViewInstance, new Rect(0, 0, 1920, 1080));
            ContainerWindowWrap.Show(containerInstance, 0, true, false, true);
            ContainerWindowWrap.OnResize(containerInstance);
        }*/

        [Flags]
        public enum AssemblyFilterType
        {
            /// <summary>
            ///     自定义的程序集
            /// </summary>
            Custom = 1,
            /// <summary>
            ///     Unity的程序集
            /// </summary>
            Unity = 2,
            /// <summary>
            ///     第三方程序集
            /// </summary>
            Other = 4,
            /// <summary>
            ///     ZLC内置dll
            /// </summary>
            Internal = 8,
            /// <summary>
            ///     全部
            /// </summary>
            All = Custom | Unity | Other | Internal
        }
        /// <summary>
        ///     检测->创建目标路径的目录
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateDir(string filePath)
        {
            var dir = GetDir(filePath);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        ///     获取文件的文件夹路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetDir(string filePath)
        {
            return filePath.Replace(Path.GetFileName(filePath), "");
        }

        /// <summary>
        ///     检测并创建文件夹，并在目标路径生成T的实例
        /// </summary>
        /// <param name="t"></param>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        public static void CreateAsset<T>(T t, string path) where T : Object
        {
            CreateDir(path);
            AssetDatabase.CreateAsset(t, path);
        }

        /// <summary>
        ///     检测并创建文件夹，如果目标文件不存在，则创建一个，如果存在，不会覆盖
        /// </summary>
        /// <param name="t"></param>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>true:创建成功 false:已有文件</returns>
        public static bool CreateAssetIfNotExit<T>(T t, string path) where T : Object
        {
            CreateDir(path);
            var result = AssetDatabase.FindAssets(path);
            if (result == null || result.Length == 0) {
                // 新建
                AssetDatabase.CreateAsset(t, path);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     查找所有资产
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> FindAssets<T>() where T : Object
        {
            return FindAssets<T>(null);
        }

        /// <summary>
        ///     查找searchFolders下的所有资产
        /// </summary>
        /// <param name="searchFolders"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> FindAssets<T>(string[] searchFolders) where T : Object
        {
            return FindAssets<T>(typeof(T).Name, searchFolders);
        }

        /// <summary>
        ///     查找所有资产
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> FindAssets<T>(string typeName, string[] serachFolders = null) where T : Object
        {
            var ts = AssetDatabase.FindAssets($"t:{typeName}", serachFolders);
            return ts.Select(temp => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(temp))).ToList();
        }

        /// <summary>
        ///     使用SearchService查找资产
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void SearchPackagesAssets<T>(string searchText, Action<IList<T>> callback) where T : Object
        {
            Action<SearchContext, IList<SearchItem>> OnSearchCompleted = (SearchContext context, IList<SearchItem> items) =>
            {
                var assets = new List<T>();
                foreach (var item in items) {
                    var path = SearchUtils.GetAssetPath(item);
                    var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                    assets.Add(asset);
                }
                callback(assets);
            };
            // 刷新ZLC提供的程序集
            SearchService.Request(searchText, OnSearchCompleted, SearchFlags.Default | SearchFlags.Packages | SearchFlags.WantsMore);
        }

        public static void GetAllChildType(List<Type> temp, AssemblyFilterType filterType, Type targetType)
        {
            Action<AssemblyFilterType> getAllChildType = singleType =>
            {
                var assmblysConfigSO = AssemblysConfigSO.Instance;
                switch (singleType) {
                    case AssemblyFilterType.Custom:
                        EditorAssemblyHelper.GetAllChildType(assmblysConfigSO.selfDlls, temp, targetType);
                        EditorAssemblyHelper.GetAllChildType(assmblysConfigSO.selfAssemblies, temp, targetType);
                        break;
                    case AssemblyFilterType.Unity:
                        EditorAssemblyHelper.GetAllChildType(assmblysConfigSO.unityDlls, temp, targetType);
                        EditorAssemblyHelper.GetAllChildType(assmblysConfigSO.unityAssemblies, temp, targetType);
                        break;
                    case AssemblyFilterType.Other:
                        EditorAssemblyHelper.GetAllChildType(assmblysConfigSO.otherDlls, temp, targetType);
                        EditorAssemblyHelper.GetAllChildType(assmblysConfigSO.otherAssemblies, temp, targetType);
                        break;
                    case AssemblyFilterType.Internal:
                        EditorAssemblyHelper.GetAllChildType(assmblysConfigSO.defaultDlls, temp, targetType);
                        EditorAssemblyHelper.GetAllChildType(assmblysConfigSO.defaultAssemblies, temp, targetType);
                        break;
                    default:
                        Debug.LogError($"filterType数据分割错误.source:{filterType} single:{singleType}");
                        break;
                }
            };

            EnumHelper.ForEachFlag(filterType, getAllChildType);
        }

        public static void GetAllChildType<T>(List<Type> temp, AssemblyFilterType filterType)
        {
            GetAllChildType(temp, filterType, typeof(T));
        }

        public static void GetAllMarkedType<T>(List<Type> temp, AssemblyFilterType filterType) where T : Attribute
        {
            GetAllMarkedType(temp, filterType, typeof(T));
        }

        public static void GetAllMarkedType(List<Type> temp, AssemblyFilterType filterType, Type targetType)
        {
            Action<AssemblyFilterType> getAllChildType = singleType =>
            {
                var assmblysConfigSO = AssemblysConfigSO.Instance;
                switch (singleType) {
                    case AssemblyFilterType.Custom:
                        EditorAssemblyHelper.GetAttributedTypes(assmblysConfigSO.selfDlls, temp, targetType);
                        EditorAssemblyHelper.GetAttributedTypes(assmblysConfigSO.selfAssemblies, temp, targetType);
                        break;
                    case AssemblyFilterType.Unity:
                        EditorAssemblyHelper.GetAttributedTypes(assmblysConfigSO.unityDlls, temp, targetType);
                        EditorAssemblyHelper.GetAttributedTypes(assmblysConfigSO.unityAssemblies, temp, targetType);
                        break;
                    case AssemblyFilterType.Other:
                        EditorAssemblyHelper.GetAttributedTypes(assmblysConfigSO.otherDlls, temp, targetType);
                        EditorAssemblyHelper.GetAttributedTypes(assmblysConfigSO.otherAssemblies, temp, targetType);
                        break;
                    case AssemblyFilterType.Internal:
                        EditorAssemblyHelper.GetAttributedTypes(assmblysConfigSO.defaultDlls, temp, targetType);
                        EditorAssemblyHelper.GetAttributedTypes(assmblysConfigSO.defaultAssemblies, temp, targetType);
                        break;
                    default:
                        Debug.LogError($"filterType数据分割错误.source:{filterType} single:{singleType}");
                        break;
                }
            };
            EnumHelper.ForEachFlag(filterType, getAllChildType);
        }


        public static void GetAllInterfaces(List<Type> temp, AssemblyFilterType filterType)
        {
            Action<AssemblyFilterType> getAllInrtfaces = singleType =>
            {
                var assmblysConfigSO = AssemblysConfigSO.Instance;
                switch (singleType) {
                    case AssemblyFilterType.Custom:
                        EditorAssemblyHelper.GetInterfaces(assmblysConfigSO.selfDlls, temp);
                        EditorAssemblyHelper.GetInterfaces(assmblysConfigSO.selfAssemblies, temp);
                        break;
                    case AssemblyFilterType.Unity:
                        EditorAssemblyHelper.GetInterfaces(assmblysConfigSO.unityDlls, temp);
                        EditorAssemblyHelper.GetInterfaces(assmblysConfigSO.unityAssemblies, temp);
                        break;
                    case AssemblyFilterType.Other:
                        EditorAssemblyHelper.GetInterfaces(assmblysConfigSO.otherDlls, temp);
                        EditorAssemblyHelper.GetInterfaces(assmblysConfigSO.otherAssemblies, temp);
                        break;
                    case AssemblyFilterType.Internal:
                        EditorAssemblyHelper.GetInterfaces(assmblysConfigSO.defaultDlls, temp);
                        EditorAssemblyHelper.GetInterfaces(assmblysConfigSO.defaultAssemblies, temp);
                        break;
                    default:
                        Debug.LogError($"filterType数据分割错误.source:{filterType} single:{singleType}");
                        break;
                }
            };
            EnumHelper.ForEachFlag(filterType, getAllInrtfaces);
        }
    }
}