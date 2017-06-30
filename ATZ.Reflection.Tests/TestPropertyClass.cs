namespace ATZ.Reflection.Tests
{
    public class TestPropertyClass
    {
        public int Property { get; set; }
        public int WriteOnlyProperty
        {
            // ReSharper disable once MemberCanBePrivate.Global => Just testing functionality of not having a get.
            // ReSharper disable once ValueParameterNotUsed => Just testing functionality of not having a get.
            set { }
        }
    }
}