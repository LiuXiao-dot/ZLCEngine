using UnityEngine;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
namespace ZLCEngine.EventSystem.MessageQueue
{
    /// <summary>
    ///     MQ的配置信息
    ///     需要根据MQ生成具体的事件的枚举类代码
    /// </summary>
    [Tool("配置/消息队列")]
    [FilePath(FilePathAttribute.PathType.XWEditor, true)]
    public class MQConfigSO : SOSingleton<MQConfigSO>
    {
        public const int SceneMessageID = 1;
        public const int WindowMessageID = 2;

        [BoxGroup("内置消息队列")]
        [ReadOnly]
        public MQConfig[] internalMQS =
        {
            new MQConfig
            {
                name = "SceneMessage",
                id = 1,
                events = new[]
                {
                    "OnSceneOpen", "OnSceneClose"
                },
                tip = "场景打开和关闭的消息",
                isInternal = true
            },
            new MQConfig
            {
                name = "WindowMessage",
                id = 2,
                events = new[]
                {
                    // 窗口资源加载完成
                    "AfterWindowLoaded",
                    // 窗口资源加载完成->窗口进入前
                    "BeforeWindowEnter",
                    // 窗口进入前->窗口进入后
                    "AfterWindowEnter",
                    // 窗口恢复前
                    "BeforeWindowResume",
                    // 窗口恢复
                    "AfterWindowResume",
                    // 窗口暂停前
                    "BeforeWindowPause",
                    // 窗口暂停
                    "AfterWindowPause",
                    // 窗口退出前
                    "BeforeWindowExit",
                    // 窗口退出
                    "AfterWindowExit"
                },
                tip = "窗口相关的消息",
                isInternal = true
            }
        };

        [BoxGroup("主线程消息队列列表")]
        [SerializeField]
        public MQConfig[] MainMQS;

        [BoxGroup("子线程消息队列列表")]
        [SerializeField]
        public MQConfig[] ChildMQS;

        #if UNITY_EDITOR
        [Button("重置默认消息队列")]
        private void ResetInternal()
        {
            internalMQS = new[]
            {
                new MQConfig
                {
                    name = "SceneMessage",
                    id = 1,
                    events = new[]
                    {
                        "OnSceneOpen", "OnSceneClose"
                    },
                    tip = "场景打开和关闭的消息"
                }
            };
        }
        #endif
    }
}