using NUnit.Framework;
using System;

namespace ATZ.Reflection.Tests
{
    [TestFixture]
    public class ImplBaseShould
    {
        [Test]
        public void ExecuteFunctionProperly()
        {
            var obj = new TestFunctionClass();
            var wrapper = new ImplBaseTester(obj);

            Assert.AreEqual(42, wrapper.ExecuteFunction(nameof(obj.Function), new[] { typeof(int) }, 42));
        }

        [Test]
        public void ExecuteMethodProperly()
        {
            var obj = new TestMethodClass();
            var wrapper = new ImplBaseTester(obj);

            wrapper.ExecuteMethod(nameof(obj.Method), 42);
            Assert.IsTrue(obj.CorrectMethodExecuted);
        }

        [Test]
        public void ReturnCorrectPropertyValue()
        {
            var obj = new TestPropertyClass { Property = 42 };
            var wrapper = new ImplBaseTester(obj);

            Assert.AreEqual(42, wrapper.GetProperty<int>(nameof(obj.Property)));
        }

        [Test]
        public void SetPropertyCorrectly()
        {
            var obj = new TestPropertyClass { Property = 13 };
            var wrapper = new ImplBaseTester(obj);

            wrapper.SetProperty(nameof(TestPropertyClass.Property), 42);
            Assert.AreEqual(42, obj.Property);
        }

        [Test]
        public void AttachEventCorrectly()
        {
            var obj = new TestEventClass();
            var wrapper = new ImplBaseTester(obj);

            wrapper.AddEvent<EventArgs>(nameof(TestEventClass.Event), nameof(ImplBaseTester.HandleEvent));
            obj.FireEvent();
            Assert.IsTrue(wrapper.EventCalled);
        }

        [Test]
        public void DetachEventCorrectly()
        {
            var obj = new TestEventClass();
            var wrapper = new ImplBaseTester(obj);

            obj.Event += wrapper.HandleEvent;
            wrapper.RemoveEvent<EventArgs>(nameof(TestEventClass.Event), nameof(ImplBaseTester.HandleEvent));
            obj.FireEvent();
            Assert.IsFalse(wrapper.EventCalled);
        }
    }

    public class TestEventClass
    {
        public event EventHandler<EventArgs> Event;

        public void FireEvent()
        {
            Event?.Invoke(this, EventArgs.Empty);
        }
    }
}
