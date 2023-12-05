using System.IO;
using UnityEditor;
using UnityEngine;
using ZLCEditor.FormatSystem;
using ZLCEngine.Utils;
namespace ZLCEditor.WindowSystem
{
    public class WindowGoConverterManager
    {
        public static void GenerateCode(GameObject go, out MonoScript ctlCode, out MonoScript viewCode)
        {
            var ctl = FormatManager.Convert<GameObject, WindowCtlCode>(go);
            var ctlPath = Path.Combine(Constant.FullCtlCodeURL, $"{go.name}Ctl.cs");
            var absoluteCtlPath = Path.Combine(ZLCEditor.Constant.BasePath, ctlPath);
            FileHelper.SaveFile(ctl.code, absoluteCtlPath);
            var view = FormatManager.Convert<GameObject, WindowViewCode>(go);
            var viewPath = Path.Combine(Constant.FullViewCodeURL, $"{go.name}View.cs");
            var absoluteViewPath = Path.Combine(ZLCEditor.Constant.BasePath, viewPath);
            FileHelper.SaveFile(view.code, absoluteViewPath);
            AssetDatabase.Refresh();
            
            ctlCode = AssetDatabase.LoadAssetAtPath<MonoScript>(ctlPath);
            viewCode = AssetDatabase.LoadAssetAtPath<MonoScript>(viewPath);
        }
    }
}