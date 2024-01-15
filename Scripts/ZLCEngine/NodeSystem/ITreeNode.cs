using System.Collections.Generic;
namespace ZLCEngine.NodeSystem
{
    /// <summary>
    /// 树节点
    /// </summary>
    public interface ITreeNode
    {
        /// <summary>
        /// 父节点
        /// </summary>
        ITreeNode Parent { get; set; }
        
        /// <summary>
        /// 子节点
        /// </summary>
        List<ITreeNode> Children { get; }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="newChild"></param>
        void Add(ITreeNode newChild);
        
        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="newChild"></param>
        bool Remove(ITreeNode newChild);
        
        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool RemoveAt(int index);

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ITreeNode Get(int index);
    }
}