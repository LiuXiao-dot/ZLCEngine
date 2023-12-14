using System;
using System.Reflection;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector
{
    /// <summary>
    ///     对方法等本不可序列化的数据创建UI
    /// </summary>
    public interface IAnySerializableAttributeEditor
    {
        VisualElement CreateGUI(Attribute attribute, MemberInfo memberInfo, object instance);
    }
}