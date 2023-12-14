using System.IO;
using System.Text;
using Unity.VisualScripting.YamlDotNet.RepresentationModel;
using Unity.VisualScripting.YamlDotNet.Serialization;
using UnityEditor;
using UnityEngine;
using ZLCEngine.Inspector;
namespace UnityYAML
{
    /// <summary>
    ///     Unity中的YAML格式解析器
    /// </summary>
    public class Analyzer
    {
                #region 测试
        public GameObject testGo;
        [Button("测试YAML解析为节点")]
        private void YAMLAnalysisTest()
        {
            if (testGo == null) return;
            StreamReader input = new StreamReader(AssetDatabase.GetAssetPath(testGo), Encoding.UTF8);
            YamlStream yaml = new YamlStream();
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
            string testValue = @"";
            StringReader input = new StringReader(testValue);
            Deserializer deserializer = new DeserializerBuilder()
                .Build();

            GameObject order = deserializer.Deserialize<GameObject>(input);

            if (order != null)
                Debug.Log("反序列化成功");
        }

        [Button("YAML序列化测试")]
        private void YAMLSerializeTest()
        {
            if (testGo == null) return;
            Serializer serializer = new SerializerBuilder().Build();
            string yaml = serializer.Serialize(testGo);
            Debug.Log(yaml);
        }
  #endregion
    }
}