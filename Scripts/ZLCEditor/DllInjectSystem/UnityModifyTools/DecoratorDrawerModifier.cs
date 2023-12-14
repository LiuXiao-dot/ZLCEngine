using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
namespace ZLCEditor.DllInjectSystem.UnityModifyTools
{
    [Tool("Unity修改器/DecoratorDrawerModifier")]
    public class DecoratorDrawerModifier
    {

        private const string source = "C:\\Users\\安然\\Desktop\\UnityDllModify\\UnityEditor.CoreModule.dll"; // 源地址
        private const string modified = "C:\\Users\\安然\\Desktop\\UnityDllModify\\Generator\\UnityEditor.CoreModule.dll"; // 修改的dll地址
        private const string target = "C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.0f1c1\\Editor\\Data\\Managed\\UnityEngine\\UnityEditor.CoreModule.dll"; // 目标地址
        /*
         todo:需要修改Unity license相关的文件，不然修改后无法正常加载打开Unity
         [Button]
        public void Excute()
        {
            var path = "C:\\Users\\安然\\Desktop\\UnityDllModify\\UnityEditor.CoreModule.dll";
            var module = ModuleDefinitionExtension.ReadModule(path);
            var typeDefinition = module.GetType("UnityEditor.DecoratorDrawer");
            typeDefinition.AddField("parent", FieldAttributes.Public, typeof(VisualElement));
            module.Write(Path.Combine(Path.GetDirectoryName(path),"Generator",Path.GetFileName(path)));
        }
        */

        /// <summary>
        ///     为UnityEditor.CoreModule.dll添加ZLCEditor.Core为友元程序集
        /// </summary>
        [Button]
        public void Excute()
        {
            string path = "C:\\Users\\安然\\Desktop\\UnityDllModify\\UnityEditor.CoreModule.dll";
            ModuleDefinition module = ModuleDefinitionExtension.ReadModule(path);
            module.Assembly.AddVisibleToAssembly("ZLCEditor.Core");
            ChangeMethodsToVirtualAndPublic(module, "UnityEditor.UIElements.PropertyField", new string[]
            {
                "Reset", "ResetDecoratorDrawers",
                "ComputeNestingLevel", "RegisterPropertyChangesOnCustomDrawerElement", "CreateOrUpdateFieldFromProperty", "PropagateNestingLevel",
                "CreatePropertyIMGUIContainer"
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
            ChangeTypeToPublic(module, "UnityEditor.PropertyHandler");
            ChangeFieldsToPublic(module, "UnityEditor.UIElements.PropertyField", new string[]
            {
                "m_DecoratorDrawersContainer", "m_DrawNestingLevel", "m_SerializedObject", "m_SerializedProperty", "m_SerializedPropertyReferenceTypeName", "m_ChildField", "m_imguiChildField"
            });
            module.Write(Path.Combine(Path.GetDirectoryName(path), "Generator", Path.GetFileName(path)));
        }

        private bool IsSameParameters(Collection<ParameterDefinition> a, Collection<ParameterDefinition> b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Count != b.Count) return false;
            var length = a.Count;
            for (int i = 0; i < length; i++) {
                if (a[i].ParameterType.FullName != b[i].ParameterType.FullName) return false;
            }
            return true;
        }

        private void ChangeMethodsToVirtualAndPublic(ModuleDefinition moduleDefinition, string typeName, string[] methodNames, Collection<ParameterDefinition>[] parameterDefinitions = null)
        {
            var length = methodNames.Length;
            for (int i = 0; i < length; i++) {
                ChangeMethodToVirtualAndPublic(moduleDefinition, typeName, methodNames[i], parameterDefinitions[i]);
            }
        }

        private void ChangeMethodToVirtualAndPublic(ModuleDefinition moduleDefinition, string typeName, string methodName, Collection<ParameterDefinition> parameterDefinitions = null)
        {
            TypeDefinition type = moduleDefinition.GetType(typeName);
            IEnumerable<MethodDefinition> methods = type.Methods.Where(t => t.Name == methodName && IsSameParameters(t.Parameters, parameterDefinitions));
            if (methods.Any()) {
                MethodDefinition method = methods.First();
                method.ChangeMethodAttributes(MethodAttributes.Virtual | MethodAttributes.Public);
            } else {
                Debug.LogError($"未找到方法{typeName}.{methodName}");
            }
        }

        private void ChangeTypeToPublic(ModuleDefinition moduleDefinition, string typeName)
        {
            TypeDefinition type = moduleDefinition.GetType(typeName);
            type.Attributes = type.Attributes & ~TypeAttributes.NotPublic | TypeAttributes.Public;
        }

        private void ChangeFieldsToPublic(ModuleDefinition moduleDefinition, string typeName, string[] fieldNames)
        {
            foreach (var filedName in fieldNames) {
                ChangeFieldToPublic(moduleDefinition, typeName, filedName);
            }
        }

        private void ChangeFieldToPublic(ModuleDefinition moduleDefinition, string typeName, string fieldName)
        {
            TypeDefinition type = moduleDefinition.GetType(typeName);
            IEnumerable<FieldDefinition> fields = type.Fields.Where(t => t.Name == fieldName);
            if (fields.Any()) {
                FieldDefinition field = fields.First();
                field.ChangeFieldAttributes(FieldAttributes.Public);
            } else {
                Debug.LogError($"未找到字段{typeName}.{fieldName}");
            }
        }
    }
}