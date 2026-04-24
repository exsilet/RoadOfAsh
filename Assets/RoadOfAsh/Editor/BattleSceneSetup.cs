using RoadOfAsh.Scripts.Presentation.Battle;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class BattleSceneSetup
{
    private const string CanvasName = "BattleUICanvas";
    private const string CardPrefabPath = "Assets/RoadOfAsh/Prefabs/CardView.prefab";

    [MenuItem("Tools/Road Of Ash/Build Battle UI Final")]
    public static void BuildBattleUI()
    {
        EnsureEventSystem();
        EnsureTmpFont();
        DeleteOldCanvas();

        var canvas = CreateCanvas();
        var canvasRt = canvas.GetComponent<RectTransform>();

        CreateBackground(canvasRt);

        var battleScreenRoot = CreateUIObject("BattleScreenRoot", canvasRt);
        StretchFull(battleScreenRoot);

        var battleScreen = battleScreenRoot.gameObject.AddComponent<BattleScreen>();

        var topSection = CreatePanel(
            "TopSection",
            battleScreenRoot,
            new Color(0f, 0f, 0f, 0.18f),
            new Vector2(0f, 0.70f),
            new Vector2(1f, 1f),
            Vector2.zero,
            Vector2.zero);

        var middleSection = CreatePanel(
            "MiddleSection",
            battleScreenRoot,
            new Color(0f, 0f, 0f, 0.10f),
            new Vector2(0f, 0.28f),
            new Vector2(1f, 0.70f),
            Vector2.zero,
            Vector2.zero);

        var bottomSection = CreatePanel(
            "BottomSection",
            battleScreenRoot,
            new Color(0f, 0f, 0f, 0.22f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0.28f),
            Vector2.zero,
            Vector2.zero);

        BuildTop(topSection);

        BuildMiddle(
            middleSection,
            out EnemyView enemyView,
            out TMP_Text playerHpText,
            out TMP_Text enemyHpText,
            out TMP_Text playerEnergyText,
            out TMP_Text playerBlockText
        );

        BuildBottom(
            bottomSection,
            out RectTransform handRoot,
            out Button endTurnButton
        );

        var cardPrefab = AssetDatabase.LoadAssetAtPath<CardView>(CardPrefabPath);
        if (cardPrefab == null)
        {
            Debug.LogWarning($"Card prefab not found at path: {CardPrefabPath}");
        }

        BindBattleScreenReferences(
            battleScreen,
            handRoot,
            endTurnButton,
            playerHpText,
            enemyHpText,
            playerEnergyText,
            playerBlockText,
            cardPrefab
        );

        BindEnemyViewReferences(enemyView);

        Selection.activeGameObject = canvas.gameObject;
        EditorUtility.SetDirty(canvas.gameObject);
        Debug.Log("Battle UI created.");
    }

    private static void BuildTop(RectTransform parent)
    {
        CreateTMPLabel(
            "GameTitle",
            parent,
            "СКАЗКА ВРЁТ",
            42,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(500f, 60f),
            new Vector2(0f, -32f));

        CreateTMPLabel(
            "EnemyCaption",
            parent,
            "ПРОТИВНИК",
            28,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.78f),
            new Vector2(0.5f, 0.78f),
            new Vector2(260f, 40f),
            Vector2.zero);
    }

    private static void BuildMiddle(
        RectTransform parent,
        out EnemyView enemyView,
        out TMP_Text playerHpText,
        out TMP_Text enemyHpText,
        out TMP_Text playerEnergyText,
        out TMP_Text playerBlockText)
    {
        var leftPanel = CreatePanel(
            "LeftInfoPanel",
            parent,
            new Color(0.08f, 0.08f, 0.08f, 0.75f),
            new Vector2(0.02f, 0.18f),
            new Vector2(0.22f, 0.68f),
            Vector2.zero,
            Vector2.zero);

        CreateTMPLabel(
            "KnowledgeTitle",
            leftPanel,
            "ПОНИМАНИЕ\nСКАЗКИ",
            26,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.80f),
            new Vector2(0.5f, 0.80f),
            new Vector2(220f, 70f),
            Vector2.zero);

        var knowledgeBarBg = CreatePanel(
            "KnowledgeBarBg",
            leftPanel,
            new Color(0.18f, 0.18f, 0.18f, 1f),
            new Vector2(0.10f, 0.45f),
            new Vector2(0.90f, 0.58f),
            Vector2.zero,
            Vector2.zero);

        CreatePanel(
            "KnowledgeBarFill",
            knowledgeBarBg,
            new Color(0.85f, 0.62f, 0.15f, 1f),
            new Vector2(0f, 0f),
            new Vector2(0.35f, 1f),
            Vector2.zero,
            Vector2.zero);

        CreateTMPLabel(
            "KnowledgeDesc",
            leftPanel,
            "Чем выше понимание,\nтем меньше искажений.",
            18,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.18f),
            new Vector2(0.5f, 0.18f),
            new Vector2(230f, 60f),
            Vector2.zero);

        var centerPreview = CreatePanel(
            "CenterPreview",
            parent,
            new Color(0.06f, 0.06f, 0.06f, 0.82f),
            new Vector2(0.36f, 0.26f),
            new Vector2(0.64f, 0.74f),
            Vector2.zero,
            Vector2.zero);

        CreateTMPLabel(
            "PreviewTitle",
            centerPreview,
            "СКАЗКА ИСКАЖАЕТ...",
            28,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.84f),
            new Vector2(0.5f, 0.84f),
            new Vector2(320f, 40f),
            Vector2.zero);

        CreateTMPLabel(
            "PreviewBody",
            centerPreview,
            "РУБАНУТЬ\n\nНанести 12 урона\n\nИСКАЖЕНО\n\nНанести 6 урона",
            24,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.42f),
            new Vector2(0.5f, 0.42f),
            new Vector2(300f, 200f),
            Vector2.zero);

        var rightPanel = CreatePanel(
            "EnemyPanel",
            parent,
            new Color(0.08f, 0.08f, 0.08f, 0.75f),
            new Vector2(0.76f, 0.14f),
            new Vector2(0.98f, 0.82f),
            Vector2.zero,
            Vector2.zero);

        enemyView = rightPanel.gameObject.AddComponent<EnemyView>();

        var enemyName = CreateTMPLabel(
            "EnemyNameText",
            rightPanel,
            "БАБА-ЯГА",
            28,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.85f),
            new Vector2(0.5f, 0.85f),
            new Vector2(240f, 40f),
            Vector2.zero);

        enemyHpText = CreateTMPLabel(
            "EnemyHpText",
            rightPanel,
            "HP: 20/20",
            24,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.72f),
            new Vector2(0.5f, 0.72f),
            new Vector2(220f, 36f),
            Vector2.zero);

        CreateTMPLabel(
            "EnemyRules",
            rightPanel,
            "Метки ночи\nМеняет карты местами\n\nВедьмовство\nСледующая карта искажается",
            18,
            TextAlignmentOptions.TopLeft,
            new Vector2(0.08f, 0.42f),
            new Vector2(0.92f, 0.42f),
            new Vector2(0f, 1f),
            new Vector2(0f, 0f),
            new Vector2(0f, 150f));

        BindEnemyViewReferences(enemyView, enemyName, enemyHpText);

        var playerPanel = CreatePanel(
            "PlayerPanel",
            parent,
            new Color(0.08f, 0.08f, 0.08f, 0.85f),
            new Vector2(0.32f, 0.02f),
            new Vector2(0.68f, 0.22f),
            Vector2.zero,
            Vector2.zero);

        CreateTMPLabel(
            "PlayerTitle",
            playerPanel,
            "ИГРОК",
            24,
            TextAlignmentOptions.Center,
            new Vector2(0.15f, 0.76f),
            new Vector2(0.15f, 0.76f),
            new Vector2(120f, 36f),
            Vector2.zero);

        playerHpText = CreateTMPLabel(
            "PlayerHpText",
            playerPanel,
            "HP: 30/30",
            22,
            TextAlignmentOptions.Left,
            new Vector2(0.36f, 0.72f),
            new Vector2(0.36f, 0.72f),
            new Vector2(180f, 32f),
            Vector2.zero);

        playerEnergyText = CreateTMPLabel(
            "PlayerEnergyText",
            playerPanel,
            "Энергия: 3",
            20,
            TextAlignmentOptions.Left,
            new Vector2(0.36f, 0.44f),
            new Vector2(0.36f, 0.44f),
            new Vector2(180f, 30f),
            Vector2.zero);

        playerBlockText = CreateTMPLabel(
            "PlayerBlockText",
            playerPanel,
            "Блок: 0",
            20,
            TextAlignmentOptions.Left,
            new Vector2(0.36f, 0.18f),
            new Vector2(0.36f, 0.18f),
            new Vector2(180f, 30f),
            Vector2.zero);
    }

    private static void BuildBottom(
        RectTransform parent,
        out RectTransform handRoot,
        out Button endTurnButton)
    {
        var handPanel = CreatePanel(
            "HandPanel",
            parent,
            new Color(0f, 0f, 0f, 0.14f),
            new Vector2(0.05f, 0.18f),
            new Vector2(0.78f, 0.92f),
            Vector2.zero,
            Vector2.zero);

        CreateTMPLabel(
            "HandTitle",
            handPanel,
            "КАРТЫ",
            26,
            TextAlignmentOptions.Left,
            new Vector2(0.04f, 0.92f),
            new Vector2(0.04f, 0.92f),
            new Vector2(120f, 36f),
            Vector2.zero);

        handRoot = CreateUIObject("HandRoot", handPanel);
        handRoot.anchorMin = new Vector2(0.02f, 0.02f);
        handRoot.anchorMax = new Vector2(0.98f, 0.82f);
        handRoot.offsetMin = Vector2.zero;
        handRoot.offsetMax = Vector2.zero;

        var layout = handRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 20f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.padding = new RectOffset(10, 10, 10, 10);

        var fitter = handRoot.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        var endTurnRt = CreatePanel(
            "EndTurnButton",
            parent,
            new Color(0.22f, 0.22f, 0.22f, 1f),
            new Vector2(0.83f, 0.36f),
            new Vector2(0.97f, 0.74f),
            Vector2.zero,
            Vector2.zero);

        var image = endTurnRt.GetComponent<Image>();
        endTurnButton = endTurnRt.gameObject.AddComponent<Button>();

        var colors = endTurnButton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.92f, 0.92f, 0.92f, 1f);
        colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
        endTurnButton.colors = colors;
        endTurnButton.targetGraphic = image;

        CreateTMPLabel(
            "EndTurnText",
            endTurnRt,
            "КОНЕЦ ХОДА",
            28,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(220f, 50f),
            Vector2.zero);
    }

    private static void BindBattleScreenReferences(
        BattleScreen battleScreen,
        RectTransform handRoot,
        Button endTurnButton,
        TMP_Text playerHpText,
        TMP_Text enemyHpText,
        TMP_Text playerEnergyText,
        TMP_Text playerBlockText,
        CardView cardPrefab)
    {
        var so = new SerializedObject(battleScreen);
        so.FindProperty("handRoot").objectReferenceValue = handRoot;
        so.FindProperty("endTurnButton").objectReferenceValue = endTurnButton;
        so.FindProperty("playerHpText").objectReferenceValue = playerHpText;
        so.FindProperty("enemyHpText").objectReferenceValue = enemyHpText;
        so.FindProperty("playerEnergyText").objectReferenceValue = playerEnergyText;
        so.FindProperty("playerBlockText").objectReferenceValue = playerBlockText;
        so.FindProperty("cardPrefab").objectReferenceValue = cardPrefab;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void BindEnemyViewReferences(EnemyView enemyView)
    {
        var nameText = enemyView.transform.Find("EnemyNameText")?.GetComponent<TMP_Text>();
        var hpText = enemyView.transform.Find("EnemyHpText")?.GetComponent<TMP_Text>();

        var so = new SerializedObject(enemyView);
        so.FindProperty("nameText").objectReferenceValue = nameText;
        so.FindProperty("hpText").objectReferenceValue = hpText;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void BindEnemyViewReferences(EnemyView enemyView, TMP_Text nameText, TMP_Text hpText)
    {
        var so = new SerializedObject(enemyView);
        so.FindProperty("nameText").objectReferenceValue = nameText;
        so.FindProperty("hpText").objectReferenceValue = hpText;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Canvas CreateCanvas()
    {
        var go = new GameObject(
            CanvasName,
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    private static void CreateBackground(RectTransform parent)
    {
        var bg = CreatePanel(
            "Background",
            parent,
            new Color(0.07f, 0.07f, 0.07f, 1f),
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero);

        bg.SetAsFirstSibling();
    }

    private static RectTransform CreatePanel(
        string name,
        RectTransform parent,
        Color color,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;

        var image = go.GetComponent<Image>();
        image.color = color;
        return rt;
    }

    private static TMP_Text CreateTMPLabel(
        string name,
        RectTransform parent,
        string text,
        float fontSize,
        TextAlignmentOptions alignment,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 sizeDelta,
        Vector2 anchoredPosition)
    {
        return CreateTMPLabel(
            name,
            parent,
            text,
            fontSize,
            alignment,
            anchorMin,
            anchorMax,
            new Vector2(0.5f, 0.5f),
            anchoredPosition,
            sizeDelta);
    }

    private static TMP_Text CreateTMPLabel(
        string name,
        RectTransform parent,
        string text,
        float fontSize,
        TextAlignmentOptions alignment,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 sizeDelta)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPosition;
        rt.sizeDelta = sizeDelta;

        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;
        tmp.raycastTarget = false;
        tmp.font = TMP_Settings.defaultFontAsset;

        return tmp;
    }

    private static RectTransform CreateUIObject(string name, RectTransform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        return rt;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    private static void DeleteOldCanvas()
    {
        var old = GameObject.Find(CanvasName);
        if (old != null)
        {
            Object.DestroyImmediate(old);
        }
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null)
            return;

        new GameObject(
            "EventSystem",
            typeof(EventSystem),
            typeof(StandaloneInputModule));
    }

    private static void EnsureTmpFont()
    {
        if (TMP_Settings.defaultFontAsset != null)
            return;

        Debug.LogError("TMP Default Font Asset is missing. Import TMP Essential Resources");
    }
}