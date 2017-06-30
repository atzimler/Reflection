using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ATZ.Reflection.Linq
{
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

        private static bool ParametersMatchTypes(ParameterInfo[] parameters, Type[] parameterTypes)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameterTypes == null)
            {
                throw new ArgumentNullException(nameof(parameterTypes));
            }

            return CompareParameterTypes(parameters, parameterTypes);
        }

        [NotNull]
        public static IEnumerable<MethodInfo> Instance([NotNull] [ItemNotNull] this IEnumerable<MethodInfo> methods)
        {
            return methods.Where(mi => !mi.IsStatic);
        }

        public static IEnumerable<MethodInfo> WithParameterSignature([NotNull] [ItemNotNull] this IEnumerable<MethodInfo> methods, Type[] parameters)
        {
            return methods.Where(mi => ParametersMatchTypes(mi.GetParameters(), parameters));
        }
    }
}
