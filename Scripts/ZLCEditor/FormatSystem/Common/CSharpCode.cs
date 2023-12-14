using System;
using UnityEngine;
namespace ZLCEditor.FormatSystem.Common
{
    /// <summary>
    ///     C#格式的文本
    /// </summary>
    [Serializable]
    public struct CSharpCode
    {
        [TextArea(5, 100)]
        public string code;
    }
}