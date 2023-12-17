using System;
using System.Reflection;
using UnityEngine;
namespace ZLCEngine.SerializeTypes
{
    /// <summary>
    ///     可序列化的Type,编辑器中展示为Type和所有继承自Type的类
    /// </summary>
    [Serializable]
    public class SType : ISerializationCallbackReceiver
    {
        public Type realType;
        public string typeName;
        public string assemblyName;

        /// <summary>
        ///     无参构造
        /// </summary>
        public SType()
        {
            realType = null;
        }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="type">实际类型</param>
        public SType(Type type)
        {
            realType = type;
            typeName = realType.FullName;
            assemblyName = realType.Assembly.FullName;
        }

        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(typeName)) return;
            realType = Assembly.Load(assemblyName).GetType(typeName);
        }

        /// <summary>
        ///     将Type转换为SType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static implicit operator SType(Type type)
        {
            return new SType(type);
        }

        /// <summary>
        ///     将SType转换为Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static implicit operator Type(SType type)
        {
            return type.realType;
        }

        public override string ToString()
        {
            return typeName;
        }
    }
}