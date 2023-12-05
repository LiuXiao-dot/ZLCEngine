using System.IO;
using UnityEditor;
namespace ZLCEditor.Utils
{
    /// <summary>
    /// 文件相关操作
    /// </summary>
    public sealed class EditorFileHelper
    {
        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="url">文件路径</param>
        public static void DeleteFile(string url)
        {
            if (File.Exists(url)) {
                AssetDatabase.DeleteAsset(url);
            }
        }
    }
}