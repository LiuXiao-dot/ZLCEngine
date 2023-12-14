using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using ZLCEngine.Utils;
using AssemblyHelper = ZLCEngine.Utils.AssemblyHelper;
namespace ZLCEditor.Utils
{
    public sealed class EditorAssemblyHelper
    {
        /// <summary>
        ///     查找所有使用ToolDataAttribute/T标记的对象（不包含抽象类和接口）
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="temp"></param>
        /// <typeparam name="T"></typeparam>
        public static void GetAttributedTypes<T>(AssemblyDefinitionAsset[] assemblies, List<Type> temp) where T : Attribute
        {
            if (assemblies == null) return;
            foreach (AssemblyDefinitionAsset assembly in assemblies) {
                if (temp == null) temp = new List<Type>();
                AssemblyHelper.GetAttributedTypes<T>(Assembly.Load(assembly.name), temp);
            }
        }

        /// <summary>
        ///     获取所有继承与T的类
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="types"></param>
        /// <typeparam name="T"></typeparam>
        public static void GetAllChildType<T>(AssemblyDefinitionAsset[] assemblies, List<Type> temp)
        {
            GetAllChildType(assemblies, temp, typeof(T));
        }

        public static void GetAllChildType<T>(DefaultAsset[] assemblies, List<Type> temp)
        {
            GetAllChildType(assemblies, temp, typeof(T));
        }

        public static void GetAllChildType(IList<AssemblyDefinitionAsset> assemblies, List<Type> temp, Type targetType)
        {
            if (assemblies == null) return;
            foreach (AssemblyDefinitionAsset assembly in assemblies) {
                if (assembly == null) continue;
                if (temp == null) temp = new List<Type>();
                try {
                    AssemblyHelper.GetAllChildType(Assembly.Load(assembly.name), temp, targetType);
                }
                catch {
                }
            }
        }

        public static IEnumerable<Type> GetEnumerableChildType(AssemblyDefinitionAsset[] assemblies, Type targetType)
        {
            if (assemblies == null) yield break;
            foreach (AssemblyDefinitionAsset assembly in assemblies) {
                if (assembly == null) continue;
                Type[] types = Assembly.Load(assembly.name).GetTypes();
                foreach (Type type in types) {
                    if (type.IsAbstract || type.IsInterface) continue;
                    if (!TypeHelper.IsChildOf(type, targetType)) continue;
                    yield return type;
                }
            }
        }

        public static void GetAllChildType(IList<DefaultAsset> assemblies, List<Type> temp, Type targetType)
        {
            if (assemblies == null) return;
            foreach (DefaultAsset assembly in assemblies) {
                if (assembly == null) continue;
                if (temp == null) temp = new List<Type>();
                try {
                    AssemblyHelper.GetAllChildType(Assembly.Load(assembly.name), temp, targetType);
                }
                catch {

                }
            }
        }

        public static void GetAttributedTypes(IList<AssemblyDefinitionAsset> assemblies, List<Type> temp, Type attributeType)
        {
            if (assemblies == null) return;
            foreach (AssemblyDefinitionAsset assembly in assemblies) {
                if (assembly == null) continue;
                if (temp == null) temp = new List<Type>();
                AssemblyHelper.GetAttributedTypes(Assembly.Load(assembly.name), temp, attributeType);
            }
        }

        public static void GetAttributedTypes(IList<DefaultAsset> assemblies, List<Type> temp, Type attributeType)
        {
            if (assemblies == null) return;
            foreach (DefaultAsset assembly in assemblies) {
                if (assembly == null) continue;
                if (temp == null) temp = new List<Type>();
                AssemblyHelper.GetAttributedTypes(Assembly.Load(assembly.name), temp, attributeType);
            }
        }

        public static void GetAttributedTypes<T>(DefaultAsset[] assemblies, List<Type> temp) where T : Attribute
        {
            GetAttributedTypes(assemblies, temp, typeof(T));
        }
    }
}