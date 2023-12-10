using System;
using System.Collections.Generic;
using UnityEngine;
namespace ZLCEngine.SerializeTypes
{


    /// <summary>
    ///     支持序列化不同对象类型的list
    /// </summary>
    [Serializable]
    public class SList : List<object>, ISerializationCallbackReceiver
    {
        /// <summary>
        ///     每个数据的类型
        /// </summary>
        public SType[] types;
        /// <summary>
        ///     每个数据的实际数据
        /// </summary>
        public string[] temps;

        /// <inheritdoc />
        public void OnBeforeSerialize()
        {
            int count = Count;
            temps = new string[count];
            types = new SType[count];
            for (int i = 0; i < count; i++) {
                temps[i] = JsonUtility.ToJson(this[i]);
                types[i] = this[i].GetType();
            }
        }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            if (temps == null) return;
            int count = temps.Length;
            for (int i = 0; i < count; i++) {
                Add(JsonUtility.FromJson(temps[i], types[i]));
            }

            temps = null;
            types = null;
        }
    }
}