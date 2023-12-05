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
    }
}