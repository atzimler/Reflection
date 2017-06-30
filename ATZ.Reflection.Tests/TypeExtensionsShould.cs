using FluentAssertions;
using NUnit.Framework;
using System;

namespace ATZ.Reflection.Tests
{
    [TestFixture]
    public class TypeExtensionsShould
    {
        [Test]
        public void ProperlyCloseTemplateWithCorrectArgumentCount()
        {
            var type = typeof(Template<>).CloseTemplate(new[] { typeof(BaseClass) });
            Assert.IsNotNull(type);
            Assert.IsNotNull(type.FullName);

            var parts = type.FullName.Split(new[] { "," }, StringSplitOptions.None);
            Assert.AreEqual("ATZ.Reflection.Tests.Template`1[[ATZ.Reflection.Tests.BaseClass", parts[0]);
        }

        [Test]
        public void ThrowExceptionWhenIncorrectNumberOfArgumentsIsProvided()
        {
            Assert.Throws(typeof(ArgumentException),
                () => typeof(Template<>).CloseTemplate(new[] { typeof(BaseClass), typeof(BaseClass) }));
        }

        [Test]
        public void ProvideMeaningfulErrorMessageForIncorrectNumberOfArguments()
        {
            try
            {
                typeof(Template<>).CloseTemplate(new[] { typeof(BaseClass), typeof(BaseClass) });
                Assert.Fail("Exception not thrown!");
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual(
                    "Type arguments to close ATZ.Reflection.Tests.Template`1 generic type are (<T>), while provided (ATZ.Reflection.Tests.BaseClass, ATZ.Reflection.Tests.BaseClass). Array counts mismatch (1 != 2)",
                    exception.Message
                    );
            }
        }

        [Test]
        public void ProvideCorrectNonGenericTypeName()
        {
            Assert.AreEqual("Template", typeof(Template<>).NonGenericName());
        }

        [Test]
        public void ProvideCorrectNonGenericTypeNameForMultiParameterGeneric()
        {
            Assert.AreEqual("IMultiParameterInterface", typeof(IMultiParameterInterface<,>).NonGenericName());
        }

        [Test]
        public void ProvideCorrectParameterizedGenericNameForContravariantTemplate()
        {
            Assert.AreEqual("IContravariantInterface{in BaseClass}", typeof(IContravariantInterface<>).ParameterizedGenericName(typeof(BaseClass)));
        }

        [Test]
        public void ProvideCorrectParameterizedGenericNameForNonVariantTemplate()
        {
            Assert.AreEqual("Template{BaseClass}", typeof(Template<>).ParameterizedGenericName(typeof(BaseClass)));
        }

        [Test]
        public void ReturnFalseForNonVariantTemplateParameterType()
        {
            var genericTypeParameters = typeof(Template<>).GetGenericTypeParameters();
            Assert.AreEqual(1, genericTypeParameters.Length);
            Assert.IsNotNull(genericTypeParameters[0]);
            Assert.IsFalse(genericTypeParameters[0].IsContravariant());
        }

        [Test]
        public void ReturnTrueForIsContravariantForContravariantTemplateParameterType()
        {
            var genericTypeParameters = typeof(IContravariantInterface<>).GetGenericTypeParameters();
            Assert.AreEqual(1, genericTypeParameters.Length);
            Assert.IsNotNull(genericTypeParameters[0]);
            Assert.IsTrue(genericTypeParameters[0].IsContravariant());
        }

        [Test]
        public void ReturnCorrectValueForTemplateParameterCountWithNonGenericClass()
        {
            Assert.AreEqual(0, typeof(BaseClass).GenericTypeParameterCount());
        }

        [Test]
        public void ReturnCorrectValueForTemplateParameterCountWithOneParameter()
        {
            Assert.AreEqual(1, typeof(IContravariantInterface<>).GenericTypeParameterCount());
        }

        [Test]
        public void ReturnCorrectValueForTemplateParameterCountWithTwoParameters()
        {
            Assert.AreEqual(2, typeof(IMultiParameterInterface<,>).GenericTypeParameterCount());
        }

        [Test]
        public void ThrowExceptionIfTemplateArgumentIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => typeof(IContravariantInterface<>).ParameterizedGenericName(null));
            Assert.IsNotNull(ex);
            ex.ParamName.Should().Be("templateArgument");
        }

        [Test]
        public void ReturnNonGenericNameForNonGenericTypes()
        {
            Assert.AreEqual(nameof(BaseClass), typeof(BaseClass).ParameterizedGenericName(typeof(int)));
        }

        [Test]
        public void ThrowExceptionIfTypeIsNullForGetMethod()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => TypeExtensions.IntrospectionGetMethod(null, "", new Type[] { }));
            Assert.IsNotNull(ex);
            ex.ParamName.Should().Be("type");
        }

        [Test]
        public void ThrowExceptionIfTypeIsNullForGetProperty()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => TypeExtensions.IntrospectionGetProperty(null, ""));
            Assert.IsNotNull(ex);
            ex.ParamName.Should().Be("type");
        }
    }
}
