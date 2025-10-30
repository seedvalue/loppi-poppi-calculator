using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._UI.Dialogs.Views
{
    public class ErrorDialogView : MonoBehaviour
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button okButton;
        
        public event Action OnClosed;

        private void Awake()
        {
            okButton.onClick.AddListener(OnOkClicked); 
            //Hide();
        }

        public void Show(string message)
        {
            Debug.Log($"ErrorDialogView : Show : {message}");
            messageText.text = message;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Debug.Log($"ErrorDialogView : Hide");
            gameObject.SetActive(false);
        }

        private void OnOkClicked()
        {
            Debug.Log("ErrorDialogView : OnOkClicked");
            Hide();
            OnClosed?.Invoke();
        }
    }
}