using System;
using UnityEngine;
using UnityEngine.InputSystem;
using ZLCEngine.Interfaces;
namespace ZLCEngine.InputSystem
{
    /// <summary>
    /// 输入的按键绑定的序列化器
    /// </summary>
    public class InputSerializer : IManager, ILoader
    {
        public InputActionAsset actions;
        private InputListener _listener;

        public void Dispose() 
        {
            var rebinds = actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("rebinds", rebinds);
        }

        public void Init()
        {
            var rebinds = PlayerPrefs.GetString("rebinds");
            if (!string.IsNullOrEmpty(rebinds))
                actions.LoadBindingOverridesFromJson(rebinds);
        }
        public void Load(Action<int, int> onProgress = null)
        {
            IAppLauncher.Get<IResLoader>().LoadAsset<InputActionAsset>(Constant.InputActionAssetName, (success, result) =>
            {
                actions = result;
                onProgress?.Invoke(1, 1);
                _listener = new InputListener(actions);
            });
        }
    }
}