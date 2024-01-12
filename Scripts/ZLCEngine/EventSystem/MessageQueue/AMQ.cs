using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using ZLCEngine.Interfaces;
using ZLCGenerate.Window.Models;
using Object = System.Object;
namespace ZLCEngine.EventSystem.MessageQueue
{

    /// <summary>
    ///     AMessageQueue:消息队列抽象类
    /// </summary>
    public abstract class AMQ : MonoBehaviour, ISubscribee<int>
    {
        /// <summary>
        ///     消息池
        /// </summary>
        protected Queue<Event> queue = new Queue<Event>();
        /// <summary>
        ///     消息订阅者
        /// </summary>
        protected Dictionary<int, List<ISubscriber>> listeners = new Dictionary<int, List<ISubscriber>>();
        /// <summary>
        ///     消息数量
        /// </summary>
        protected int eventCount;
        /// <summary>
        ///     当前消息剩余订阅执行数量
        /// </summary>
        protected int taskCount;
        /// <summary>
        ///     当前执行中的消息
        /// </summary>
        private Event currentEvent;
        

        /// <summary>
        ///     消息队列的唯一ID
        /// </summary>
        public int id
        {
            get;
            internal set;
        }

        /// <inheritdoc cref="ISubscribee{int}" />
        public void Subscribe(ISubscriber subscriber, int operate)
        {
            if (!listeners.ContainsKey(operate)) {
                listeners.Add(operate, new List<ISubscriber>());
            }

            if (listeners.TryGetValue(operate, out List<ISubscriber> operateListeners) &&
                !operateListeners.Contains(subscriber)) {
                operateListeners.Add(subscriber);
            } else {
                Debug.LogWarning($"重复添加监听,listener:{subscriber.GetType()} operate:{operate}");
            }
        }

        /// <inheritdoc cref="ISubscribee{int}" />
        public void Subscribe(ISubscriber subscriber, IEnumerable<int> operates)
        {
            foreach (int operate in operates) {
                Subscribe(subscriber, operate);
            }
        }

        /// <inheritdoc cref="ISubscribee{int}" />
        public void Unsubscribe(ISubscriber subscriber, int operate)
        {
            if (listeners.TryGetValue(operate, out List<ISubscriber> operateListeners) &&
                operateListeners.Contains(subscriber)) {
                operateListeners.Remove(subscriber);
            }
        }

        /// <inheritdoc cref="ISubscribee{int}" />
        public void Unsubscribe(ISubscriber subscriber, IEnumerable<int> operates)
        {
            foreach (int operate in operates) {
                Unsubscribe(subscriber, operate);
            }
        }
        /// <inheritdoc cref="ISubscribee{int}" />
        public virtual void SendEvent(int operate, object args)
        {
            try {
                if (listeners.TryGetValue(operate,out var subscribers) && subscribers.Count > 0) {
                    queue.Enqueue(new Event(this, operate, args, Callback));
                    eventCount++;
                }
            }
            catch (InvalidCastException e) {
                Debug.LogError(e);
            }
        }

        /// <summary>
        ///     执行
        /// </summary>
        protected void Act()
        {
            if (taskCount > 0 || queue.Count == 0 || !queue.TryDequeue(out currentEvent)) return;
            try {
                if (listeners.TryGetValue( currentEvent.operate, out List<ISubscriber> currentListeners)) {
                    int length = currentListeners.Count;
                    taskCount = length;

                    for (int i = length - 1; i >= 0; i--) {
                        if (currentListeners[i] == null) {
                            currentListeners.RemoveAt(i);
                            Callback(currentEvent);
                        }
                    }
                    length = currentListeners.Count;
                    for (int i = length - 1; i >= 0; i--) {
                        currentListeners[i].OnMessage(currentEvent);
                    }
                }
            }
            catch (Exception e) {
                Debug.LogError(e);
                IAppLauncher.Get<IWindowManager>().Open(300100, new TipWindowModel()
                {
                    value = e.StackTrace
                });
            }
        }
#if ZLC_DEBUG
    [SerializeField] private string currentEventName;
    private string _empty = "无";
#endif
        /// <summary>
        ///     消息执行完成后需要调用回调
        /// </summary>
        /// <param name="message"></param>
        public void Callback(Event message)
        {
            if (!message.Equals(currentEvent)) {
                return;
            }
            taskCount--;
            if (taskCount != 0) return;
            eventCount--;
            if (eventCount < 0) {
                IAppLauncher.Get<IWindowManager>().Open(300100, new TipWindowModel()
                {
                    value = $"MQ:{id}的eventCount={eventCount} {currentEvent.ToString()}"
                });
            }
#if ZLC_DEBUG
        currentEventName = _empty;
#endif
        }
    }
}