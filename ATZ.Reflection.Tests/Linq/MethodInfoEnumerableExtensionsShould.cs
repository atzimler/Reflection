using ATZ.Reflection.Linq;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace ATZ.Reflection.Tests.Linq
{
    [TestFixture]
    public class MethodInfoEnumerableExtensionsShould
    {
        [Test]
        public void ThrowExceptionIfParameterTypesIsNullForFiltering()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Enumerable.Empty<MethodInfo>().WithParameterSignature(null));
            Assert.IsNotNull(ex);
            ex.ParamName.Should().Be("parameterTypes");
        }
    }
}
