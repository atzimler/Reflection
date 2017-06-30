using System;

namespace ATZ.Reflection.Tests
{
    public class ImplBaseTester : ImplBase
    {
        public ImplBaseTester(object impl)
            : base(impl)
        {
        }

        public new void AddEvent<T>(string eventName, string handlerName)
        {
            base.AddEvent<T>(eventName, handlerName);
        }

        public new void RemoveEvent<T>(string eventName, string handlerName)
        {
            base.RemoveEvent<T>(eventName, handlerName);
        }

        public new object ExecuteFunction(string functionName, Type[] parameterTypes, params object[] parameters)
        {
            return base.ExecuteFunction(functionName, parameterTypes, parameters);
        }

        public new void ExecuteMethod(string methodName, params object[] parameters)
        {
            base.ExecuteMethod(methodName, parameters);
        }

        public void ExecuteMethod2(string methodName, Type[] parameterTypes, params object[] parameters)
        {
            base.ExecuteMethod(methodName, parameterTypes, parameters);
        }

        public new T GetProperty<T>(string propertyName)
        {
            return base.GetProperty<T>(propertyName);
        }

        public new void SetProperty<T>(string propertyName, T value)
        {
            base.SetProperty(propertyName, value);
        }


        public bool EventCalled { get; private set; }
        public void HandleEvent(object sender, EventArgs e)
        {
            EventCalled = true;
        }

    }
}
