using System;
using UnityEngine;
namespace ZLCEngine.Interfaces
{

    /// <summary>
    ///     窗口管理器
    /// </summary>
    public interface IWindowManager : IManager, ILoader
    {
        /// <summary>
        ///     打开窗口
        /// </summary>
        /// <param name="id">窗口ID</param>
        /// <param name="windowModel">窗口的数据</param>
        /// <returns>窗口实例的id</returns>
        int Open(int id, IWindowModel windowModel = null);
        /// <summary>
        ///     关闭窗口
        /// </summary>
        /// <param name="instanceID">窗口ID</param>
        void Close(int instanceID);
    }

    /// <summary>
    ///     窗口控制器
    /// </summary>
    public interface IWindowCtl
    {
        void SetInstanceID(int instanceID);
        void SetModel(IWindowModel model);
        IWindowModel GetModel();
        void SetView(IWindowView view);
        IWindowView GetView();
        void Open();
        void Pause();
        void Resume();
        void Close();
        int GetID();
        int GetInstanceID();
    }

    /// <summary>
    ///     窗口视图
    /// </summary>
    public interface IWindowView
    {
        void Close();
        int GetID();
        int GetWindowLayer();
    }

    /// <summary>
    ///     窗口数据
    /// 目前只有一个标记的意义，没有实际作用
    /// </summary>
    public interface IWindowModel
    {
    }

    /// <summary>
    ///     加载窗口数据
    /// </summary>
    [Serializable]
    public class LoadingWindowModel : IWindowModel
    {
        /// <summary>
        ///     加载完成的回调
        /// </summary>
        public Action loadFinished;
        /// <summary>
        ///     当前加载的进度(默认100倍)
        /// </summary>
        public int progress;
        
        /// <summary>
        /// 加载进度变化时调用(回调中直接从Model里拿数据，保持所有数据都直接从model中拿最新的)
        /// </summary>
        public Action onValueChanged;

        /// <summary>
        /// progress的倍率
        /// </summary>
        public int ratio;

        /// <summary>
        /// 虚假进度条的速度
        /// </summary>
        public int fakeSpeed;

        /// <summary>
        /// 虚假进度
        /// </summary>
        public int fakeProgress;

        public LoadingWindowModel(int progress, Action loadFinished, int ratio = 100)
        {
            this.progress = progress;
            this.loadFinished = loadFinished;
            this.ratio = ratio;
            this.fakeSpeed = ratio / 60;
        }

        public void AddValueChangedEvent(Action onValueChanged)
        {
            this.onValueChanged += onValueChanged;
        }
        
        public void AddLoadFinishedEvent(Action onLoadFinished)
        {
            this.loadFinished += onLoadFinished;
        }
        
        public void SetValue(int progress)
        {
            this.progress = progress;
            onValueChanged?.Invoke();
            if (progress > ratio) {
                Debug.LogError($"加载进度异常 progress:{progress} ratio:{ratio}");
            }
        }
    }
}