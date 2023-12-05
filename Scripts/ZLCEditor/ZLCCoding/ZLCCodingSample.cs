/*using Sirenix.OdinInspector;
using UnityEngine;
using ZLCEditor.FormatSystem.Common;
using ZLCEngine.SerializeSystem;
namespace ZLCEditor.ZLCCoding
{
    public class ZLCCodingSample : MonoBehaviour
    {
        public ZLCCode zlcCode;
        public CSharpCode cSharpCode;
        [Button]
        public void Excute()
        {
            zlcCode.code =
@"public void $NAME$(){
#for $VALUES$ #
    Debug.Log($VALUES$);
#end
#if $OPTION$ #
    Debug.Log($TRUE$);
#end
}";
            zlcCode.kvs = new SDictionary<string, object>()
            {
                {"NAME", "name"},
                {"VALUES", new string[]{"str0", "str1"}},
                {"OPTION", true},
                {"TRUE", "选择语句"}
            };
            var converter = new ZLC2CSharpConverter();
            cSharpCode = converter.Convert(zlcCode);
        }
    }
}*/