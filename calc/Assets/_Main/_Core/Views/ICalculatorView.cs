using System;
using System.Collections.Generic;

namespace _Main._Core.Views
{
    public interface ICalculatorView
    {
        event Action<string> OnExpressionChanged;
        event Action OnCalculateClicked;
        event Action OnErrorDialogClosed;
        string Expression { get; set; }
        void UpdateHistory(List<string> history);
        void ShowErrorDialog(string message); 
        void HideErrorDialog();
        void RestoreExpression(string expression);
    }
}