using Unity.Collections;
using UnityEngine;
using ZLCEngine.Interfaces;
namespace ZLCEngine.WindowSystem
{
    /// <summary>
    /// 窗口视图层
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [DisallowMultipleComponent]
    public abstract class AWindowView : MonoBehaviour, IWindowView
    {
        [ReadOnly]
        [SerializeField]
        internal int ID;

        /// <summary>
        /// 窗口层级
        /// </summary>
        public WindowLayer windowLayer;
        
        /// <inheritdoc/>
        public virtual void Close()
        {
            GameObject.Destroy(gameObject);
        }

        public int GetID()
        {
            return ID;
        }
    }
}