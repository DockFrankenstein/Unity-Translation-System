using UnityEngine;
using UnityEditor;
using Translations.UI;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Translations.Editor
{
    public static class GameObjectFactory
    {
        [MenuItem("GameObject/Translation System/Translation Dropdown", false, 1)]
        public static void CreateDropdown()
        {
            var dark = new Color(0.1960784f, 0.1960784f, 0.1960784f);
            var rootCanv = GetContextCanvas();

            //Create
            var root = new GameObject("Translation Dropdown");
            var rootRect = root.AddComponent<RectTransform>();
            var rootDrop = root.AddComponent<TranslationDropdown>();

            var btn = new GameObject("Button");
            var btnTrans = btn.AddComponent<RectTransform>();
            var btnBtn = btn.AddComponent<Button>();
            var btnImg = btn.AddComponent<Image>();
            var btnBack = new GameObject("Background");
            var btnBackTrans = btnBack.AddComponent<RectTransform>();
            var btnBackImg = btnBack.AddComponent<Image>();
            var btnTxt = new GameObject("Text (TMP)");
            var btnTextTrans = btnTxt.AddComponent<RectTransform>();
            var btnTxtTxt = btnTxt.AddComponent<TextMeshProUGUI>();
            var btnArrow = new GameObject("Arrow");
            var btnArrowTrans = btnArrow.AddComponent<RectTransform>();
            var btnArrowImg = btnArrow.AddComponent<Image>();

            var cont = new GameObject("Content");
            var contTrans = cont.AddComponent<RectTransform>();
            var contImg = cont.AddComponent<Image>();
            var contScroll = cont.AddComponent<ScrollRect>();
            var contCanv = cont.AddComponent<Canvas>();
            var contCanvGr = cont.AddComponent<CanvasGroup>();
            var contRay = cont.AddComponent<GraphicRaycaster>();

            var vp = new GameObject("Viewport");
            var vpTrans = vp.AddComponent<RectTransform>();
            var vpMask = vp.AddComponent<Mask>();
            var vpImg = vp.AddComponent<Image>();
            var vpCt = new GameObject("Content");
            var vpCtTrans = vpCt.AddComponent<RectTransform>();
            var scrl = new GameObject("Scrollbar");
            var scrlTrans = scrl.AddComponent<RectTransform>();
            var scrlImg = scrl.AddComponent<Image>();
            var scrlScrl = scrl.AddComponent<Scrollbar>();
            var scrlSlid = new GameObject("Sliding Area");
            var scrlSlidTrans = scrlSlid.AddComponent<RectTransform>();
            var scrlHan = new GameObject("Handle");
            var scrlHanTrans = scrlHan.AddComponent<RectTransform>();
            var scrlHanImg = scrlHan.AddComponent<Image>();

            var item = new GameObject("Item");
            var itemTrans = item.AddComponent<RectTransform>();
            var itemToggle = item.AddComponent<Toggle>();
            var itemOpt = item.AddComponent<TranslationDropdownOption>();
            var itemBack = new GameObject("Item Background");
            var itemBackTrans = itemBack.AddComponent<RectTransform>();
            var itemBackImg = itemBack.AddComponent<Image>();
            var itemCheck = new GameObject("Item Checkmark");
            var itemCheckTrans = itemCheck.AddComponent<RectTransform>();
            var itemCheckImg = itemCheck.AddComponent<Image>();
            var itemLabel = new GameObject("Item Label");
            var itemLabelTrans = itemLabel.AddComponent<RectTransform>();
            var itemLabelTxt = itemLabel.AddComponent<TextMeshProUGUI>();

            //Set parents
            root.transform.SetParent(rootCanv.transform);

            btn.transform.SetParent(root.transform);
            btnBack.transform.SetParent(btn.transform);
            btnTxt.transform.SetParent(btn.transform);
            btnArrow.transform.SetParent(btn.transform);

            cont.transform.SetParent(root.transform);
            vp.transform.SetParent(cont.transform);
            vpCt.transform.SetParent(vp.transform);
            scrl.transform.SetParent(cont.transform);
            scrlSlid.transform.SetParent(scrl.transform);
            scrlHan.transform.SetParent(scrlSlid.transform);

            item.transform.SetParent(vpCt.transform);
            itemBack.transform.SetParent(item.transform);
            itemCheck.transform.SetParent(item.transform);
            itemLabel.transform.SetParent(item.transform);

            //layers
            root.layer = rootCanv.gameObject.layer;
            btn.layer = root.layer;
            btnBack.layer = root.layer;
            btnTxt.layer = root.layer;
            btnArrow.layer = root.layer;
            cont.layer = root.layer;
            vp.layer = root.layer;
            vpCt.layer = root.layer;
            scrl.layer = root.layer;
            scrlSlid.layer = root.layer;
            scrlHan.layer = root.layer;
            item.layer = root.layer;
            itemBack.layer = root.layer;
            itemCheck.layer = root.layer;
            itemLabel.layer = root.layer;

            //Transform
            rootRect.sizeDelta = new Vector2(160f, 30f);
            rootRect.localPosition = Vector2.zero;

            btnTrans.anchorMin = Vector2.zero;
            btnTrans.anchorMax = Vector2.one;
            btnTrans.sizeDelta = Vector2.zero;

            btnBackTrans.anchorMin = Vector2.zero;
            btnBackTrans.anchorMax = Vector2.one;
            btnBackTrans.sizeDelta = new Vector2(-2f, -2f);

            btnTextTrans.anchorMin = Vector2.zero;
            btnTextTrans.anchorMax = Vector2.one;
            btnTextTrans.sizeDelta = new Vector2(-35f, -5f);
            btnTextTrans.anchoredPosition = new Vector2(-15f, 0f);

            btnArrowTrans.anchorMin = new Vector2(1f, 0.5f);
            btnArrowTrans.anchorMax = new Vector2(1f, 0.5f);
            btnArrowTrans.sizeDelta = new Vector2(25f, 25f);
            btnArrowTrans.anchoredPosition = new Vector2(-17.5f, 0f);

            contTrans.anchorMin = new Vector2(0f, 0f);
            contTrans.anchorMax = new Vector2(1f, 0f);
            contTrans.pivot = new Vector2(0.5f, 1f);
            contTrans.anchoredPosition = Vector2.zero;
            contTrans.sizeDelta = new Vector2(-2f, 150f);

            contScroll.content = vpCtTrans;
            contScroll.horizontal = false;
            contScroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            contScroll.verticalScrollbarSpacing = -3f;
            contScroll.viewport = vpTrans;
            contScroll.verticalScrollbar = scrlScrl;

            contCanv.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                AdditionalCanvasShaderChannels.Normal |
                AdditionalCanvasShaderChannels.Tangent;

            EditorApplication.update += DoCanv;
            contCanvGr.interactable = true;
            contCanvGr.blocksRaycasts = true;
            contCanvGr.ignoreParentGroups = false;

            contRay.ignoreReversedGraphics = true;
            contRay.blockingObjects = GraphicRaycaster.BlockingObjects.None;

            vpTrans.anchorMin = Vector2.zero;
            vpTrans.anchorMax = Vector2.one;
            vpTrans.pivot = new Vector2(0f, 1f);
            vpTrans.sizeDelta = new Vector2(-17f, 0f);

            vpMask.showMaskGraphic = false;

            scrlTrans.anchorMin = new Vector2(1f, 0f);
            scrlTrans.anchorMax = new Vector2(1f, 1f);
            scrlTrans.pivot = new Vector2(1f, 1f);
            scrlTrans.sizeDelta = new Vector2(20f, 0f);

            scrlImg.color = dark;

            scrlScrl.targetGraphic = scrlHanImg;
            scrlScrl.handleRect = scrlHanTrans;
            scrlScrl.direction = Scrollbar.Direction.BottomToTop;
            scrlScrl.value = 0f;
            scrlScrl.size = 1f;

            scrlSlidTrans.anchorMin = Vector2.zero;
            scrlSlidTrans.anchorMax = Vector2.one;
            scrlSlidTrans.anchoredPosition = Vector2.zero;
            scrlSlidTrans.sizeDelta = new Vector2(-20f, -20f);

            scrlHanTrans.anchorMin = Vector2.zero;
            scrlHanTrans.anchorMax = Vector2.one;
            scrlHanTrans.anchoredPosition = Vector2.zero;
            scrlHanTrans.sizeDelta = new Vector2(20f, 20f);

            vpCtTrans.anchorMin = new Vector2(0f, 1f);
            vpCtTrans.anchorMax = new Vector2(1f, 1f);
            vpCtTrans.pivot = new Vector2(0.5f, 1f);
            vpCtTrans.sizeDelta = new Vector2(0f, 28f);

            itemTrans.anchorMin = new Vector2(0f, 1f);
            itemTrans.anchorMax = new Vector2(1f, 1f);
            itemTrans.anchoredPosition = Vector2.zero;
            itemTrans.pivot = new Vector2(0.5f, 1f);
            itemTrans.sizeDelta = new Vector2(0f, 20f);

            itemToggle.targetGraphic = itemBackImg;
            itemToggle.graphic = itemCheckImg;
            itemToggle.isOn = true;

            itemOpt.toggle = itemToggle;
            itemOpt.translationName = itemLabelTxt;

            itemBackTrans.anchorMin = Vector2.zero;
            itemBackTrans.anchorMax = Vector2.one;
            itemBackTrans.sizeDelta = Vector2.zero;

            itemCheckTrans.anchorMin = new Vector2(0f, 0.5f);
            itemCheckTrans.anchorMax = new Vector2(0f, 0.5f);
            itemCheckTrans.anchoredPosition = new Vector2(10f, 0f);
            itemCheckTrans.sizeDelta = new Vector2(20f, 20f);

 
            itemLabelTrans.anchorMin = Vector2.zero;
            itemLabelTrans.anchorMax = Vector2.one;
            itemLabelTrans.anchoredPosition = new Vector2(5f, -0.5f);
            itemLabelTrans.sizeDelta = new Vector2(-30f, -3f);

            //Properties
            btnImg.color = dark;

            btnBtn.targetGraphic = btnBackImg;

            btnTxtTxt.text = "Translation";
            btnTxtTxt.color = dark;
            btnTxtTxt.fontSize = 18f;
            btnTxtTxt.alignment = TextAlignmentOptions.MidlineLeft;

            btnArrowImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite> ("UI/Skin/DropdownArrow.psd");
            itemCheckImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite> ("UI/Skin/Checkmark.psd");

            itemLabelTxt.text = "English";
            itemLabelTxt.color = dark;
            itemLabelTxt.fontSize = 14f;
            itemLabelTxt.alignment = TextAlignmentOptions.MidlineLeft;

            //Assign to dropdown
            rootDrop.button = btnBtn;
            rootDrop.buttonLabel = btnTxtTxt;
            rootDrop.content = contCanv;
            rootDrop.itemTemplate = itemOpt;

            FinishObj(root);


            //We have to do this horribleness, because
            //Unity won't let us change overrideSorting
            //before it locates the root canvas
            void DoCanv()
            {
                contCanv.overrideSorting = true;
                contCanv.sortingOrder = 30000;
                cont.SetActive(false);

                EditorApplication.update -= DoCanv;
            }
        }

        [MenuItem("GameObject/Translation System/Translatable Text - TextMeshPro", false, 20)]
        public static void CreateTranslatableTMPro()
        {
            var root = new GameObject("Translatable Text (TMP)");
            var text = root.AddComponent<TextMeshPro>();
            root.AddComponent<TranslationTextLoaderTmp>();

            root.transform.SetParent(Selection.activeGameObject?.transform ?? null);

            text.text = "Translatable Text";

            FinishObj(root);
        }

        [MenuItem("GameObject/Translation System/Translatable Text (UI) - TextMeshPro", false, 21)]
        public static void CreateTranslatableTMProUI()
        {
            var canv = GetContextCanvas();

            var root = new GameObject("Translatable Text UI (TMP)");
            var trans = root.AddComponent<RectTransform>();
            var text = root.AddComponent<TextMeshProUGUI>();
            root.AddComponent<TranslationTextLoaderTmp>();

            root.transform.SetParent(canv.transform);
            trans.anchoredPosition = Vector2.zero;
            text.text = "Translatable Text";

            FinishObj(root);
        }

        [MenuItem("GameObject/Translation System/Translatable Text (UI)", false, 22)]
        public static void CreateTranslatable()
        {
            var canv = GetContextCanvas();

            var root = new GameObject("Translatable Text UI");
            var trans = root.AddComponent<RectTransform>();
            var text = root.AddComponent<Text>();
            root.AddComponent<TranslationTextLoader>();

            root.transform.SetParent(canv.transform);
            trans.anchoredPosition = Vector2.zero;
            text.text = "Translatable Text";

            FinishObj(root);
        }

        private static Canvas GetContextCanvas()
        {
            if (Selection.activeGameObject?.TryGetComponent(out Canvas sel) == true)
                return sel;

            var canv = new GameObject("Canvas");
            var canvCanv = canv.AddComponent<Canvas>();
            canv.AddComponent<CanvasScaler>();
            canv.AddComponent<GraphicRaycaster>();

            var eve = new GameObject("EventSystem");
            eve.AddComponent<EventSystem>();
            eve.AddComponent<StandaloneInputModule>();

            canv.layer = 5;
            canvCanv.renderMode = RenderMode.ScreenSpaceOverlay;

            return canvCanv;
        }

        private static void FinishObj(GameObject obj)
        {
            Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
            Selection.activeGameObject = obj;
        }
    }
}