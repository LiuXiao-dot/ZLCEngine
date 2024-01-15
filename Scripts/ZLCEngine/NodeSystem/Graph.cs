using System;
using System.Collections.Generic;
namespace ZLCEngine.NodeSystem
{
    /// <summary>
    /// 图
    /// 1.不允许有相同的节点存在
    /// </summary>
    [Serializable]
    public class Graph
    {
        /// <summary>
        /// 图节点
        /// </summary>
        public HashSet<IGraphNode> nodes;
    }
}