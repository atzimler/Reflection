using ATZ.Reflection.Linq;
using JetBrains.Annotations;
using System;
using System.Linq;
using System.Reflection;

namespace ATZ.Reflection
{
    /// <summary>
    /// Base class for separating class usage from requirement of referencing the library on lower level.
    /// </summary>
    public class ImplBase
    {
        private readonly object _impl;

        [NotNull]
        private readonly Type _type;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="impl">The implementation object to wrap.</param>
        /// <exception cref="ArgumentNullException">The object to be wrapped is null.</exception>
        protected ImplBase(object impl)
        {
            if (impl == null)
            {
                throw new ArgumentNullException(nameof(impl));
            }

            _impl = impl;
            _type = _impl.GetType();
        }

        [NotNull]
        private EventInfo GetEventInfo(string eventName)
        {
            var eventInfo = _type.GetEvent(eventName);
            if (eventInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(eventName), "Invalid event name supplied!");
            }

            return eventInfo;
        }

        private static MethodInfo GetGetMethod([NotNull] PropertyInfo pi)
        {
            return pi.GetGetMethod();
        }

        [NotNull]
        private MethodInfo GetHandlerMethod(string handlerName, Type eventArgsType)
        {
            if (handlerName == null)
            {
                throw new ArgumentNullException(nameof(handlerName));
            }

            var handler = GetType().GetMethod(handlerName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(object), eventArgsType },
                null);
            if (handler == null)
            {
                throw new ArgumentOutOfRangeException(nameof(handlerName), "Invalid handler name supplied!");
            }

            return handler;
        }

        [NotNull]
        private MethodInfo GetMethod(string functionName, Type[] parameterTypes)
        {
            if (functionName == null)
            {
                throw new ArgumentNullException(nameof(functionName));
            }
            if (parameterTypes == null)
            {
                throw new ArgumentNullException(nameof(parameterTypes));
            }

            var methodInfo = _type.GetMethod(functionName, parameterTypes);
            if (methodInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(functionName), "Class function with given name doest not exist!");
            }

            return methodInfo;
        }

        [NotNull]
        private MethodInfo GetPropertyMethod(string propertyName, [NotNull] Func<PropertyInfo, MethodInfo> propertyMethod, string propertyMethodName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var propertyInfo = _type.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(propertyName), "Property does not exist!");
            }

            var methodInfo = propertyMethod(propertyInfo);
            if (methodInfo == null)
            {
                throw new ArgumentOutOfRangeException(
                    $"Property {propertyMethodName} does not exist for {propertyName}!");
            }

            return methodInfo;
        }

        private MethodInfo GetSetMethod([NotNull] PropertyInfo pi)
        {
            return pi.GetSetMethod();
        }

        /// <summary>
        /// Attach event handler to an event.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments.</typeparam>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="handlerName">The name of the event handler.</param>
        /// <exception cref="ArgumentNullException">handlerName is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The event named in eventName or the handler named handlerName doest not exist.</exception>
        protected void AddEvent<T>(string eventName, string handlerName)
        {
            var eventInfo = GetEventInfo(eventName);
            var delegateType = eventInfo.EventHandlerType;
            var handler = GetHandlerMethod(handlerName, typeof(T));
            var d = Delegate.CreateDelegate(delegateType, this, handler);
            var addMethod = eventInfo.GetAddMethod();
            addMethod.Invoke(_impl, new object[] { d });
        }

        /// <summary>
        /// Execute a function on the wrapped object.
        /// </summary>
        /// <param name="functionName">The name of the function to execute.</param>
        /// <param name="parameterTypes">The types of the parameters passed to the function.</param>
        /// <param name="parameters">The parameters of the function.</param>
        /// <returns>The return value of the function.</returns>
        /// <exception cref="ArgumentNullException">functionName or parameterTypes is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Class function with functionName and parameterTypes parameters does not exist.</exception>
        protected object ExecuteFunction(string functionName, Type[] parameterTypes, params object[] parameters)
        {
            var method = GetMethod(functionName, parameterTypes);
            return method.Invoke(_impl, parameters);
        }

        /// <summary>
        /// Execute a method on the wrapped object.
        /// </summary>
        /// <param name="methodName">The name of the method to execute.</param>
        /// <param name="parameters">The parameters of the method. The parameter array should not contain a null value,
        /// because in that case the type of the object cannot be automatically determined.
        /// <seealso cref="ExecuteMethod(string,Type[],object[])"/>
        /// </param>
        /// <exception cref="ArgumentNullException">The argument functionName or parameters is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Class function with functionName and parameterTypes parameters does not exist, or parameters contains a null value.</exception>
        protected void ExecuteMethod(string methodName, params object[] parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var method = GetMethod(methodName, parameters.Types().ToArray());
            method.Invoke(_impl, parameters);
        }

        /// <summary>
        /// Execute a method on the wrapped object.
        /// </summary>
        /// <param name="methodName">The name of the method to execute.</param>
        /// <param name="parameterTypes">The type of the parameters.</param>
        /// <param name="parameters">The parameters of the method.</param>
        /// <exception cref="ArgumentNullException">functionName or parameters is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Class function with functionName and parameterTypes parameters does not exist.</exception>
        protected void ExecuteMethod(string methodName, Type[] parameterTypes, params object[] parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var method = GetMethod(methodName, parameterTypes);
            method.Invoke(_impl, parameters);
        }

        /// <summary>
        /// Get the value of a property.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentNullException">propertyName is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Property named in propertyName does not exist or does not have accessible getter.</exception>
        protected T GetProperty<T>(string propertyName)
        {
            var get = GetPropertyMethod(propertyName, GetGetMethod, "get");
            return (T)get.Invoke(_impl, null);
        }

        /// <summary>
        /// Detach event handler from an event.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments.</typeparam>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="handlerName">The name of the event handler.</param>
        /// <exception cref="ArgumentNullException">handlerName is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The event named in eventName or the handler named handlerName doest not exist.</exception>
        protected void RemoveEvent<T>(string eventName, string handlerName)
        {
            var eventInfo = GetEventInfo(eventName);
            var delegateType = eventInfo.EventHandlerType;
            var handler = GetHandlerMethod(handlerName, typeof(T));
            var d = Delegate.CreateDelegate(delegateType, this, handler);
            var removeMethod = eventInfo.GetRemoveMethod();
            removeMethod.Invoke(_impl, new object[] { d });
        }

        /// <summary>
        /// Set the value of a property.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">propertyName is null.</exception>
        protected void SetProperty<T>(string propertyName, T value)
        {
            var set = GetPropertyMethod(propertyName, GetSetMethod, "set");
            set.Invoke(_impl, new object[] { value });
        }


    }
}
