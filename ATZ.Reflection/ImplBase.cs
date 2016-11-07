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
        private readonly Type _type;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="impl">The implementation object to wrap.</param>
        protected ImplBase(object impl)
        {
            _impl = impl;
            _type = _impl.GetType();
        }

        /// <summary>
        /// Attach event handler to an event.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments.</typeparam>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="handlerName">The name of the event handler.</param>
        protected void AddEvent<T>(string eventName, string handlerName)
        {
            var eventInfo = _type.GetEvent(eventName);
            var delegateType = eventInfo.EventHandlerType;
            var handler = GetType().GetMethod(handlerName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
        protected object ExecuteFunction(string functionName, Type[] parameterTypes, params object[] parameters)
        {
            var method = _type.GetMethod(functionName, parameterTypes);
            return method.Invoke(_impl, parameters);
        }

        /// <summary>
        /// Execute a method on the wrapped object.
        /// </summary>
        /// <param name="methodName">The name of the method to execute.</param>
        /// <param name="parameters">The parameters of the method.</param>
        protected void ExecuteMethod(string methodName, params object[] parameters)
        {
            var parameterTypes = parameters.ToList().ConvertAll(p => p.GetType()).ToArray();

            var method = _type.GetMethod(methodName, parameterTypes);
            method.Invoke(_impl, parameters);
        }

        /// <summary>
        /// Get the value of a property.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        protected T GetProperty<T>(string propertyName)
        {
            var property = _type.GetProperty(propertyName);
            var get = property.GetGetMethod();
            return (T)get.Invoke(_impl, null);
        }

        /// <summary>
        /// Detach event handler from an event.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments.</typeparam>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="handlerName">The name of the event handler.</param>
        protected void RemoveEvent<T>(string eventName, string handlerName)
        {
            var eventInfo = _type.GetEvent(eventName);
            var delegateType = eventInfo.EventHandlerType;
            var handler = GetType()
                .GetMethod(handlerName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
        protected void SetProperty<T>(string propertyName, T value)
        {
            var property = _type.GetProperty(propertyName);
            var set = property.GetSetMethod();
            set.Invoke(_impl, new object[] { value });
        }


    }
}
