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
    }
}