using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
namespace ZLCEngine.SerializeTypes
{
    /// <summary>
    /// 可序列化的Type,编辑器中展示为Type和所有继承自Type的类
    /// </summary>
    [Serializable]
    public class SType : ISerializationCallbackReceiver
    {
        [ShowInInspector]
        public Type realType;
        
        [HideInInspector]
        [SerializeField]
        private string typeName;
        /// <summary>
        /// 类型的完整名称
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private string assemblyName;

        /// <summary>
        /// 无参构造
        /// </summary>
        public SType()
        {
            assemblyName = "";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">实际类型</param>
        public SType(Type type)
        {
            this.realType = type;
        }

        /// <summary>
        /// 将Type转换为SType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static implicit operator SType(Type type)
        {
            return new SType(type);
        }

        /// <summary>
        /// 将SType转换为Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static implicit operator Type(SType type)
        {
            return type.realType;
        }
        
        public void OnBeforeSerialize()
        {
            if (realType == null) return;
            typeName = realType.FullName;
            assemblyName = realType.Assembly.FullName;
        }
        public void OnAfterDeserialize()
        {
            if(assemblyName == string.Empty || typeName == string.Empty) return;
            realType = Assembly.Load(assemblyName).GetType(typeName);
            assemblyName = string.Empty;
            typeName = string.Empty;
        }
    }

    [Serializable]
    public class STypeArray
    {
        public SType[] types;
    }
}