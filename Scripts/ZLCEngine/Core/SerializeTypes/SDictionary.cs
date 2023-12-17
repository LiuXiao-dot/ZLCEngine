using System;
using System.Collections.Generic;
using UnityEngine;
using ZLCEngine.Extensions;
namespace ZLCEngine.SerializeTypes
{
    /// <summary>
    ///     可序列化的字典
    /// </summary>
    /// <typeparam name="K">键</typeparam>
    /// <typeparam name="V">值</typeparam>
    [Serializable]
    public class SDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct KV
        {
            public K key;
            public V value;
        }
        
        [SerializeField] private KV[] _cache;

        /// <inheritdoc />
        public void OnBeforeSerialize()
        {
            if (Count == 0) return;

            _cache = new KV[Count];
            int index = 0;
            foreach (KeyValuePair<K, V> kv in this) {
                _cache[index] = new KV()
                {
                    key = kv.Key,
                    value = kv.Value
                };
                index++;
            }
        }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            Clear();
            if (_cache.IsEmptyOrNull()) return;
            int count = _cache.Length;
            for (int i = 0; i < count; i++) {
                Add(_cache[i].key, _cache[i].value);
            }
            _cache = null;
        }
    }
}