using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLCEngine.Interfaces;
using Object = UnityEngine.Object;
namespace ZLCEngine.EventSystem.MessageQueue
{
    /// <summary>
    /// MessageQueue管理器
    /// 在使用消息队列时都通过该管理器来接收、监听、取消监听消息
    /// </summary>
    public sealed class MQManager : IManager
    {
        public static MQManager Instance
        {
            get {
                if (instance == null) {
                    instance = new MQManager();
                    instance._mqs = new Dictionary<int, AMQ>();
                }
                return instance;
            }
        }

        private static MQManager instance;

        /// <summary>
        /// key:AMQ的id
        /// value:AMQ
        /// </summary>
        private Dictionary<int, AMQ> _mqs;

        /// <summary>
        /// 在创建MQ时添加一个MQ
        /// </summary>
        internal void AddMQ(int id, AMQ amq)
        {
            if (_mqs.ContainsKey(id)) {
                Debug.LogError($"MQManager已包含id为{id}的MessageQueue:{_mqs[id].GetType().FullName}");
            } else {
                _mqs.Add(id, amq);
            }
        }

        /// <summary>
        /// 在销毁一个MQ时移除掉
        /// </summary>
        internal void RemoveMQ(int id)
        {
            _mqs.Remove(id);
        }

        private static bool Check(int id)
        {
            if (Instance._mqs.ContainsKey(id)) return true;
            Debug.LogError($"无id为{id}的MQ");
            return false;
        }
        
        public static void SendEvent<T>(int id, T operate, object args) where T : Enum
        {
            if (Check(id)) {
                Instance._mqs[id].SendEvent(Convert.ToInt32(operate), args);
            } 
        }

        /// <summary>
        /// 订阅
        /// </summary>
        public static void Subscribe<T>(int id, ISubscriber<int> subscriber,params T[] operates) where T : Enum
        {
            if (Check(id)) {
                Instance._mqs[id].Subscribe(subscriber, operates.Select(t => Convert.ToInt32(t)));
            }
        }
        
        public static void Subscribe(int id, ISubscriber<int> subscriber,params int[] operates)
        {
            if (Check(id)) {
                Instance._mqs[id].Subscribe(subscriber, operates);
            }
        }
        
        /// <summary>
        /// 取消订阅
        /// </summary>
        public static void Unsubscribe<T>(int id, ISubscriber<int> subscriber,params T[] operates) where T : Enum
        {
            if (Check(id)) {
                Instance._mqs[id].Unsubscribe(subscriber, operates.Select(t => Convert.ToInt32(t)));
            }
        }
        
        public static void Unsubscribe(int id, ISubscriber<int> subscriber,params int[] operates)
        {
            if (Check(id)) {
                Instance._mqs[id].Unsubscribe(subscriber, operates);
            }
        }

        ~MQManager()
        {
            Dispose();
        }
        public void Dispose()
        {
            
        }
        
        /// <summary>
        /// 初始化全部消息队列
        /// </summary>
        public void Init()
        {
            var mQGameObject = new GameObject("MQs");
            var parent = mQGameObject.transform;
            var mqConfigSO = MQConfigSO.Instance;
            void CreateMainMQ(MQConfig[] mqs)
            {
                foreach (var mqConfig in mqs) {
                    var mq = new GameObject(mqConfig.name, typeof(MainThreadMQ));
                    var mqComponent = mq.GetComponent<MainThreadMQ>();
                    mqComponent.id = mqConfig.id;
                    AddMQ(mqConfig.id, mqComponent);
                    mq.transform.SetParent(parent);
                }                
            }
            void CreateChildMQ(MQConfig[] mqs)
            {
                foreach (var mqConfig in mqs) {
                    var mq = new GameObject(mqConfig.name, typeof(ChildThreadMQ));
                    var mqComponent = mq.GetComponent<ChildThreadMQ>();
                    mqComponent.id = mqConfig.id;
                    AddMQ(mqConfig.id, mqComponent);
                    mq.transform.SetParent(parent);
                }
            }
            // 内置的暂时都使用主线程的消息队列 
            CreateMainMQ(mqConfigSO.internalMQS);
            CreateMainMQ(mqConfigSO.MainMQS);
            CreateChildMQ(mqConfigSO.ChildMQS);
            Object.DontDestroyOnLoad(mQGameObject);
        }
    }
}