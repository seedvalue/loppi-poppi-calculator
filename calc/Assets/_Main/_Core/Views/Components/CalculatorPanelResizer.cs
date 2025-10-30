using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Core.Views.Components
{
    public class CalculatorPanelResizer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform calculatorView;
        [SerializeField] private LayoutElement calculatorViewLayoutElement;
        [SerializeField] private ContentSizeFitter calculatorViewSizeFitter;


        [SerializeField] private RectTransform historyContent;
        [SerializeField] private RectTransform emptySpaceBtn;
        
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private RectTransform scrollViewRect; // RectTransform ScrollView для изменения размера

        [Header("Settings")] [SerializeField] private float maxHeight = 666f;
        [SerializeField] private float minHeight = 100f;

        [SerializeField]
        private float additionalHeightOffset = 80f; // Высота для input field, button и других элементов

        
        private LayoutElement _scrollViewLayoutElement;

        private void Start()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            if (scrollViewRect != null)
            {
                _scrollViewLayoutElement = scrollViewRect.GetComponent<LayoutElement>();
                if (_scrollViewLayoutElement == null)
                {
                    Debug.LogWarning("ScrollView doesn't have LayoutElement component! Adding one...");
                    _scrollViewLayoutElement = scrollViewRect.gameObject.AddComponent<LayoutElement>();
                }
            } else Debug.LogError("CalculatorView : InitializeComponents : scrollViewRect == NULL");

            // SetScrollEnabled(false);
            UpdateViewHeight();
        }

        public void OnContentChanged()
        {
            Debug.Log("CalculatorPanelResizer : OnContentChanged");
            UpdateViewHeight();
        }

        private void UpdateViewHeight()
        {
            Debug.Log("CalculatorPanelResizer : UpdateViewHeight...");
            // Check components
            if (calculatorView == null || historyContent == null || scrollView == null || calculatorViewLayoutElement == null)
            {
                var missing = new List<string>();
                if (calculatorView == null) missing.Add(nameof(calculatorView));
                if (historyContent == null) missing.Add(nameof(historyContent)); 
                if (scrollView == null) missing.Add(nameof(scrollView));
                if (calculatorViewLayoutElement == null) missing.Add(nameof(calculatorViewLayoutElement));
                Debug.LogError($"CalculatorPanelResizer: Missing required components: {string.Join(", ", missing)}");
                return;
            }

            var panelHeight = CalculatePanelHeight();
            Debug.Log($"CalculatorPanelResizer : UpdateViewHeight : panelHeight = {panelHeight}");
            
            var scrollViewHeight = CalculateScrollViewHeight();
            calculatorViewLayoutElement.preferredHeight = panelHeight;

            var currentSize = calculatorView.sizeDelta;
            calculatorView.sizeDelta = new Vector2(currentSize.x, panelHeight);
            Debug.Log(
                $"CalculatorPanelResizer : UpdateViewHeight : sizeDelta updated to {calculatorView.sizeDelta}");

            if (_scrollViewLayoutElement != null)
            {
                _scrollViewLayoutElement.preferredHeight = scrollViewHeight;
                Debug.Log(
                    $"CalculatorPanelResizer : UpdateScrollView : scrollView preferredHeight = {scrollViewHeight}");
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(calculatorView);
            Canvas.ForceUpdateCanvases();
        }


        


        #region Heights calculation

        private float CalculatePanelHeight()
        {
            var historyHeight = CalculateContentHeight();
            var emptyBtnHeight = GetEmptyBtnHeight();
            
            // Total = history + additionalOffset + emptyBtn, but clamped to maxHeight
            var totalContentHeight = historyHeight + additionalHeightOffset + emptyBtnHeight;
            var desiredHeightClamped = Mathf.Clamp(totalContentHeight, minHeight, maxHeight);
            
            Debug.Log($"CalculatorPanelResizer : CalculatePanelHeight : historyHeight={historyHeight}, emptyBtnHeight={emptyBtnHeight}, additionalOffset={additionalHeightOffset}, total={totalContentHeight}, clamped={desiredHeightClamped}");
            return desiredHeightClamped;
        }

        private float CalculateScrollViewHeight()
        {
            var historyHeight = CalculateContentHeight();
            var emptyBtnHeight = GetEmptyBtnHeight();
            
            // Calculate how much space is available for ScrollView when panel is at maxHeight
            var maxAvailableForScroll = maxHeight - additionalHeightOffset - emptyBtnHeight;
            
            // If history fits within available space, use full history height
            // Otherwise, limit ScrollView to available space
            var scrollViewHeight = Mathf.Min(historyHeight, maxAvailableForScroll);
            
            Debug.Log($"CalculatorPanelResizer : CalculateScrollViewHeight : historyHeight={historyHeight}, maxAvailable={maxAvailableForScroll}, result={scrollViewHeight}");
            return scrollViewHeight;
        }

        private float GetEmptyBtnHeight()
        {
            if (emptySpaceBtn == null) return 0f;
            if (!emptySpaceBtn.gameObject.activeSelf) return 0f;
            return emptySpaceBtn.rect.height;
        }

        private float CalculateContentHeight()
        {
            if (historyContent.childCount == 0) return 0f;
            var totalHeight = 0f;
            var layoutGroup = historyContent.GetComponent<VerticalLayoutGroup>();
            for (var i = 0; i < historyContent.childCount; i++)
            {
                var child = historyContent.GetChild(i);
                var rect = child.GetComponent<RectTransform>();
                if (rect != null)
                {
                    totalHeight += rect.rect.height;
                }
                else
                {
                    Debug.LogError("CalculatorPanelResizer : CalculateContentHeight : rect == NULL");
                }
            }

            // Добавляем spacing и padding
            if (layoutGroup != null)
            {
                if (historyContent.childCount > 1)
                {
                    totalHeight += layoutGroup.spacing * (historyContent.childCount - 1);
                }

                totalHeight += layoutGroup.padding.top + layoutGroup.padding.bottom;
            }
            else
            {
                Debug.LogError("CalculatorPanelResizer : CalculateContentHeight : layoutGroup == NULL");
            }

            Debug.Log($"CalculatorPanelResizer : CalculateContentHeight : totalHeight = {totalHeight}");
            return totalHeight;
        }

        #endregion
        
    }
}