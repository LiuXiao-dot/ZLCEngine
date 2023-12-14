using UnityEngine;
namespace ZLCEditor.FormatSystem.Persets
{
    /// <summary>
    ///     Json格式的文本转换为 Prefab的转换器
    /// </summary>
    public class Json2PrefabConverter : IFormatConverter<string, GameObject>
    {
        public GameObject Convert(string from)
        {
            return null;
        }
    }
}