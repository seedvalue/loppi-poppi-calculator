using System.Collections.Generic;

namespace _Main._Core.Model
{
    public interface ICalculatorRepository
    {
        void SaveState(string expression, List<string> history);
        (string expression, List<string> history) LoadState();
    }
}