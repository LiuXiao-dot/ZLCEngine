using ZLCEngine.Interfaces;
namespace ZLCEngine.WindowSystem
{
    /// <summary>
    /// 窗口配置接口
    /// </summary>
    public interface IWindowConfig
    {
        /// <summary>
        /// 需要实现该接口，并为Instance赋值
        /// </summary>
        public static IWindowConfig Instance
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// 根据窗口ID创建窗口Ctl
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IWindowCtl CreateWindowCtl(int id);
        /// <summary>
        /// 根据窗口id获取窗口资源路径
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetWindowPath(int id);
    }
}