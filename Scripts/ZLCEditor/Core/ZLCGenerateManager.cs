using System.IO;
using UnityEditor;
using ZLCEngine.Utils;
namespace ZLCEditor
{
    /// <summary>
    ///     ZLCGenerate程序集管理器
    /// </summary>
    public class ZLCGenerateManager
    {
        public static string path = Path.Combine(Constant.ZLCGenerateURL, "ZLCGenerate.asmdef");

        [InitializeOnLoadMethod]
        public static void CheckAsmdef()
        {
            if (File.Exists(path))
                return;
            string asmdefValue = @"{
    ""name"": ""ZLCGenerate"",
            ""rootNamespace"": ""ZLCGenerate"",
            ""references"": [],
            ""includePlatforms"": [],
            ""excludePlatforms"": [],
            ""allowUnsafeCode"": false,
            ""overrideReferences"": false,
            ""precompiledReferences"": [],
            ""autoReferenced"": true,
            ""defineConstraints"": [],
            ""versionDefines"": [],
            ""noEngineReferences"": false
        }";
            FileHelper.SaveFile(asmdefValue, path);
        }
    }
}