using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static int GenericTypeParameterCount(this Type type)
        {
            return type.GetTypeInfo().GenericTypeParameters.Length;
        }

        public static bool IsContravariant(this Type type)
        {
            return (type.GetTypeInfo().GenericParameterAttributes & GenericParameterAttributes.Contravariant) != 0;
        }

        public static string NonGenericName(this Type type)
        {
            return type.Name.Replace("`1", "");
        }

        public static string ParameterizedGenericName(this Type type, Type templateArgument)
        {
            var genericTypeParameters = type.GetTypeInfo().GenericTypeParameters;
            var contravariantModifier = genericTypeParameters[0].IsContravariant() ? "in " : "";

            return $"{type.NonGenericName()}{{{contravariantModifier}{templateArgument.Name}}}";
        }

    }
}
