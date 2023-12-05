using System;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Utils;
using FilePathAttribute = ZLCEngine.ConfigSystem.FilePathAttribute;
namespace ZLCEditor.Tool
{
    /// <summary>
    /// 工具的菜单
    /// </summary>
    public class ToolMenu : OdinMenuEditorWindow
    {
        /// <summary>
        /// 展示工具菜单
        /// </summary>
        [MenuItem("Tools/ZLC/菜单")]
        private static void ExcuteToolMenu()
        {
            var window = EditorWindow.GetWindow<ToolMenu>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tools = ToolConfig.Instance.toolTypes;

            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true);
            tree.Config.DrawSearchToolbar = true;
            tree.Config.DefaultMenuStyle.Height = 22;
            foreach (var toolList in tools) {
                foreach (var tool in toolList.Value.types) {
                    if (tool.realType == null) continue;
                    var realType = tool.realType;
                    var typeName = tool.realType.FullName.Replace(".", "/");
                    var path = tool.realType.GetAttribute<ToolAttribute>().path;
                    typeName = path == null ? typeName : path;

                    if (TypeHelper.IsChildOf(realType, typeof(ScriptableObject))) {
                        if (TypeHelper.IsChildOf(realType, typeof(SOSingleton<>))) {
                            var checkAsset = realType.GetBaseTypes().First(t => t.Name == typeof(SOSingleton<>).Name).GetMethod("CheckAsset", BindingFlags.Static | BindingFlags.NonPublic);
                            checkAsset.Invoke(null, null);
                        }
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