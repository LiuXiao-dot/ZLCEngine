using System;
using UnityEngine;
namespace ZLCEngine.EventSystem.MessageQueue
{
    /// <summary>
    ///     单个消息队列的配置数据
    /// </summary>
    [Serializable]
    public class MQConfig
    {
        /// <summary>
        ///     消息队列的名字
        /// </summary>
        public string name;

        /// <summary>
        ///     消息队列的id
        /// </summary>
        public int id;

        #if UNITY_EDITOR
        /// <summary>
        ///     消息队列的事件
        /// </summary>
        public string[] events;

        /// <summary>
        ///     编辑器模式下的提示信息
        /// </summary>
        public string tip;

        /// <summary>
        ///     为true时，不会生成对应的events枚举类型
        /// </summary>
        [HideInInspector] public bool isInternal;
        #endif
    }
}