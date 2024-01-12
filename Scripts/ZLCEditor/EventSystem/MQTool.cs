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
    ///     MQ相关的工具
    /// </summary>
    [Tool("消息队列")]
    public class MQTool : SOSingleton<MQTool>
    {

        public MQConfigSO mqConfigSo
        {
            get {
                return MQConfigSO.Instance;
            }
        }

        [Button]
        public void GenerateCodes()
        {
            IEnumerable<MQConfig> mqs = mqConfigSo.MainMQS;
            if (mqConfigSo.MainMQS != null) {
                mqs = mqConfigSo.internalMQS.Concat(mqConfigSo.MainMQS);
            }
            if (mqConfigSo.ChildMQS != null) {
                mqs = mqs.Concat(mqConfigSo.ChildMQS);
            }
            mqs  = mqs.OrderBy(t => t.id);
            GenerateMQEnums(mqs);
            IEnumerable<MQConfig> mqConfigs = mqs.Where(t => !t.isInternal);
            foreach (MQConfig mq in mqs) {
                GenerateWithMQConfig(mq);
            }
        }

        private static void GenerateMQEnums(IEnumerable<MQConfig> mqs)
        {
            CSharpCode cSharpCode = FormatManager.Convert<IEnumerable<MQConfig>, CSharpCode>(mqs);
            FileHelper.SaveFile(cSharpCode.code, Path.Combine(Constant.ZLCGenerateURL, "MQType.cs"));
        }

        private static void GenerateWithMQConfig(MQConfig mqConfig)
        {
            CSharpCode cSharpCode = FormatManager.Convert<MQConfig, CSharpCode>(mqConfig);
            FileHelper.SaveFile(cSharpCode.code, Path.Combine(Constant.ZLCGenerateURL, $"{mqConfig.name}.cs"));
        }
    }
}