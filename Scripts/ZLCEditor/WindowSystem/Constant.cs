using System.IO;
namespace ZLCEditor.WindowSystem
{
    public class Constant
    {
        public const string CtlCodeURL = "Window/Ctls";
        public const string ViewCodeURL = "Window/Views";
        public const string WindwoIDURL = "Window/WindowID.cs";
        public const string WindwoConfigURL = "Window/WindowConfig.cs";
        public const string PrefabURL = "Assets/Arts/Windows";
        public const string PresetURL = "Packages/com.zlc.zlcengine/Presets/Windows/RectTransforms";
        public static string FullCtlCodeURL = Path.Combine(ZLCEditor.Constant.ZLCGenerateURL, CtlCodeURL);
        public static string FullViewCodeURL = Path.Combine(ZLCEditor.Constant.ZLCGenerateURL, ViewCodeURL);
    }
}