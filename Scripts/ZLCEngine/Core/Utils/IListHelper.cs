using System;
using System.Collections.Generic;
using UnityEngine;
namespace ZLCEngine.Utils
{
    /// <summary>
    ///     IList的帮助类
    /// </summary>
    public sealed class IListHelper
    {
        /// <summary>
        ///     移除全部空值
        /// </summary>
        /// <returns>true:移除成功 false:无空值可移除</returns>
        public static bool RemoveNulls<T>(IList<T> list)
        {
            try {
                if (list == null) return false;
                int length = list.Count;
                for (int i = length - 1; i >= 0; i--) {
                    if (list[i] == null) {
                        list.RemoveAt(i);
                    }
                }
                return list.Count == length;
            }
            catch (Exception e) {
                Debug.LogError(e);
                throw;
            }
        }

        public static void ReverseForeach<T>(IList<T> source, Action<T> action)
        {
            var length = source.Count;
            for (int i = length - 1; i >= 0; i--) {
                action?.Invoke(source[i]);
            }
        }
        
        /// <summary>
        /// false:退出
        /// true:继续执行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public static void ReverseForeach<T>(IList<T> source, Func<T,bool> action)
        {
            var length = source.Count;
            for (int i = length - 1; i >= 0; i--) {
                if (!action.Invoke(source[i])) {
                    break;
                }
            }
        }
    }
}