using System;
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
        void Open(int id, IWindowModel windowModel = null);
        /// <summary>
        ///     关闭窗口
        /// </summary>
        /// <param name="ID">窗口ID</param>
        void Close(int ID);
    }

    /// <summary>
    ///     窗口控制器
    /// </summary>
    public interface IWindowCtl
    {
        void SetModel(IWindowModel model);
        IWindowModel GetWindowModel();
        void SetView(IWindowView view);
        IWindowView GetView();
        void Open();
        void Pause();
        void Resume();
        void Close();
        int GetID();
    }

    /// <summary>
    ///     窗口视图
    /// </summary>
    public interface IWindowView
    {
        void Close();
        int GetID();
    }

    /// <summary>
    ///     窗口数据
    /// </summary>
    public interface IWindowModel
    {
    }

    /// <summary>
    ///     加载窗口数据
    /// </summary>
    public class LoadingWindowModel : IWindowModel
    {

        /// <summary>
        ///     加载失败的回调
        /// </summary>
        public Action loadFailed;

        /// <summary>
        ///     加载完成的回调
        /// </summary>
        public Action loadFinished;
        /// <summary>
        ///     当前加载的进度
        /// </summary>
        public float progress;

        public LoadingWindowModel(int progress, Action loadFinished, Action loadFailed = null)
        {
            this.progress = progress;
            this.loadFinished = loadFinished;
            this.loadFailed = loadFailed;
        }
    }
}