namespace ZLCEditor
{
    public static class Constant
    {
        public const string ZLCGenerateURL = "Assets/ZLCGenerate";
        private static string basePath = string.Empty;

        /// <summary>
        /// Assets目录的路径
        /// </summary>
        public static string BasePath
        {
            get {
                if (basePath == string.Empty) {
                    basePath = UnityEngine.Application.dataPath.Remove(UnityEngine.Application.dataPath.Length - 7, 7);
                }

                return basePath;
            }
        }

        public const string USSPath = "Packages/com.zlc.zlcengine/InspectorUI/USS";
        public const string UXMLPath = "Packages/com.zlc.zlcengine/InspectorUI/UXML";
#region StyleSheets
        public const string ZLC_TREE_VIEW = "zlc-tree-view";
#endregion
    }
}