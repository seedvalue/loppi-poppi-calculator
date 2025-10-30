using _Main._Core.Presenter;
using _Main._Core.Services;
using _Main._Core.Views;
using UnityEngine;

namespace _Main
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private CalculatorView calculatorView;
        
        private CalculatorPresenter _presenter;

        private void Start()
        {
            var repository = new PlayerPrefsCalculatorRepository();
            _presenter = new CalculatorPresenter(calculatorView, repository);
        }
    }
}