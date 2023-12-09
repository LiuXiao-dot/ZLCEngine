using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ZLCEditor.Inspector.Menu;
using ZLCEditor.Utils;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Utils;
using FilePathAttribute = ZLCEngine.ConfigSystem.FilePathAttribute;
namespace ZLCEditor.Tool
{
    /// <summary>
    /// 工具菜单窗口
    /// </summary>
    //public class ToolMenuWindow : TwoPanelWindow
    public class ToolMenuWindow : TwoPanelWindow
    {
        /// <summary>
        /// 展示工具菜单
        /// </summary>
        [UnityEditor.MenuItem("Tools/ZLC/菜单")]
        private static void ExcuteToolMenu()
        {
            var window = EditorWindow.GetWindow<ToolMenuWindow>();
            window.titleContent = new GUIContent("ZLC菜单");
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
        }

        protected override MenuTree BuildMenuTree()
        {
            var tools = ToolConfig.Instance.toolTypes;

            MenuTree tree = new MenuTree();
            foreach (var toolList in tools) {
                foreach (var tool in toolList.Value.types) {
                    if (tool.realType == null) continue;
                    var realType = tool.realType;
                    var typeName = tool.realType.FullName.Replace(".", "/");
                    var path = tool.realType.GetCustomAttribute<ToolAttribute>().path;
                    typeName = path == null ? typeName : path;

                    if (TypeHelper.IsChildOf(realType, typeof(ScriptableObject))) {
                        if (TypeHelper.IsChildOf(realType, typeof(SOSingleton<>))) {
                            var soSingletonType = realType.GetBaseTypes().First(t => t.Name == typeof(SOSingleton<>).Name);
                            var checkAsset = soSingletonType.GetMethod("CheckAsset", BindingFlags.Static | BindingFlags.NonPublic);
                            checkAsset.Invoke(null, null);
                            // 不缓存的单例
                            tree.Add(typeName, soSingletonType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null));
                        } else {
                            // 如果是ScriptableObject，会加载所有实例，并按名字添加多个菜单项
                            var sos = EditorHelper.FindAssets<ScriptableObject>(realType.FullName, null);
                            foreach (var so in sos) {
                                tree.Add($"{typeName}/{so.name}", so);
                                // 添加子工具
                                var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(so));
                                foreach (var subAsset in subAssets) {
                                    tree.Add($"{typeName}/{so.name}/{subAsset.name}", subAsset);
                                }
                            }
                        }
                    } else if (TypeHelper.IsChildOf(realType, typeof(UnityEngine.Object))) {
                        // 暂不支持
                        Debug.LogError($"暂不支持继承自Object类的工具:{realType.FullName}");
                    } else {
                        tree.Add(typeName, Activator.CreateInstance(tool.realType));
                    }
                }
            }
            tree.SortMenuItemsByName();
            return tree;
        }
    }
}