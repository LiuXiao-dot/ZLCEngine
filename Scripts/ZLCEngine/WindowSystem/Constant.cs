using System.Collections.Generic;
namespace ZLCEngine.WindowSystem
{
    /// <summary>
    ///     常量
    /// </summary>
    public sealed class Constant
    {
        /// <summary>
        ///     各个层级窗口的基础SortingOrder值
        /// </summary>
        public static Dictionary<WindowLayer, int> sortingOrders { get; } = new Dictionary<WindowLayer, int>
        {
            {
                WindowLayer.MAIN, 0
            },
            {
                WindowLayer.CHILD, 10
            },
            {
                WindowLayer.SMALL, 30
            },
            {
                WindowLayer.LOADING, 50
            },
            {
                WindowLayer.MASK, 70
            },
            {
                WindowLayer.TIP, 90
            }
        };
        
        /// <summary>
        /// 同一个windowID的窗口同时存在的数量上限
        /// </summary>
        public const int WindowNum = 100;
        /// <summary>
        /// 不同层级间窗口的ID倍率
        /// </summary>
        public const int LayerRatio = 100000;
    }
}