using System;
using System.Collections.Generic;
using _Core.Views;
using _Core.Views.Components;
using _UI.Dialogs.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Core.Views
{
    public class CalculatorView : MonoBehaviour, ICalculatorView
    {
        [Header("UI References")] [SerializeField]
        private TMP_InputField expressionInput;

        [SerializeField] private Button calculateButton;
        [SerializeField] private ScrollRect historyScrollView;
        [SerializeField] private RectTransform historyContent;
        [SerializeField] private GameObject historyItemPrefab;
        [SerializeField] private ErrorDialogView errorDialogView;

        [Header("Input:")] [SerializeField] private Color caretColor = Color.white;
        [SerializeField] private Image imageLine;
        [SerializeField] private Color imageColorActivated = Color.blue;
        [SerializeField] private Color imageColorDeactivated = Color.gray;
        [SerializeField] private GameObject emptySpace;
        [SerializeField] private GameObject emptySpaceBtn;
        
        [Header("History Item")] [SerializeField]
        private float historyItemHeight = 30f;

        [Header("Layout")] [SerializeField] private CalculatorPanelResizer panelResizer;

        //Actions:
        public event Action<string> OnExpressionChanged;
        public event Action OnCalculateClicked;
        public event Action OnErrorDialogClosed;

        public string Expression
        {
            get => expressionInput.text;
            set => expressionInput.text = value;
        }


        public void UpdateHistory(List<string> historyRecords)
        {
            Debug.Log($"CalculatorView : UpdateHistory : {historyRecords.Count}");
            ClearContent();
            FillContent(historyRecords);
            
            // Show/hide empty space based on history count
            RefreshEmptySpaces(historyRecords.Count);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(historyContent);
            Canvas.ForceUpdateCanvases();

            if (historyScrollView != null)
            {
                historyScrollView.normalizedPosition = Vector2.up;
            }
            else
            {
                Debug.LogError("CalculatorView : UpdateHistory : historyScrollView == NULL");
            }

            if (panelResizer != null)
            {
                panelResizer.OnContentChanged();
            }
            else
            {
                Debug.LogError("CalculatorView : UpdateHistory : panelResizer == NULL");
            }
        }

        private void RefreshEmptySpaces(int historyRecordsCount)
        {
            if (emptySpace != null)
            {
               // emptySpace.SetActive(historyRecordsCount == 0);
            }
            else  Debug.LogError("CalculatorView : RefreshEmptySpaces : emptySpace == NULL");

            if (emptySpaceBtn != null)
            {
                emptySpaceBtn.SetActive(historyRecordsCount > 0);
            }
            else  Debug.LogError("CalculatorView : RefreshEmptySpaces : emptySpaceBtn == NULL");
        }


        public void RestoreExpression(string expression)
        {
            Debug.Log($"CalculatorView : RestoreExpression : {expression}");
            expressionInput.text = expression;
        }


        #region Events

        private void OnInputChanged(string value)
        {
            Debug.Log($"CalculatorView : OnInputChanged : {value}");
            OnExpressionChanged?.Invoke(value);
        }

        private void OnCalculateButtonClicked()
        {
            Debug.Log("CalculatorView : OnCalculateButtonClicked");
            OnCalculateClicked?.Invoke();
        }

        private void OnErrorDialogClosedHandler()
        {
            Debug.Log("CalculatorView : OnErrorDialogClosedHandler");
            OnErrorDialogClosed?.Invoke();
        }

        #endregion



        #region ErrorDialog

        public void ShowErrorDialog(string message)
        {
            Debug.Log($"CalculatorView : ShowErrorDialog : {message}");
            if (errorDialogView != null)
            {
                errorDialogView.Show(message);
            }
            else
            {
                Debug.LogWarning("ErrorDialogView is not assigned!");
                OnErrorDialogClosed?.Invoke();
            }
        }

        public void HideErrorDialog()
        {
            Debug.Log("CalculatorView : HideErrorDialog");
            if (errorDialogView != null)
            {
                errorDialogView.Hide();
            }
        }

        #endregion



        #region Content

        private void ClearContent()
        {
            Debug.Log("CalculatorView : ClearContent");
            foreach (Transform child in historyContent)
            {
                Destroy(child.gameObject);
            }
        }

        private void FillContent(List<string> historyRecords)
        {
            Debug.Log($"CalculatorView : FillContent : historyRecords.Count = {historyRecords.Count}");
            for (var i = historyRecords.Count - 1; i >= 0; i--)
            {
                var item = Instantiate(historyItemPrefab, historyContent);
                var textComponent = item.GetComponentInChildren<TMP_Text>();
                if (textComponent != null)
                {
                    var text = historyRecords[i];
                    textComponent.text = text;
                    Debug.Log($"CalculatorView : UpdateHistory : text = {text}");
                }
                else
                {
                    Debug.LogError("CalculatorView : UpdateHistory : textComponent == NULL");
                }

                var layoutElement = item.GetComponent<LayoutElement>();
                if (layoutElement == null)
                {
                    layoutElement = item.gameObject.AddComponent<LayoutElement>();
                }

                layoutElement.minHeight = historyItemHeight;
            }
        }

        #endregion


        #region Input Focus

        private void OnInputSelected(string value)
        {
            Debug.Log("CalculatorView : OnInputSelected");
            SetImageLineColor(true);
        }

        private void OnInputDeselected(string value)
        {
            Debug.Log("CalculatorView : OnInputDeselected");
            SetImageLineColor(false);
        }

        private void SetImageLineColor(bool isActivated)
        {
            if (imageLine != null)
            {
                imageLine.color = isActivated ? imageColorActivated : imageColorDeactivated;
                Debug.Log($"CalculatorView : SetImageLineColor : {imageLine.color}");
            }
            else
            {
                Debug.LogError("CalculatorView : SetImageLineColor : imageLine is null");
            }
        }

        #endregion



        #region Unity

        private void Awake()
        {
            expressionInput.onValueChanged.AddListener(OnInputChanged);
            expressionInput.onSelect.AddListener(OnInputSelected);
            expressionInput.onDeselect.AddListener(OnInputDeselected);
            expressionInput.caretColor = caretColor;
            calculateButton.onClick.AddListener(OnCalculateButtonClicked);

            if (errorDialogView != null)
            {
                errorDialogView.OnClosed += OnErrorDialogClosedHandler;
            }
            else Debug.LogError("CalculatorView : Awake : ErrorDialogView is not assigned!");

            //ClearContent(); // Kill test content (used for check UI)
            //UpdateHistory(new List<string>());
            SetImageLineColor(false); // Start with deactivated color
        }

        private void OnDestroy()
        {
            expressionInput.onValueChanged.RemoveListener(OnInputChanged);
            expressionInput.onSelect.RemoveListener(OnInputSelected);
            expressionInput.onDeselect.RemoveListener(OnInputDeselected);
            calculateButton.onClick.RemoveListener(OnCalculateButtonClicked);

            if (errorDialogView != null)
            {
                errorDialogView.OnClosed -= OnErrorDialogClosedHandler;
            }
        }

        #endregion

    }
}