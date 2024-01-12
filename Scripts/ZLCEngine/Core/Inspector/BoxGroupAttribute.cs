using System;
using UnityEngine;
namespace ZLCEngine.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BoxGroupAttribute : PropertyAttribute
    {
        public string groupName;
        public BoxGroupAttribute(string groupName)
        {
            this.groupName = groupName;
        }
    }
}