using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Translations.UI
{
    public class TranslationDropdown : MonoBehaviour
    {
        [Header("Button")]
        public Button button;
        public TMP_Text buttonLabel;

        [Header("Content")]
        public Canvas content;
        public TranslationDropdownOption itemTemplate;

        public TranslatableText loadingText;

        List<Item> items = new List<Item>();

        private void Reset()
        {
            
        }

        GameObject blocker;
        bool isLoading = false;

        private void Start()
        {
            if (itemTemplate?.gameObject?.scene != null)
                itemTemplate.gameObject.SetActive(false);

            isLoading = TranslationManager.Serializer.IsLoadingTranslationInfo;
            RefreshContent();
        }

        private void OnEnable()
        {
            TranslationManager.Serializer.OnStartLoadingInfoList += Serializer_OnStartLoadingInfoList;
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            TranslationManager.Serializer.OnStartLoadingInfoList -= Serializer_OnStartLoadingInfoList;
            button.onClick.RemoveListener(OnClick);
        }

        private void Serializer_OnStartLoadingInfoList()
        {
            isLoading = true;
        }

        private void OnClick()
        {
            content.gameObject.SetActive(true);
            CreateBlocker();
            RefreshContent();
        }

        private void CloseDropdown()
        {
            content.gameObject.SetActive(false);
            if (blocker != null)
            {
                Destroy(blocker);
                blocker = null;
            }
        }

        private void OnSelectOption(int index)
        {
            CloseDropdown();
            
            Debug.Log(index);
            var info = items[index].info;
            var trans = TranslationManager.Serializer.LoadTranslation(info);
            TranslationManager.LoadTranslation(trans);
        }

        void CreateBlocker()
        {
            Transform canvas = transform;
            while (canvas.parent != null)
            {
                if (canvas.GetComponent<Canvas>() != null)
                    break;

                canvas = canvas.parent;
            }

            blocker = new GameObject("Blocker");
            blocker.transform.SetParent(canvas);

            var rectTrans = blocker.AddComponent<RectTransform>();
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.offsetMin = Vector2.zero;
            rectTrans.offsetMax = Vector2.zero;
            var blockerCanvas = blocker.AddComponent<Canvas>();
            blockerCanvas.overrideSorting = true;
            blockerCanvas.sortingOrder = content.sortingOrder - 1;
            blocker.AddComponent<GraphicRaycaster>();
            blocker.AddComponent<CanvasRenderer>();
            var img = blocker.AddComponent<Image>();
            img.color = Color.clear;
            var btn = blocker.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(CloseDropdown);
        }

        private void Update()
        {
            if (isLoading && !TranslationManager.Serializer.IsLoadingTranslationInfo)
            {
                isLoading = false;
                RefreshContent();
            }
        }

        void RefreshContent()
        {
            var loadedInfo = TranslationManager.Serializer.LoadedInfo;
            var parent = itemTemplate.transform.parent as RectTransform;
            var elHeight = (itemTemplate.transform as RectTransform).sizeDelta.y;

            while (items.Count < loadedInfo.Count)
            {
                var newOption = Instantiate(itemTemplate, parent);
                newOption.transform.localPosition = Vector3.down * elHeight * items.Count;
                var index = items.Count;
                newOption.toggle.onValueChanged.AddListener(_ => OnSelectOption(index));
                newOption.gameObject.SetActive(true);

                items.Add(new Item()
                {
                    option = newOption,
                });
            }

            while (items.Count > loadedInfo.Count)
                Destroy(items.Last().option.gameObject);

            parent.sizeDelta = new Vector2(parent.sizeDelta.x, items.Count * elHeight);

            for (int i = 0; i < loadedInfo.Count; i++)
            {
                items[i].info = loadedInfo[i];
                items[i].option.LoadContent(loadedInfo[i]);
                bool selected = TranslationManager.LoadedTranslation.Info.Path == loadedInfo[i].Path;
                items[i].option.toggle.SetIsOnWithoutNotify(selected);
            }
        }

        class Item
        {
            public TranslationDropdownOption option;
            public TranslationInfo info;
        }
    }
}