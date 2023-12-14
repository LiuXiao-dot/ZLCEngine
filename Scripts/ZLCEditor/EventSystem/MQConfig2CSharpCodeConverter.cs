using ZLCEditor.FormatSystem;
using ZLCEditor.FormatSystem.Common;
using ZLCEditor.ZLCCoding;
using ZLCEngine.EventSystem.MessageQueue;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.EventSystem
{
    /// <summary>
    ///     MQConfig到CSharp代码的转换器
    ///     MQConfig生成的是一个枚举类
    /// </summary>
    public class MQConfig2CSharpCodeConverter : IFormatConverter<MQConfig, CSharpCode>
    {
        public CSharpCode Convert(MQConfig from)
        {
            string code = string.Empty;
            string name = from.name;

            ZLCCode zlcCode = new ZLCCode
            {
                code =
                    @"namespace ZLCGenerate
{
    /// <summary>
    /// $TIP$
    /// </summary>
    public enum $ENUMNAME$
    {
#for $ENUMS$ #
        $ENUMS$,
#end    
    }
}",
                kvs = new SDictionary<string, object>
                {
                    {
                        "TIP", from.tip
                    },
                    {
                        "ENUMNAME", name
                    },
                    {
                        "ENUMS", from.events
                    }
                }
            };

            return FormatManager.Convert<ZLCCode, CSharpCode>(zlcCode);
        }
    }
}