using System.Collections.Generic;
using ZLCEditor.FormatSystem;
using ZLCEditor.FormatSystem.Common;
using ZLCEditor.ZLCCoding;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.WindowSystem
{
    public class WindowConfigCode
    {
        public string code;
    }

    public class WindowLayerArray2WindowConfigConverter : IFormatConverter<WindowLayerTool[],WindowConfigCode>
    {
        private static string defaultCode =
            $@"using UnityEngine;
using ZLCEngine.Interfaces;
using ZLCEngine.WindowSystem;
namespace ZLCGenerate.Window
{{
    public class WindowConfig : IWindowConfig
    {{
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {{
            IWindowConfig.Instance = new WindowConfig();
        }}

        public IWindowCtl CreateWindowCtl(int id)
        {{
            switch(id){{
#for ${ZLCCoding.Constant.ForDEF}1$ #
                ${ZLCCoding.Constant.ForDEF}1$
#end
                default:
                    return null;
            }}
        }}

        public string GetWindowPath(int id)
        {{
            switch(id){{
#for ${ZLCCoding.Constant.ForDEF}2$ #
                ${ZLCCoding.Constant.ForDEF}2$
#end
                default:
                    return """";
            }}
        }}
    }}
}}";
        public WindowConfigCode Convert(WindowLayerTool[] from)
        {
            var forDef1s = new List<string>();
            var forDef2s = new List<string>();
            foreach (var layerTool in from) {
                if(layerTool == null) continue;
                List<WindowGo> gos = layerTool.gos;
                if(gos == null) continue;
                foreach (var go in gos) {
                    forDef1s.Add(@$"case {go.id}:
                    return new {go.ctlCode.GetClass()}();");
                    forDef2s.Add($@"case {go.id}:
                    return ""{go.prefab.name}.prefab"";");
                }
            }
            var zlcCode = new ZLCCode()
            {
                code = defaultCode,
                kvs = new SDictionary<string, object>()
                {
                    {
                        $"{ZLCCoding.Constant.ForDEF}1", forDef1s
                    },
                    {
                        $"{ZLCCoding.Constant.ForDEF}2", forDef2s
                    }
                }
            };
            CSharpCode cSharpCode = FormatManager.Convert<ZLCCode, CSharpCode>(zlcCode);

            return new WindowConfigCode()
            {
                code = cSharpCode.code
            };
        }
    }
}