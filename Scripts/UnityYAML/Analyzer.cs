using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using Unity.VisualScripting.YamlDotNet.RepresentationModel;
using Unity.VisualScripting.YamlDotNet.Serialization;
using UnityEditor;
using UnityEngine;
namespace UnityYAML
{
    /// <summary>
    /// Unity中的YAML格式解析器
    /// </summary>
    public class Analyzer
    {
                #region 测试
        public GameObject testGo;
        [Button("测试YAML解析为节点")]
        private void YAMLAnalysisTest()
        {
            if (testGo == null) return;
            var input = new StreamReader(AssetDatabase.GetAssetPath(testGo), Encoding.UTF8);
            var yaml = new YamlStream();
            yaml.Load(input);
            for (int i = 0; i < yaml.Documents.Count; i++) {
                string str = "";
                str = str + (YamlMappingNode)yaml.Documents[i].RootNode + "\n";
                Debug.Log(str);
                foreach (YamlNode yamlNode in yaml.Documents[i].AllNodes) {
                    Debug.Log(yamlNode.ToString());
                }
            }
        }

        [Button("测试YAML反序列化")]
        private void YAMLDeserializeTest()
        {
            var testValue = @"";
            var input = new StringReader(testValue);
            var deserializer = new DeserializerBuilder()
                .Build();

            var order = deserializer.Deserialize<GameObject>(input);

            if (order != null)
                Debug.Log("反序列化成功");
        }

        [Button("YAML序列化测试")]
        private void YAMLSerializeTest()
        {
            if (testGo == null) return;
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(testGo);
            Debug.Log(yaml);
        }
  #endregion
    }
}