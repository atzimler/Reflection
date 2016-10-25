using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ATZ.Reflection;

namespace ATZ.Reflection.Tests
{
    [TestFixture]
    public class TypeExtensionsShould
    {
        [Test]
        public void ProperlyCloseTemplateWithCorrectArgumentCount()
        {
            var type = typeof(Template<>).CloseTemplate(new[] {typeof(BaseClass)});
            var parts = type.FullName.Split(new[] {","}, StringSplitOptions.None);
            Assert.AreEqual("ATZ.Reflection.Tests.Template`1[[ATZ.Reflection.Tests.BaseClass", parts[0]);
        }

        [Test]
        public void ThrowExceptionWhenIncorrectNumberOfArgumentsIsProvided()
        {
            Assert.Throws(typeof(ArgumentException),
                () => typeof(Template<>).CloseTemplate(new[] {typeof(BaseClass), typeof(BaseClass)}));
        }

        [Test]
        public void ProvideMeaningfulErrorMessageForIncorrectNumberOfArguments()
        {
            try
            {
                typeof(Template<>).CloseTemplate(new[] {typeof(BaseClass), typeof(BaseClass)});
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
        public void ProvideCorrectContravariantTemplateName()
        {
            Assert.AreEqual("Template{in BaseClass}", typeof(Template<>).ContravariantGenericName(typeof(BaseClass)));
        }
    }
}
