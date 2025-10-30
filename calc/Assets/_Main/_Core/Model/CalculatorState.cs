using System;
using System.Collections.Generic;

namespace _Main._Core.Model
{
    [Serializable]
    public class CalculatorState
    {
        public string CurrentExpression = "";
        public List<string> History = new();
    }
}