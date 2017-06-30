namespace ATZ.Reflection.Tests
{
    public class TestPropertyClass
    {
        private int _w;

        public int Property { get; set; }
        public int WriteOnlyProperty
        {
            set => _w = value;
        }
    }
}