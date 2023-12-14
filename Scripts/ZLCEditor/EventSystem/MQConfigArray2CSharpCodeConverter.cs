using System.Collections.Generic;
using System.Linq;
using ZLCEditor.FormatSystem;
using ZLCEditor.FormatSystem.Common;
using ZLCEditor.ZLCCoding;
using ZLCEngine.EventSystem.MessageQueue;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.EventSystem
{
    /// <summary>
    ///     根据MQConfig数组包含的消息生成消息队列枚举
    /// </summary>
    public class MQConfigArray2CSharpCodeConverter : IFormatConverter<IEnumerable<MQConfig>, CSharpCode>
    {

        public CSharpCode Convert(IEnumerable<MQConfig> from)
        {
            string code =
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
}";
            MQConfig[] mqArray = from.ToArray();
            string[] @params = new string[mqArray.Count()];
            for (int i = 0; i < mqArray.Length; i++) {
                @params[i] = $"{mqArray[i].name} = {mqArray[i].id}";
            }
            ZLCCode zlcCode = new ZLCCode
            {
                code = code,
                kvs = new SDictionary<string, object>
                {
                    {
                        "TIP", "消息队列"
                    },
                    {
                        "ENUMNAME", "MQType"
                    },
                    {
                        "ENUMS", @params
                    }
                }
            };

            return FormatManager.Convert<ZLCCode, CSharpCode>(zlcCode);
        }
    }
}