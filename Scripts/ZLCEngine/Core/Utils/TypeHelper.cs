using System;
namespace ZLCEngine.Utils
{
    /// <summary>
    /// Type相关的方法
    /// </summary>
    public sealed class TypeHelper
    {
        /// <summary>
        /// 检测<paramref name="child"/>是否是<paramref name="parent"/>的子类型
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool IsChildOf(Type child, Type parent)
        {
            var temp = child;
            if (parent.IsInterface) {
                return temp.GetInterface(parent.Name) != null;
            } else {
                do {
                    if (temp.Name == parent.Name && temp.Namespace == parent.Namespace) return true;
                    temp = temp.BaseType;
                } while (temp != null);
            }

            return false;
        }

        /// <summary>
        /// 检测<paramref name="child"/>是否是<typeparamref name="T"/>的子类型
        /// </summary>
        /// <param name="child"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsChildOf<T>(Type child)
        {
            return IsChildOf(child, typeof(T));
        }

        /// <summary>
        /// 检测<paramref name="child"/>是否是<paramref name="parent"/>的子类型
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool IsChildOf(Type child, string parent)
        {
            var temp = child;
            if (temp.GetInterface(parent) != null) {
                return true;
            }

            do {
                if (temp.Name == parent) return true;
                temp = temp.BaseType;
            } while (temp != null);
            return false;
        }

        /// <summary>
        /// 获取继承的泛型接口的实际接口类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interface"></param>
        /// <returns></returns>
        public static Type GetGenericInterface(Type type, Type @interface)
        {
            var interfaces = type.GetInterfaces();
            foreach (var temp in interfaces) {
                if (temp.GetGenericTypeDefinition() == @interface) {
                    return temp;
                }
            }
            return null;
        }
    }
}