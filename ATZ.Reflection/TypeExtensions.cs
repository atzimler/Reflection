using ATZ.Reflection.Linq;
using JetBrains.Annotations;
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
        [NotNull]
        private static IEnumerable<MethodInfo> IntrospectionGetDeclaredMethods([NotNull] this Type type, [NotNull] string methodName)
        {
            // ReSharper disable once AssignNullToNotNullAttribute => Microsoft libraries correctly return empty enumerations when no result.
            return type.IntrospectionGetTypeInfo().GetDeclaredMethods(methodName);
        }

        private static string TypeArgumentsToString([NotNull] IEnumerable<Type> typeArguments)
        {
            return $"({string.Join(", ", typeArguments.ToList().Select(t => t?.FullName ?? $"<{t?.Name}>"))})";
        }

        /// <summary>
        /// Gets the type which a type directly inherits.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>The type from which the type given as parameter directly inherits or null if the parameter type represents the object class or an interface.</returns>
        public static Type IntrospectionBaseType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IntrospectionGetTypeInfo().BaseType;
        }

        /// <summary>
        /// Searches for the specified instance method which matches the specified argument types in its parameter signature.
        /// </summary>
        /// <param name="type">The type on which the search should be done.</param>
        /// <param name="methodName">The name of the method to look up.</param>
        /// <param name="parameterTypes">The parameter signature of the method to look up.</param>
        /// <returns>The MethodInfo representing the method or null if the method was not found.</returns>
        public static MethodInfo IntrospectionGetMethod(this Type type, [NotNull] string methodName, [NotNull] Type[] parameterTypes)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IntrospectionGetDeclaredMethods(methodName).Instance().WithParameterSignature(parameterTypes).FirstOrDefault();
        }

        /// <summary>
        /// Searches for the specified property.
        /// </summary>
        /// <param name="type">The type on which the search should be done.</param>
        /// <param name="propertyName">The name of the property to look up.</param>
        /// <returns>The PropertyInfo representing the property or null if the property was not found.</returns>
        public static PropertyInfo IntrospectionGetProperty(this Type type, [NotNull] string propertyName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IntrospectionGetTypeInfo().GetDeclaredProperty(propertyName);
        }

        /// <summary>
        /// Returns the TypeInfo object and at the same time eliminates the ReSharper warning about possible null reference.
        /// </summary>
        /// <param name="type">The Type for which we are looking for the TypeInfo object.</param>
        /// <returns>The TypeInfo object associated with the Type.</returns>
        [NotNull]
        public static TypeInfo IntrospectionGetTypeInfo(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // ReSharper disable once AssignNullToNotNullAttribute => IntrospectionExtensions.GetTypeInfo() has
            // no documented return possibility for null and it would not make sense to return null for a non-null type.
            return type.GetTypeInfo();
        }

        /// <summary>
        /// Close the generic template with the specified types.
        /// </summary>
        /// <param name="type">The generic type to close with the provided arguments.</param>
        /// <param name="typeArguments">The types used to create the concrete type from the generic template.</param>
        /// <returns>The concrete type created from type by using typeArguments.</returns>
        /// <exception cref="ArgumentException">The count of type arguments provided mismatch the count of type arguments needed to complete the template.</exception>
        /// <exception cref="ArgumentNullException">The parameter typeArguments is null.</exception>
        [NotNull]
        public static Type CloseTemplate([NotNull] this Type type, Type[] typeArguments)
        {
            if (typeArguments == null)
            {
                throw new ArgumentNullException(nameof(typeArguments));
            }

            var genericTypeParameters = type.GetGenericTypeParameters();
            if (genericTypeParameters.Length != typeArguments.Length)
            {
                throw new ArgumentException(
                    $"Type arguments to close {type.FullName} generic type are {TypeArgumentsToString(genericTypeParameters)}, "
                    + $"while provided {TypeArgumentsToString(typeArguments)}. Array counts mismatch ({genericTypeParameters.Length} != {typeArguments.Length})"
                    );
            }
            // ReSharper disable once AssignNullToNotNullAttribute => A type is either created or exception is being thrown.
            return type.MakeGenericType(typeArguments);
        }

        /// <summary>
        /// Returns the number of type parameters in the generic type.
        /// </summary>
        /// <param name="type">The generic type.</param>
        /// <returns>The number of type parameters in the generic type.</returns>
        public static int GenericTypeParameterCount([NotNull] this Type type)
        {
            return GetGenericTypeParameters(type).Length;
        }

        /// <summary>
        /// Returns the type parameters of the generic type.
        /// </summary>
        /// <param name="type">The generic type.</param>
        /// <returns>The type parameters of the generic type or an empty array if the type is non-generic.</returns>
        [NotNull]
        [ItemNotNull]
        public static Type[] GetGenericTypeParameters([NotNull] this Type type)
        {
            var typeInfo = IntrospectionGetTypeInfo(type);
            // ReSharper disable once AssignNullToNotNullAttribute => According to the documentation of TypeInfo.GenericTypeParameters
            // property, it always returns a non-null value.
            return typeInfo.GenericTypeParameters;
        }


        /// <summary>
        /// Returns information on the usage of the type in the generic template.
        /// </summary>
        /// <param name="type">The type used in the generic template.</param>
        /// <returns>True if the type is used in the generic template as contra-variant (&lt;in&gt;), otherwise false.</returns>
        public static bool IsContravariant([NotNull] this Type type)
        {
            return (IntrospectionGetTypeInfo(type).GenericParameterAttributes & GenericParameterAttributes.Contravariant) != 0;
        }

        /// <summary>
        /// Returns the Name of the type without the indication on generic parameter count (`&lt;number&gt;).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The Name of the type with removed generic parameter count indication.</returns>
        public static string NonGenericName([NotNull] this Type type)
        {
            // ReSharper disable once PossibleNullReferenceException => types always have name.
            return type.Name.Replace($"`{type.GenericTypeParameterCount()}", "");
        }

        /// <summary>
        /// Returns a type name for the generic type with substituted type names for the parameters.
        /// </summary>
        /// <param name="type">The generic type.</param>
        /// <param name="templateArgument">The type arguments to substitute in the name as parameters.</param>
        /// <returns>The generic type name with additional parameters.</returns>
        /// <exception cref="ArgumentNullException">The argument templateArgument is null.</exception>
        public static string ParameterizedGenericName([NotNull] this Type type, Type templateArgument)
        {
            if (templateArgument == null)
            {
                throw new ArgumentNullException(nameof(templateArgument));
            }

            var genericTypeParameters = type.GetGenericTypeParameters();
            if (genericTypeParameters.Length == 0 || genericTypeParameters[0] == null)
            {
                return type.NonGenericName();
            }

            var contravariantModifier = genericTypeParameters[0].IsContravariant() ? "in " : "";

            return $"{type.NonGenericName()}{{{contravariantModifier}{templateArgument.Name}}}";
        }

    }
}
