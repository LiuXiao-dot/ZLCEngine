using System.Collections.Generic;
using UnityEngine.InputSystem;
using ZLCEngine.CacheSystem;
using ZLCEngine.EventSystem.MessageQueue;
using System;
namespace ZLCEngine.InputSystem
{
    /// <summary>
    /// 输入监听器
    /// </summary>
    public class InputListener
    {
        private InputActionAsset actions;

        private Dictionary<Guid, int> messages;

        /// <summary>
        /// 输入上下文池
        /// </summary>
        private ObjectPool<InputContext> _pool;

        internal InputListener(InputActionAsset actions)
        {
            _pool = new ObjectPool<InputContext>(() => new InputContext());
            messages = new Dictionary<Guid, int>();
            var index = 0;
            foreach (var action in actions) {
                messages.Add(action.id, index++);
                RegisterAction(action);
            }
        }

        private void RegisterAction(InputAction action)
        {
            action.started += SendEvent;
            action.performed += SendEvent;
            action.canceled += SendEvent;
        }

        private void SendEvent(InputAction.CallbackContext context)
        {
            var value = _pool.Get();
            value.context = context;
            MQManager.SendEvent(Constant.MQId, messages[context.action.id], value);
        }
    }
}