#if UNITY_EDITOR
using System;
using UnityEngine;
using ZLCEngine.Inspector;
using ZLCEngine.SerializeTypes;
namespace ZLCEngine.TerrainSystem.Boolean
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [Serializable]
    public class BooleanTerrain : MonoBehaviour
    {
        /// <summary>
        /// K:层级
        /// V:形状列表
        /// </summary>
        [SerializeField]public SDictionary<int,ShapeLayer> shapes;
        [SerializeField][ReadOnly]private MeshFilter _meshFilter;
        [SerializeField][ReadOnly]private MeshRenderer _meshRenderer;
        

        #if UNITY_EDITOR
        private void Reset()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        #endif
    }
}
#endif