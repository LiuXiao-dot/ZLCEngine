using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.InputSystem;
using ZLCEditor.EventSystem;
using ZLCEngine.ConfigSystem;
using ZLCEngine.EventSystem.MessageQueue;
using ZLCEngine.Inspector;
using FilePathAttribute = ZLCEngine.ConfigSystem.FilePathAttribute;
namespace ZLCEditor.InputSystem
{
    [Tool("输入系统")]
    public class InputSystemSO : SOSingleton<InputSystemSO>
    {
        [ReadOnly]public InputActionAsset inputActionAsset;

        protected override void OnEnable()
        {
            base.OnEnable();
            Check();
        }

        private void Check()
        {
            // 在Assets目录下查找，如果没有，则复制Presets/InputActions/DefaultInputAction.inputactions到Assets/ZLC_Config_Internal/Configs里
            //inputActionAsset = EditorHelper.CreateAssetIfNotExit<InputActionAsset>(FilePathAttribute.GetInternalPath(ZLCEngine.InputSystem.Constant.InputActionAssetName,FilePathAttribute.PathType.XW));

            if(inputActionAsset != null) return;

            var result = AssetDatabase.FindAssets("t:InputActionAsset", new []{"Assets"});
            if (result.Length == 0) {
                // 创建新的
                AssetDatabase.CopyAsset("Packages\\com.zlc.zlcengine\\Presets\\InputActions\\DefaultInputActions.inputactions", FilePathAttribute.GetInternalPath(ZLCEngine.InputSystem.Constant.InputActionAssetName, FilePathAttribute.PathType.XW));
            } else {
                foreach (var s in result) {
                    var path = AssetDatabase.GUIDToAssetPath(s);
                    var name = Path.GetFileName(path);
                    if (name == ZLCEngine.InputSystem.Constant.InputActionAssetName) {
                        inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// 将InputAction同步到输入事件的枚举中
        /// </summary>
        [Button("同步输入信息")]
        private void SyncToMessage()
        {
            Check();
            var mqConfigSo = MQConfigSO.Instance;
            var internalMqs = mqConfigSo.internalMQS;
            var inputMqConfig = internalMqs[2];
            inputMqConfig.events = inputActionAsset.Select(t => t.name).ToArray();;
            MQTool.Instance.GenerateCodes();
        }
    }
}