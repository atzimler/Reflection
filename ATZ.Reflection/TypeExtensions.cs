using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ATZ.Reflection
{
    /// <summary>
    /// Extension methods for the Type type.
    /// </summary>
    public static class TypeExtensions
    {
        private static string TypeArgumentsToString(IEnumerable<Type> typeArguments)
        {
            return $"({string.Join(", ", typeArguments.ToList().ConvertAll(t => t.FullName ?? $"<{t.Name}>"))})";
        }

        /// <summary>
        /// Close the the generic template with the specified types.
        /// </summary>
        /// <param name="type">The generic type to close with the provided arguments.</param>
        /// <param name="typeArguments">The types used to create the concrete type from the generic template.</param>
        /// <returns>The concrete type created from type by using typeArguments.</returns>
        /// <exception cref="ArgumentException">The count of type arguments provided mismatch the count of type arguments needed to complete the template.</exception>
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

        /// <summary>
        /// Returns the number of type parameters in the generic type.
        /// </summary>
        /// <param name="type">The generic type.</param>
        /// <returns>The number of type parameters in the generic type.</returns>
        public static int GenericTypeParameterCount(this Type type)
        {
            return type.GetTypeInfo().GenericTypeParameters.Length;
        }

        /// <summary>
        /// Returns information on the usage of the type in the generic template.
        /// </summary>
        /// <param name="type">The type used in the generic template.</param>
        /// <returns>True if the type is used in the generic template as contravariant (&lt;in&gt;), otherwise false.</returns>
        public static bool IsContravariant(this Type type)
        {
            return (type.GetTypeInfo().GenericParameterAttributes & GenericParameterAttributes.Contravariant) != 0;
        }

        /// <summary>
        /// Returns the Name of the type without the indication on generic parameter count (`&lt;number&gt;).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The Name of the type with removed generic parameter count indication.</returns>
        public static string NonGenericName(this Type type)
        {
            return type.Name.Replace($"`{type.GenericTypeParameterCount()}", "");
        }

        /// <summary>
        /// Returns a type name for the generic type with substituted type names for the parameters.
        /// </summary>
        /// <param name="type">The generic type.</param>
        /// <param name="templateArgument">The type arguments to substitute in the name as parameters.</param>
        /// <returns>The generic type name with additional parameters.</returns>
        public static string ParameterizedGenericName(this Type type, Type templateArgument)
        {
            var genericTypeParameters = type.GetTypeInfo().GenericTypeParameters;
            var contravariantModifier = genericTypeParameters[0].IsContravariant() ? "in " : "";

            return $"{type.NonGenericName()}{{{contravariantModifier}{templateArgument.Name}}}";
        }

    }
}
