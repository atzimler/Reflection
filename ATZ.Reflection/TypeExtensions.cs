using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ATZ.Reflection
{
    public static class TypeExtensions
    {
        private static string TypeArgumentsToString(IEnumerable<Type> typeArguments)
        {
            return $"({string.Join(", ", typeArguments.ToList().ConvertAll(t => t.FullName ?? $"<{t.Name}>"))})";
        }

        public static Type CloseTemplate(this Type type, Type[] typeArguments)
        {
            var genericTypeParameters = type.GetTypeInfo().GenericTypeParameters;
            if (genericTypeParameters.Length != typeArguments.Length)
            {
                throw new ArgumentException(
                    $"Type arguments to close {type.FullName} generic type are {TypeArgumentsToString(genericTypeParameters)}, "
                    + $"while provided {TypeArgumentsToString(typeArguments)}. Array counts mismatch ({genericTypeParameters.Length} != {typeArguments.Length})"
                    );
            }
            return type.MakeGenericType(typeArguments);
        }

        public static string ContravariantGenericName(this Type type, Type templateArgument)
        {
            return $"{type.NonGenericName()}{{in {templateArgument.Name}}}";
        }

        public static string NonGenericName(this Type type)
        {
            return type.Name.Replace("`1", "");
        }
    }
}
