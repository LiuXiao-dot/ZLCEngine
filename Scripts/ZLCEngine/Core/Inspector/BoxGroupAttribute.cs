using UnityEngine;
namespace ZLCEngine.Inspector
{
    public class BoxGroupAttribute : PropertyAttribute
    {
        public string groupName;
        public BoxGroupAttribute(string groupName)
        {
            this.groupName = groupName;
        }
    }
}