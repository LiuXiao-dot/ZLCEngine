using System;
using UnityEngine;
namespace ZLCEditor.Inspector
{
    [Serializable]
    internal class ZLCTempObject : ScriptableObject
    {
        [SerializeReference]public object t;
    }
}