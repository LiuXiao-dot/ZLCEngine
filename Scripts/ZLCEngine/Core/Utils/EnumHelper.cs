using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
namespace ZLCEngine.Utils
{

    /// <summary>
    ///     枚举类帮助类
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        ///     对与Flag型的枚举值，对每个值进行遍历
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <param name="action">对于每个存在的枚举值都执行一次</param>
        public static void ForEachFlag<T>(T enumValue, Action<T> action) where T : Enum
        {
            IEnumerable<T> flags = GetFlagEnumrator(enumValue);
            foreach (T flag in flags) {
                action(flag);
            }
        }

        /// <summary>
        ///     获取枚举值的每一位
        /// </summary>
        /// <param name="enumValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetFlagEnumrator<T>(T enumValue) where T : Enum
        {
            int temp = UnsafeUtility.As<T, int>(ref enumValue);
            int index = 0;
            while (temp > 0) {
                int flag = temp & 1;
                if (flag == 1) {
                    int tempInt = MathI.Pow(2, index);
                    yield return UnsafeUtility.As<int, T>(ref tempInt);
                }
                temp >>= 1;
                index++;
            }
        }
    }

}