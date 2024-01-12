using UnityEngine;
namespace ZLCEngine.Inspector
{
    /// <summary>
    /// 可以在面板中选择子类作为实例的特性
    /// 注意：目前没有专门支持序列化，部分需要配合[SerializeReference]使用
    /// </summary>
    public class VirtualSerializeAttribute : PropertyAttribute
    {
    }
}