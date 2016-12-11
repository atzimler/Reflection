using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
