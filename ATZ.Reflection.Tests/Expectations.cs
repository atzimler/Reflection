using NUnit.Framework;
using System.Reflection;

namespace ATZ.Reflection.Tests
{
    [TestFixture]
    public class Expectations
    {
        // ReSharper disable once MemberCanBePrivate.Global => This is to trick the resolver in case of not considering the parameters because it is public.
        // ReSharper disable once MemberCanBeMadeStatic.Global => Cannot be static, we want to resolve dynamic methods.
        public int EmptyMethodToResolve(string str)
        {
            return 13;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local => Cannot be static, we want to resolve dynamic methods.
        // ReSharper disable once UnusedParameter.Local => Just to differentiate between the method signatures, because visibility cannot be the only difference.
        private int EmptyMethodToResolve(object obj)
        {
            return 42;
        }

        [Test]
        public void PrivateMethodsCanBeAlsoResolvedByParameterTypes()
        {
            var methodInfo = GetType().GetMethod(nameof(EmptyMethodToResolve), BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(object) }, null);
            Assert.IsNotNull(methodInfo);
            // ReSharper disable once CoVariantArrayConversion => will not use the parameter array, just verifying the correct method has been resolved.
            Assert.AreEqual(42, methodInfo.Invoke(this, new[] { string.Empty }));
        }
    }
}
