using System;
using System.Collections.Generic;
using UnityEngine;
using ZLCEngine.Utils;
namespace ZLCEditor.FormatSystem
{
    /// <summary>
    ///     转换器工厂
    /// </summary>
    internal class FormaterFactory : IDisposable
    {

        private Dictionary<FT, Type> _converterDic;

        /// <summary>
        ///     获取全部转换器并添加到字典中
        /// </summary>
        internal FormaterFactory()
        {
            // 反射获取全部转换器
            _converterDic = new Dictionary<FT, Type>();
            List<Type> childTypes = new List<Type>();
            EditorHelper.GetAllChildType(childTypes, EditorHelper.AssemblyFilterType.Custom | EditorHelper.AssemblyFilterType.Internal, typeof(IFormatConverter<,>));

            foreach (Type childType in childTypes) {
                Type interf = TypeHelper.GetGenericInterface(childType, typeof(IFormatConverter<,>));
                Type[] genericArguments = interf.GetGenericArguments();
                if (genericArguments.Length != 2) {
                    Debug.LogError($"错误的转换器类型{childType.FullName}");
                } else {
                    _converterDic.Add(new FT(genericArguments[0], genericArguments[1]), childType);
                }
            }
        }

        public void Dispose()
        {
            if (_converterDic == null) return;
            _converterDic.Clear();
            _converterDic = null;
        }

        /// <summary>
        ///     获取F到T的转换器
        /// </summary>
        /// <typeparam name="F">源数据</typeparam>
        /// <typeparam name="T">目标数据</typeparam>
        /// <returns>转换器</returns>
        internal IFormatConverter<F, T> GetConverter<F, T>()
        {
            FT ft = new FT(typeof(F), typeof(T));
            if (_converterDic.TryGetValue(ft, out Type converterType)) {
                return (IFormatConverter<F, T>)Activator.CreateInstance(converterType);
            }
            Debug.LogError($"请实现从{typeof(F).Name}转变为{typeof(T).Name}的转换器,继承IFormatConverter<F,T>");
            return null;
        }

        /// <summary>
        ///     析构
        /// </summary>
        ~FormaterFactory()
        {
            Dispose();
        }
        /// <summary>
        ///     From，To
        /// </summary>
        private struct FT : IEquatable<FT>
        {
            private Type _from;
            private Type _to;

            internal FT(Type from, Type to)
            {
                _from = from;
                _to = to;
            }

            public bool Equals(FT other)
            {
                return Equals(_from, other._from) && Equals(_to, other._to);
            }
            public override bool Equals(object obj)
            {
                return obj is FT other && Equals(other);
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(_from, _to);
            }
        }
    }
}