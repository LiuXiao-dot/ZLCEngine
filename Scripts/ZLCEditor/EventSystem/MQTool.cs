using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZLCEditor.FormatSystem;
using ZLCEditor.FormatSystem.Common;
using ZLCEngine.ConfigSystem;
using ZLCEngine.EventSystem.MessageQueue;
using ZLCEngine.Inspector;
using ZLCEngine.Utils;
namespace ZLCEditor.EventSystem
{
    /// <summary>
    /// MQ相关的工具
    /// </summary>
    [Tool("消息队列")]
    public class MQTool : SOSingleton<MQTool>
    {
        public MQConfigSO mqConfigSo => MQConfigSO.Instance;

        public MQTool()
        {
        }
        
        [Button]
        private void GenerateCodes()
        {
            var mqs = mqConfigSo.internalMQS.Concat(mqConfigSo.MainMQS).Concat(mqConfigSo.ChildMQS).OrderBy(t => t.id);
            GenerateMQEnums(mqs);
            var mqConfigs = mqs.Where(t => !t.isInternal);
            foreach (var mq in mqs) {
                GenerateWithMQConfig(mq);
            }
        }

        private static void GenerateMQEnums(IEnumerable<MQConfig> mqs)
        {
            var cSharpCode = FormatManager.Convert<IEnumerable<MQConfig>, CSharpCode>(mqs);
            FileHelper.SaveFile(cSharpCode.code, Path.Combine(Constant.ZLCGenerateURL, $"MQType.cs"));
        }

        private static void GenerateWithMQConfig(MQConfig mqConfig)
        {
            var cSharpCode = FormatManager.Convert<MQConfig, CSharpCode>(mqConfig);
            FileHelper.SaveFile(cSharpCode.code, Path.Combine(Constant.ZLCGenerateURL, $"{mqConfig.name}.cs"));
        }
    }
}