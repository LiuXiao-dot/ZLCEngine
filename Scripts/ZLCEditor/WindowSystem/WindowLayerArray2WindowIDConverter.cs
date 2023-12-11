using System.Collections.Generic;
using ZLCEditor.FormatSystem;
using ZLCEditor.FormatSystem.Common;
using ZLCEditor.ZLCCoding;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.WindowSystem
{
    public class WindowIDCode
    {
        public string code;
    }
    
    /// <summary>
    /// 根据窗口view生成窗口ID的枚举代码
    /// </summary>
    public class WindowLayerArray2WindowIDConverter : IFormatConverter<WindowLayerTool[],WindowIDCode>
    {
        private static string defaultCode =
            $@"namespace ZLCGenerate.Window
{{
    public enum WindowID
    {{
#for ${ZLCCoding.Constant.EnumDEF} #
        ${ZLCCoding.Constant.EnumDEF}
#end
    }}
}}";
        public WindowIDCode Convert(WindowLayerTool[] from)
        {
            List<string> enumDefs = new List<string>();
            foreach (var layerTool in from) {
                if(layerTool == null) continue;
                var gos = layerTool.gos;
                if(gos == null) continue;
                foreach (var go in gos) {
                    enumDefs.Add($"{go.prefab.name} = {go.id},");
                }
            }
            var zlcCode = new ZLCCode()
            {
                code = defaultCode,
                kvs = new SDictionary<string, object>()
                {
                    {ZLCCoding.Constant.EnumDEF, enumDefs}
                }
            };
            
            var cSharpCode = FormatManager.Convert<ZLCCode, CSharpCode>(zlcCode);

            return new WindowIDCode()
            {
                code = cSharpCode.code
            };
        }
    }
}