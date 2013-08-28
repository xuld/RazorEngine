using System;
using System.Linq;
using System.Collections;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Web.Routing;
using System.Collections.Generic;

namespace Xuld.RazorEngine {

    /// <summary>
    /// 提供类型处理相关的辅助函数。
    /// </summary>
    internal static class TypeHelper {

        private static readonly Type DynamicType = typeof(DynamicObject);
        private static readonly Type ExpandoType = typeof(ExpandoObject);
        private static readonly Type EnumerableType = typeof(IEnumerable);
        private static readonly Type EnumeratorType = typeof(IEnumerator);
        
        /// <summary>
        /// Determines if the specified type is an anonymous type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is an anonymous type, otherwise false.</returns>
        public static bool IsAnonymousType(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            return (type.IsClass
                    && type.IsSealed
                    && type.BaseType == typeof(object)
                    && type.Name.StartsWith("<>", StringComparison.Ordinal)
                    && type.IsDefined(typeof(CompilerGeneratedAttribute), true));
        }

        /// <summary>
        /// Determines if the specified type is a dynamic type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is an anonymous type, otherwise false.</returns>
        public static bool IsDynamicType(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            return (DynamicType.IsAssignableFrom(type)
                    || ExpandoType.IsAssignableFrom(type)
                    || IsAnonymousType(type));
        }

        /// <summary>
        /// Determines if the specified type is a compiler generated iterator type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is an iterator type, otherwise false.</returns>
        public static bool IsIteratorType(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            return type.IsNestedPrivate
                && type.Name.StartsWith("<", StringComparison.Ordinal)
                && (EnumerableType.IsAssignableFrom(type) || EnumeratorType.IsAssignableFrom(type));
        }

        /// <summary>
        /// 获取指定类型的名字。
        /// </summary>
        /// <param name="type">类型对象。</param>
        /// <returns>返回类型名字。</returns>
        public static string GetTypeName(Type type) {
            if (IsIteratorType(type))
                type = GetFirstGenericInterface(type);

            if (!type.IsGenericType)
                return type.FullName;

            return type.Namespace
                  + "."
                  + type.Name.Substring(0, type.Name.IndexOf('`'))
                  + "<"
                  + string.Join(", ", type.GetGenericArguments().Select(GetTypeName))
                  + ">";
        }

        /// <summary>
        /// Gets the first generic interface of the specified type if one exists.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The first generic interface if one exists, otherwise the first interface or the target type itself if there are no interfaces.</returns>
        static Type GetFirstGenericInterface(Type type) {
            Type firstInterface = null;
            foreach (var @interface in type.GetInterfaces()) {
                if (firstInterface == null)
                    firstInterface = @interface;

                if (@interface.IsGenericType)
                    return @interface;
            }
            return @firstInterface ?? type;
        }

        /// <summary>
        /// 使用指定类型的默认构造函数来创建该类型的实例。
        /// </summary>
        /// <param name="type">要创建的对象的类型。</param>
        /// <returns>对新创建对象的引用。</returns>
        public static object CreateInstance(Type type) {
            return Activator.CreateInstance(type);
        }

        public static IDictionary<string, object> ObjectToDictionary(object value) {
            return new RouteValueDictionary(value);
        }

    }

}
