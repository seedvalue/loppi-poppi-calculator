using System.Collections.Generic;
using System.Linq;
using _Main._Core.Model;
using UnityEngine;

namespace _Main._Core.Services
{
    public class PlayerPrefsCalculatorRepository : ICalculatorRepository
    {
        private const string EXPRESSION_KEY = "Calculator_CurrentExpression";
        private const string HISTORY_KEY = "Calculator_History";
        private const string HISTORY_SEPARATOR = "|~|";

        public void SaveState(string expression, List<string> history)
        {
            Debug.Log($"PlayerPrefsCalculatorRepository : SaveState : expression = {expression}, history = {history}");
            PlayerPrefs.SetString(EXPRESSION_KEY, expression ?? "");
            if (history != null)
            {
                var historyString = string.Join(HISTORY_SEPARATOR, history);
                PlayerPrefs.SetString(HISTORY_KEY, historyString);
            }
            else
            {
                PlayerPrefs.SetString(HISTORY_KEY, "");
            }
            PlayerPrefs.Save();
        }

        public (string expression, List<string> history) LoadState()
        {
            Debug.Log("PlayerPrefsCalculatorRepository : LoadState");
            var expression = PlayerPrefs.GetString(EXPRESSION_KEY, "");
            
            var historyString = PlayerPrefs.GetString(HISTORY_KEY, "");
            List<string> history = null;
            
            if (!string.IsNullOrEmpty(historyString))
            {
                history = historyString.Split(new[] { HISTORY_SEPARATOR }, System.StringSplitOptions.None).ToList();
            }
            return (expression, history);
        }
    }
}