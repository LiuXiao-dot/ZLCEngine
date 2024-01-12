using System;
using UnityEngine;
namespace ZLCEditor.Inspector
{
    public static class ZLCTempManager
    {
        /// <summary>
        /// todo:支持更多类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ScriptableObject CreateTemp(object value, Type type)
        {
            if (type == typeof(int)) {
                var zlcTempInt = ScriptableObject.CreateInstance<ZLCTempInt>();
                zlcTempInt.value = (int)value;
                return zlcTempInt;
            }
            var zlcTempObject = ScriptableObject.CreateInstance<ZLCTempObject>();
            zlcTempObject.value = value;
            return zlcTempObject;
        }
        
        [Serializable]
        private class ZLCTempObject : ScriptableObject
        {
            [SerializeReference]public object value;
        }
        
        [Serializable]
        private class ZLCTempInt : ScriptableObject
        {
            [SerializeField]public int value;
        }

        public static object GetTempValue(object obj)
        {
            var type = obj.GetType();
            var valueField = type.GetField("value");
            return valueField.GetValue(obj);
        }
    }
}