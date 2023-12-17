using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ZLCEditor.Inspector.Menu;
using ZLCEditor.Utils;
using ZLCEngine.ConfigSystem;
using ZLCEngine.SerializeTypes;
using ZLCEngine.Utils;
using Object = UnityEngine.Object;
namespace ZLCEditor.Tool
{
    /// <summary>
    ///     工具菜单窗口
    /// </summary>
    //public class ToolMenuWindow : TwoPanelWindow
    public class ToolMenuWindow : TwoPanelWindow
    {
        /// <summary>
        ///     展示工具菜单
        /// </summary>
        [MenuItem("Tools/ZLC/菜单")]
        private static void ExcuteToolMenu()
        {
            ToolMenuWindow window = GetWindow<ToolMenuWindow>();
            window.titleContent = new GUIContent("ZLC菜单");
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
        }

        protected override MenuTree BuildMenuTree()
        {
            SDictionary<string, SType[]> tools = ToolConfig.Instance.toolTypes;

            MenuTree tree = new MenuTree();
            foreach (KeyValuePair<string, SType[]> toolList in tools) {
                foreach (SType tool in toolList.Value) {
                    if (tool.realType == null) continue;
                    Type realType = tool.realType;
                    string typeName = tool.realType.FullName.Replace(".", "/");
                    string path = tool.realType.GetCustomAttribute<ToolAttribute>().path;
                    typeName = path == null ? typeName : path;

                    if (TypeHelper.IsChildOf(realType, typeof(ScriptableObject))) {
                        if (TypeHelper.IsChildOf(realType, typeof(SOSingleton<>))) {
                            Type soSingletonType = realType.GetBaseTypes().First(t => t.Name == typeof(SOSingleton<>).Name);
                            MethodInfo checkAsset = soSingletonType.GetMethod("CheckAsset", BindingFlags.Static | BindingFlags.NonPublic);
                            checkAsset.Invoke(null, null);
                            // 不缓存的单例
                            tree.Add(typeName, soSingletonType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null));
                        } else {
                            // 如果是ScriptableObject，会加载所有实例，并按名字添加多个菜单项
                            List<ScriptableObject> sos = EditorHelper.FindAssets<ScriptableObject>(realType.FullName);
                            foreach (ScriptableObject so in sos) {
                                if (realType != so.GetType()) {
                                    // 说明查找的是子资产，此时不添加该父资产
                                    Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(so));
                                    foreach (Object subAsset in subAssets) {
                                        tree.Add($"{typeName}/{subAsset.name}", subAsset);
                                    }
                                } else {
                                    tree.Add($"{typeName}/{so.name}", so);
                                    // 添加子工具
                                    Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(so));
                                    foreach (Object subAsset in subAssets) {
                                        tree.Add($"{typeName}/{so.name}/{subAsset.name}", subAsset);
                                    }
                                }
                            }
                        }
                    } else if (TypeHelper.IsChildOf(realType, typeof(Object))) {
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