using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using ZLCEngine.Utils;
namespace ZLCEditor.ResSystem
{
    /// <summary>
    ///     资源管理帮助类
    /// </summary>
    public sealed class ResHelper
    {
        /// <summary>
        ///     支持的文件类型
        /// </summary>
        private static HashSet<string> extensions = new HashSet<string>
        {
            ".prefab",
            ".mat",
            ".png",
            ".unity",
            ".asset",
            ".physicsMaterial",
            ".ttf",
            ".fbx",
            ".shader",
            ".wav",
            ".mixer",
            ".inputactions",
            ".spriteatlasv2"
        };

        /// <summary>
        ///     资源打包
        /// </summary>
        public static string Build(IList<string> dirs)
        {
            try {
                // 同步AddressablesSync(dirs);
                Sync(dirs);
                // Addressables打包
                AddressableAssetSettings.BuildPlayerContent();
                // todo:其他
                // Build
                BuildPlayerOptions options = new BuildPlayerOptions();
                options.options = BuildOptions.ShowBuiltPlayer;
                BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(options));
                return "Build成功";
            }
            catch (Exception e) {
                return e.StackTrace;
            }
        }

        /// <summary>
        ///     资源同步
        ///     1.检测目录是否存在，不存在添加错误信息
        ///     todo:使用多个GroupTemplateObject
        /// </summary>
        public static string Sync(IList<string> dirs)
        {
            if (!(dirs is { Count: > 0 })) {
                return "未设置目录";
            }
            AddressableAssetSettings setting = AddressableAssetSettingsDefaultObject.GetSettings(true);
            AddressableAssetGroupTemplate template = setting.GetGroupTemplateObject(0) as AddressableAssetGroupTemplate;

            string info;
            using (zstring.Block()) {
                zstring log = "";
                // 创建一个group
                void createGroup(string dir, string groupName = null)
                {
                    groupName ??= FileHelper.GetDirectoryName(dir);
                    AddressableAssetGroup group = setting.FindGroup(groupName);
                    if (group == null) {
                        // 创建新的Group
                        group = setting.CreateGroup(groupName, false, true, true, template.SchemaObjects);
                        log = log + "\n创建Group:" + dir;
                    }

                    IEnumerable<string> fileUrls = GetInvalidFiles(dir);
                    foreach (string fileUrl in fileUrls) {
                        string fileName = Path.GetFileName(fileUrl);
                        string guid = AssetDatabase.AssetPathToGUID(fileUrl);
                        AddressableAssetEntry entry = group.GetAssetEntry(guid);
                        if (entry == null) {
                            entry = setting.CreateOrMoveEntry(guid, group);
                            log = log + "\n创建Entry:" + fileUrl;
                        }
                        if (entry == null) continue; // 创建失败
                        entry.SetAddress(fileName);
                    }
                    string[] childDirs = Directory.GetDirectories(dir);
                    foreach (string childDir in childDirs) {
                        createGroup(childDir, groupName);
                    }
                }

                foreach (string dir in dirs) {
                    if (Directory.Exists(dir))
                        createGroup(dir);
                }
                info = log.Intern();
            }
            return info;
        }

        /// <summary>
        ///     资源整理
        /// </summary>
        public static void Sort()
        {

        }

        /// <summary>
        ///     资源检测
        /// </summary>
        public static void Check()
        {

        }

        /// <summary>
        ///     获取符合条件的文件
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetInvalidFiles(string dir)
        {
            return Directory.EnumerateFiles(dir).Where(IsFileMatch);
        }

        /// <summary>
        ///     文件筛选
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileMatch(string fileName)
        {
            string end = Path.GetExtension(fileName);
            return extensions.Contains(end);
        }
    }
}