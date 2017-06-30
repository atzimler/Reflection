using FluentAssertions;
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
        public void ThrowExceptionIfMethodNameIsNull()
        {
            var obj = new TestMethodClass();
            var wrapper = new ImplBaseTester(obj);

            var ex = Assert.Throws<ArgumentNullException>(() => wrapper.ExecuteMethod(null, 42));
            Assert.IsNotNull(ex);
            ex.ParamName.Should().Be("methodName");
        }

        [Test]
        public void ThrowExceptionIfMethodNameIsInvalid()
        {
            var obj = new TestMethodClass();
            var wrapper = new ImplBaseTester(obj);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => wrapper.ExecuteMethod("invalid", 42));
            Assert.IsNotNull(ex);
            ex.Message.Should().Be("Class method with given name does not exist!\r\nParameter name: methodName");
        }

        [Test]
        public void ThrowExceptionIfParametersIsNull()
        {
            var obj = new TestMethodClass();
            var wrapper = new ImplBaseTester(obj);

            var ex = Assert.Throws<ArgumentNullException>(() => wrapper.ExecuteMethod(nameof(obj.Method), null));
            Assert.IsNotNull(ex);
            ex.ParamName.Should().Be("parameters");
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
        public void ThrowExeceptionIfParametersIsNullWithGivenParameterTypes()
        {
            var obj = new TestMethodClass();
            var wrapper = new ImplBaseTester(obj);

            var ex = Assert.Throws<ArgumentNullException>(
                () => wrapper.ExecuteMethod2(nameof(obj.Method), new[] { typeof(int) }, null));
            Assert.IsNotNull(ex);
            ex.ParamName.Should().Be("parameters");
        }

        [Test]
        public void ExecuteMethodProperlyWithGivenParameterTypes()
        {
            var obj = new TestMethodClass();
            var wrapper = new ImplBaseTester(obj);

            wrapper.ExecuteMethod2(nameof(obj.Method), new[] { typeof(int) }, 42);
            Assert.IsTrue(obj.CorrectMethodExecuted);
        }

        [Test]
        public void ThrowExceptionIfPropertyNameIsNull()
        {
            var obj = new TestPropertyClass();
            var wrapper = new ImplBaseTester(obj);

            var ex = Assert.Throws<ArgumentNullException>(() => wrapper.GetProperty<int>(null));
            Assert.IsNotNull(ex);
            ex.ParamName.Should().Be("propertyName");
        }

        [Test]
        public void ThrowExceptionIfPropertyDoesNotExist()
        {
            var obj = new TestPropertyClass();
            var wrapper = new ImplBaseTester(obj);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => wrapper.GetProperty<int>("invalid"));
            Assert.IsNotNull(ex);
            ex.Message.Should().Be("Property does not exist!\r\nParameter name: propertyName");
        }

        [Test]
        public void ThrowExceptionIfPropertyAccessMethodDoesNotExist()
        {
            var obj = new TestPropertyClass();
            var wrapper = new ImplBaseTester(obj);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => wrapper.GetProperty<int>(nameof(obj.WriteOnlyProperty)));
            Assert.IsNotNull(ex);
            ex.Message.Should().Be("Specified argument was out of the range of valid values.\r\nParameter name: Property get does not exist for WriteOnlyProperty!");
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
