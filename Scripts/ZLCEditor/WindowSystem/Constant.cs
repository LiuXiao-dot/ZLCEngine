using System.IO;
namespace ZLCEditor.WindowSystem
{
    public class Constant
    {
        public const string CtlCodeURL = "Window/Ctls";
        public const string ViewCodeURL = "Window/Views";
        public const string PrefabURL = "Assets/Arts/Windows";
        public static string FullCtlCodeURL = Path.Combine(global::ZLCEditor.Constant.ZLCGenerateURL, CtlCodeURL);
        public static string FullViewCodeURL = Path.Combine(global::ZLCEditor.Constant.ZLCGenerateURL, ViewCodeURL);
        public const string PresetURL = "Packages/com.zlc.zlcengine/Presets/Windows/RectTransforms";
    }
}