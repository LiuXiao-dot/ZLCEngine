using System.Collections.Generic;
using UnityEngine;
namespace ZLCEngine.Utils
{
    /// <summary>
    ///     Transform帮助类
    /// </summary>
    public sealed class TransformHelper
    {
        /// <summary>
        ///     重置Transform的position,scale,rotation
        /// </summary>
        public static void Reset(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        ///     遍历全部transform
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static IEnumerable<Transform> ForEach(Transform transform)
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++) {
                Transform child = transform.GetChild(i);
                yield return child;
                IEnumerable<Transform> temp = ForEach(child);
                foreach (Transform childTemp in temp) {
                    yield return childTemp;
                }
            }
        }
    }
}