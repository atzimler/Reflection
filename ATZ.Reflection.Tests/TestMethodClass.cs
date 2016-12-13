namespace ATZ.Reflection.Tests
{
    public class TestMethodClass
    {
        public bool CorrectMethodExecuted { get; set; } = false;

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
