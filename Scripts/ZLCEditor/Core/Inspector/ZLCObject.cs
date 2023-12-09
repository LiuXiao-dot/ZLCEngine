using System;
using UnityEngine;
namespace ZLCEditor.Inspector
{
    [Serializable]
    [CreateAssetMenu]
    public class ZLCObject : ScriptableObject
    {
        [SerializeReference]
        public object t;
    }
}