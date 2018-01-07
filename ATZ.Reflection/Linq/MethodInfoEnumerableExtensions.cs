using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ATZ.Reflection.Linq
{
    /// <summary>
    /// Extensions for handling MethodInfo enumerations.
    /// </summary>
    public static class MethodInfoEnumerableExtensions
    {
        private static bool CompareParameterTypes([NotNull] IReadOnlyList<ParameterInfo> parameters, [NotNull] IReadOnlyList<Type> parameterTypes)
        {
            if (parameters.Count != parameterTypes.Count)
            {
                return false;
            }

            for (var p = 0; p < parameters.Count; ++p)
            {
                if (parameters[p]?.ParameterType != parameterTypes[p])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Keep only the MethodInfo objects in the enumeration which are instance methods.
        /// </summary>
        /// <param name="methods">The enumeration of MethodInfo objects on which the filter operation should be executed.</param>
        /// <returns>The new enumeration where each of the element is an instance method.</returns>
        [NotNull]
        public static IEnumerable<MethodInfo> Instance([NotNull] [ItemNotNull] this IEnumerable<MethodInfo> methods)
        {
            return methods.Where(mi => !mi.IsStatic);
        }

        /// <summary>
        /// Keep only the MethodInfo objects in the enumeration which have compatible parameter type.
        /// </summary>
        /// <param name="methods">The enumeration of MethodInfo objects on which the filter operation should be executed.</param>
        /// <param name="parameterTypes">The parameter signature that should be matched to keep in the enumeration.</param>
        /// <returns>The new enumeration where each of the element has the given parameter signature.</returns>
        public static IEnumerable<MethodInfo> WithParameterSignature([NotNull] [ItemNotNull] this IEnumerable<MethodInfo> methods, Type[] parameterTypes)
        {
            if (parameterTypes == null)
            {
                throw new ArgumentNullException(nameof(parameterTypes));
            }

            // ReSharper disable once AssignNullToNotNullAttribute => .NET Framework correctly returns empty array if no parameters.
            return methods.Where(mi => CompareParameterTypes(mi.GetParameters(), parameterTypes));
        }
    }
}
