using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using ZLCEngine.Inspector;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonDrawer : PropertyDrawer, IAnySerializableAttributeEditor
    {
        public VisualElement CreateGUI(MemberInfo memberInfo, object instance)
        {
            if (memberInfo is MethodInfo methodInfo) {
                var root = new VisualElement();
                var parameters = methodInfo.GetParameters();
                if (parameters.Length > 0) {
                    // 有参数，添加折叠栏用于设置参数
                    //var foldout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Path.Combine(Constant.UXMLPath, "Foldout.uxml"));
                    var foldoutUI = new Foldout();
                    var parameterUis = new VisualElement[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++) {
                        parameterUis[i] = PropertyFieldWrap.CreateFieldFromType(parameters[i].ParameterType);
                        foldoutUI.Add(parameterUis[i]);
                    }

                    var btn = new Button(() => methodInfo?.Invoke(instance, 
                        parameterUis.Select(t=>PropertyFieldWrap.GetFieldValue(t)).ToArray()))
                    {
                        text = methodInfo.Name
                    };
                    root.Add(foldoutUI);
                    root.Add(btn);
                    return root;
                }
                
                root.Add(new Button(()=>methodInfo?.Invoke(instance, null))
                {
                    text = methodInfo.Name
                });
                return root;
            }
            return null;
        }
    }
}