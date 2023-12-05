using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using ZLCEngine.ConfigSystem;
namespace ZLCEditor.Tool
{
    /// <summary>
    /// 示例工具
    /// </summary>
    [Tool("测试")]
    public class SamplerTool
    {
        private interface ISampler
        {
        }
        
        private class Sampler : ISampler
        {
            
        }
        
        /// <summary>
        /// 测试Type.GetInterface方法
        /// </summary>
        [Button]
        private void TestGetInterface()
        {
            var type = typeof(Sampler);
            var @interface = typeof(ISampler); 
            var i0 = type.GetInterface(@interface.Name);
            Assert.IsNotNull(i0);
            var i1 = type.GetInterface(@interface.FullName);
            Assert.IsNotNull(i1);
            var i2 = type.GetInterface(@interface.AssemblyQualifiedName);
            Assert.IsNotNull(i2);
        }
    }
}