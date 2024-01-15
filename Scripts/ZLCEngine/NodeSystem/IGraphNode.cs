using System.Collections.Generic;
namespace ZLCEngine.NodeSystem
{
    /// <summary>
    /// 图节点
    /// </summary>
    public interface IGraphNode : INode
    {
        /// <summary>
        /// 邻节点
        /// </summary>
        List<IGraphNode> neighbours { get; set; }
    }
}