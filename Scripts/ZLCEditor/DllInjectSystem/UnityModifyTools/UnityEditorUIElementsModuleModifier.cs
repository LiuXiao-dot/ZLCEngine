using Mono.Cecil;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
namespace ZLCEditor.DllInjectSystem.UnityModifyTools
{
    [Tool("Unity修改器/UnityEditor.UIElementsModule")]
    public class UnityEditorUIElementsModuleModifier
    {
        private const string source = "C:\\Users\\安然\\Desktop\\UnityDllModify\\UnityEditor.UIElementsModule.dll"; // 源地址
        private const string modified = "C:\\Users\\安然\\Desktop\\UnityDllModify\\Generator\\UnityEditor.UIElementsModule.dll"; // 修改的dll地址
        private const string target = "C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.14f1c1\\Editor\\Data\\Managed\\UnityEngine\\UnityEditor.UIElementsModule.dll"; // 目标地址
        
        [Button]
        public void Excute()
        {
            ModuleDefinition module = ModuleDefinitionExtension.ReadModule(source);
            module.Assembly.AddVisibleToAssembly("ZLCEditor.Core");
            module.Write(modified);
            module.Write(target);
        }
    }
}