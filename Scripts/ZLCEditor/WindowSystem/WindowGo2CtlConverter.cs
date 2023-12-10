using UnityEngine;
using ZLCEditor.FormatSystem;
using ZLCEditor.FormatSystem.Common;
using ZLCEditor.ZLCCoding;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.WindowSystem
{
    public class WindowCtlCode
    {
        public string code;
    }
    public class WindowGo2CtlConverter : IFormatConverter<GameObject,WindowCtlCode>
    {
        private static string defaultCode =
            $@"using ZLCEngine.WindowSystem;
public class ${ZLCCoding.Constant.ClassName}$ : AWindowCtl
{{
    protected override void DoOpen()
    {{

    }}

    protected override void DoPause()
    {{

    }}

    protected override void DoResume()
    {{

    }}

    protected override void DoClose()
    {{

    }}
}}";
        
        public WindowCtlCode Convert(GameObject from)
        {
            var zlcCode = new ZLCCode()
            {
                code = defaultCode,
                kvs = new SDictionary<string, object>()
                {
                    {ZLCCoding.Constant.ClassName,$"{from.name}Ctl"}
                }
            };
            var cSharpCode = FormatManager.Convert<ZLCCode, CSharpCode>(zlcCode);

            return new WindowCtlCode()
            {
                code = cSharpCode.code
            };
        }
    }
}