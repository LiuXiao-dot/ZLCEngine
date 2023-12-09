using System;
using UnityEngine;
namespace ZLCEngine.Inspector
{
    /// <summary>
    /// 资产列表过滤
    /// </summary>
    public class AssetListAttribute : PropertyAttribute
    {
        public string CustomFilterMethod { get; set; }
    }
}