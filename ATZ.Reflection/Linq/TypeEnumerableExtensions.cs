using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace ATZ.Reflection.Linq
{
    /// <summary>
    /// LINQ extensions for manipulating type information.
    /// </summary>
    public static class TypeEnumerableExtensions
    {
        /// <summary>
        /// Converts an object enumeration into a type enumeration by getting the type of each objects.
        /// </summary>
        /// <param name="objects">The object enumeration.</param>
        /// <returns>The types of the objects.</returns>
        /// <exception cref="ArgumentOutOfRangeException">One or more object in the enumeration is null.</exception>
        [NotNull]
        public static IEnumerable<Type> Types([NotNull] this IEnumerable<object> objects)
        {
            foreach (var obj in objects)
            {
                if (obj == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(objects));
                }

                yield return obj.GetType();
            }
        }
    }
}
