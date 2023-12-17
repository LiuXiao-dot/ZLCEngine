using System.IO;
using Mono.Cecil;
using Mono.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
namespace ZLCEditor.DllInjectSystem.UnityModifyTools
{
    [Tool("Unity修改器/UnityEditor.CoreModule")]
    public class UnityEditorCoreModifier
    {
        private const string source = "C:\\Users\\安然\\Desktop\\UnityDllModify\\UnityEditor.CoreModule.dll"; // 源地址
        private const string modified = "C:\\Users\\安然\\Desktop\\UnityDllModify\\Generator\\UnityEditor.CoreModule.dll"; // 修改的dll地址
        private const string target = "C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.14f1c1\\Editor\\Data\\Managed\\UnityEngine\\UnityEditor.CoreModule.dll"; // 目标地址
        /// <summary>
        ///     为UnityEditor.CoreModule.dll添加ZLCEditor.Core为友元程序集
        /// </summary>
        [Button]
        public void Excute()
        {
            string path = source;
            ModuleDefinition module = ModuleDefinitionExtension.ReadModule(path);
            module.Assembly.AddVisibleToAssembly("ZLCEditor.Core");
            module.ChangeMethodsToVirtualAndPublic("UnityEditor.UIElements.PropertyField", new string[]
            {
                "Reset", "ResetDecoratorDrawers", "ComputeNestingLevel", "RegisterPropertyChangesOnCustomDrawerElement", "CreateOrUpdateFieldFromProperty", "PropagateNestingLevel", "CreatePropertyIMGUIContainer"
            }, new Collection<ParameterDefinition>[]
            {
                new Collection<ParameterDefinition>()
                {
                    new ParameterDefinition(module.ImportReference(typeof(SerializedProperty)))
                },
                new Collection<ParameterDefinition>()
                {
                    new ParameterDefinition(module.ImportReference(typeof(PropertyHandler)))
                },
                new Collection<ParameterDefinition>(), new Collection<ParameterDefinition>()
                {
                    new ParameterDefinition(module.ImportReference(typeof(VisualElement)))
                },
                new Collection<ParameterDefinition>()
                {
                    new ParameterDefinition(module.ImportReference(typeof(SerializedProperty))),
                    new ParameterDefinition(module.ImportReference(typeof(object)))
                },
                new Collection<ParameterDefinition>()
                {
                    new ParameterDefinition(module.ImportReference(typeof(VisualElement)))
                },
                new Collection<ParameterDefinition>()
            });
            module.ChangeTypeToPublic("UnityEditor.PropertyHandler");
            module.ChangeFieldsToPublic("UnityEditor.UIElements.PropertyField", new string[]
            {
                "m_DecoratorDrawersContainer", "m_DrawNestingLevel", "m_SerializedObject", "m_SerializedProperty", "m_SerializedPropertyReferenceTypeName", "m_ChildField", "m_imguiChildField"
            });
            module.Write(modified);
            module.Write(target);
        }

    }
}