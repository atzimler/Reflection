namespace ATZ.Reflection.Tests
{
    public class TestMethodClass
    {
        public bool CorrectMethodExecuted { get; private set; }

        public void Method()
        {
            CorrectMethodExecuted = false;
        }

        public void Method(int i)
        {
            CorrectMethodExecuted = true;
        }
    }
}
