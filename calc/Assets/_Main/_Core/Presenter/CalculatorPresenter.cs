using System.Collections.Generic;
using System.Linq;
using _Core.Model;
using _Core.Views;
using UnityEngine;

namespace _Main._Core.Presenter
{
    public class CalculatorPresenter
    {
        private readonly ICalculatorView _view;
        private readonly ICalculatorRepository _repository;
        private string _lastExpression = "";
        private List<string> _history = new();
        
        private const string ErrorMessage = "Please check the expression you just entered";

        public CalculatorPresenter(ICalculatorView view, ICalculatorRepository repository)
        {
            _view = view;
            _repository = repository;
            _view.OnExpressionChanged += OnExpressionChanged;
            _view.OnCalculateClicked += OnCalculateClicked;
            _view.OnErrorDialogClosed += OnErrorDialogClosed;
            LoadState();
        }

        private void OnExpressionChanged(string expression)
        {
            Debug.Log($"CalculatorPresenter : OnExpressionChanged : expression = {expression}");
            _lastExpression = expression;
            SaveState();
        }

        private void OnCalculateClicked()
        {
            Debug.Log($"CalculatorPresenter : OnCalculateClicked : _lastExpression = {_lastExpression}");
            if (string.IsNullOrEmpty(_lastExpression))
            {
                ShowError();
                return;
            }

            var result = CalculateExpression(_lastExpression);
            if (result.HasValue)
            {
                AddToHistory($"{_lastExpression}={result.Value}");
                ClearInput();
            }
            else
            {
                ShowError();
            }
        }

        private void OnErrorDialogClosed()
        {
            Debug.Log("CalculatorPresenter : OnErrorDialogClosed");
            _view.RestoreExpression(_lastExpression);
        }

        private static int? CalculateExpression(string expression)
        {
            Debug.Log($"CalculatorPresenter : CalculateExpression : expression = {expression}");
            if (string.IsNullOrEmpty(expression))
                return null;

            var parts = expression.Split('+');
            if (parts.Length != 2)
                return null;

            if (string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
                return null;

            if (!IsDigitsOnly(parts[0]) || !IsDigitsOnly(parts[1]))
                return null;

            if (int.TryParse(parts[0], out var a) && int.TryParse(parts[1], out var b))
            {
                return a + b;
            }
            return null;
        }

        private static bool IsDigitsOnly(string str)
        {
            return str.All(c => c is >= '0' and <= '9');
        }

        private void ShowError()
        {
           Debug.Log($"CalculatorPresenter : ShowError : _lastExpression = {_lastExpression}");
            AddToHistory($"{_lastExpression}=ERROR");
            ClearInput();
            _view.ShowErrorDialog(ErrorMessage);
        }

        private void AddToHistory(string record)
        {
            Debug.Log($"CalculatorPresenter : AddToHistory : record = {record}");
            _history.Add(record);
            UpdateHistoryView();
        }

        private void UpdateHistoryView()
        {
            Debug.Log($"CalculatorPresenter : UpdateHistoryView : _history = {_history}");
            _view.UpdateHistory(_history);
        }

        private void ClearInput()
        {
            Debug.Log("CalculatorPresenter : ClearInput");
            _lastExpression = "";
            _view.Expression = "";
            SaveState();
        }

        private void SaveState()
        {
            Debug.Log($"CalculatorPresenter : SaveState : _lastExpression = {_lastExpression}, _history = {_history}");
            _repository.SaveState(_lastExpression, _history);
        }

        private void LoadState()
        {
            Debug.Log("CalculatorPresenter : LoadState");
            var (expression, loadedHistory) = _repository.LoadState();
            _lastExpression = expression;
            _history = loadedHistory ?? new List<string>();
            _view.Expression = _lastExpression;
            UpdateHistoryView();
        }
    }
}