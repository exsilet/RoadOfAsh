using RoadOfAsh.Scripts.Presentation.Battle;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class CardViewPrefabBuilder
{
    private const string PrefabFolder = "Assets/RoadOfAsh/Prefabs";
    private const string PrefabPath = PrefabFolder + "/CardView.prefab";

    private const string BaseSpriteFolder = "Assets/RoadOfAsh/Art/card_ui_assets/";
    private const string DecorSpriteFolder = "Assets/RoadOfAsh/Art/card_ui_assets_v3/";

    private const string AttackBasePath = BaseSpriteFolder + "card_base_attack.png";
    private const string SkillBasePath = BaseSpriteFolder + "card_base_skill.png";
    private const string CurseBasePath = BaseSpriteFolder + "card_base_curse.png";
    private const string PowerBasePath = BaseSpriteFolder + "card_base_power.png";

    private const string HeaderBgPath = BaseSpriteFolder + "header_bg.png";
    private const string TypeBarPath = BaseSpriteFolder + "type_bar.png";
    private const string CostBadgePath = BaseSpriteFolder + "cost_badge.png";
    private const string ShadowPath = BaseSpriteFolder + "card_shadow.png";

    private const string HeaderFancyPath = DecorSpriteFolder + "header_fancy.png";
    private const string ArtFrameHeavyPath = DecorSpriteFolder + "art_frame_heavy.png";
    private const string CornerOrnamentsPath = DecorSpriteFolder + "corner_ornaments.png";
    private const string DirtOverlayPath = DecorSpriteFolder + "dirt_overlay.png";
    private const string InnerGlowPath = DecorSpriteFolder + "inner_glow.png";
    private const string VignettePath = DecorSpriteFolder + "vignette_overlay.png";

    [MenuItem("Tools/Road Of Ash/Build CardView Prefab Final")]
    public static void BuildCardViewPrefab()
    {
        EnsureFolder("Assets/RoadOfAsh");
        EnsureFolder(PrefabFolder);

        if (TMP_Settings.defaultFontAsset == null)
        {
            Debug.LogError("TMP Default Font Asset missing. Import TMP Essentials first.");
            return;
        }

        Sprite attackBase = LoadSprite(AttackBasePath);
        Sprite skillBase = LoadSprite(SkillBasePath);
        Sprite curseBase = LoadSprite(CurseBasePath);
        Sprite powerBase = LoadSprite(PowerBasePath);

        Sprite headerBg = LoadSprite(HeaderBgPath);
        Sprite typeBar = LoadSprite(TypeBarPath);
        Sprite costBadge = LoadSprite(CostBadgePath);
        Sprite shadowSprite = LoadSprite(ShadowPath);

        Sprite headerFancy = LoadSprite(HeaderFancyPath);
        Sprite artFrameHeavy = LoadSprite(ArtFrameHeavyPath);
        Sprite cornerOrnaments = LoadSprite(CornerOrnamentsPath);
        Sprite dirtOverlay = LoadSprite(DirtOverlayPath);
        Sprite innerGlow = LoadSprite(InnerGlowPath);
        Sprite vignetteOverlay = LoadSprite(VignettePath);

        if (attackBase == null || skillBase == null || curseBase == null || powerBase == null)
        {
            Debug.LogError("Base card sprites missing. Check Assets/RoadOfAsh/Art/card_ui_assets/");
            return;
        }

        if (headerBg == null || typeBar == null || costBadge == null || shadowSprite == null)
        {
            Debug.LogError("Some required base UI sprites are missing in card_ui_assets.");
            return;
        }

        if (headerFancy == null || artFrameHeavy == null || cornerOrnaments == null || dirtOverlay == null || innerGlow == null || vignetteOverlay == null)
        {
            Debug.LogError("Decorative sprites missing. Check Assets/RoadOfAsh/Art/card_ui_assets_v3/");
            return;
        }

        var root = CreateUI("CardView");
        var rootRt = root.GetComponent<RectTransform>();
        rootRt.sizeDelta = new Vector2(220, 320);

        root.AddComponent<CanvasRenderer>();

        var button = root.AddComponent<Button>();
        var layout = root.AddComponent<LayoutElement>();
        layout.preferredWidth = 220;
        layout.preferredHeight = 320;
        layout.minWidth = 220;
        layout.minHeight = 320;

        var cardView = root.AddComponent<CardView>();

        var shadow = CreateImage("Shadow", root.transform, shadowSprite, new Color(1f, 1f, 1f, 0.9f));
        Stretch(shadow.rectTransform, -2, -2, -2, -2);
        shadow.raycastTarget = false;

        var baseImage = CreateImage("Base", root.transform, attackBase, Color.white);
        Stretch(baseImage.rectTransform, 0, 0, 0, 0);

        var glow = CreateImage("InnerGlow", root.transform, innerGlow, new Color(1f, 1f, 1f, 0.9f));
        Stretch(glow.rectTransform, 0, 0, 0, 0);
        glow.raycastTarget = false;

        var header = CreateImage("HeaderBg", root.transform, headerBg, new Color(1f, 1f, 1f, 0.85f));
        SetAnchors(header.rectTransform, new Vector2(0.15f, 0.84f), new Vector2(0.88f, 0.95f));
        header.raycastTarget = false;

        var headerFancyImage = CreateImage("HeaderFancy", root.transform, headerFancy, Color.white);
        SetAnchors(headerFancyImage.rectTransform, new Vector2(0.15f, 0.84f), new Vector2(0.88f, 0.95f));
        headerFancyImage.raycastTarget = false;

        var cost = CreateImage("CostBadge", root.transform, costBadge, Color.white);
        SetAnchors(cost.rectTransform, new Vector2(0.03f, 0.835f), new Vector2(0.20f, 0.985f));
        cost.raycastTarget = false;

        var costText = CreateText("CostText", cost.transform, "1", 24, TextAlignmentOptions.Center, FontStyles.Bold);
        Stretch(costText.rectTransform, 0, 0, 0, 0);
        costText.color = new Color(0.97f, 0.93f, 0.84f, 1f);

        var titleText = CreateText("TitleText", root.transform, "Рубануть", 18, TextAlignmentOptions.Center, FontStyles.Bold);
        SetAnchors(titleText.rectTransform, new Vector2(0.21f, 0.855f), new Vector2(0.84f, 0.94f));
        titleText.color = new Color(0.95f, 0.87f, 0.70f, 1f);

        var artFrame = CreateImage("ArtFrame", root.transform, artFrameHeavy, Color.white);
        SetAnchors(artFrame.rectTransform, new Vector2(0.08f, 0.52f), new Vector2(0.92f, 0.81f));
        artFrame.raycastTarget = false;

        var artBackdrop = CreateImage("ArtBackdropImage", artFrame.transform, null, Color.white);
        Stretch(artBackdrop.rectTransform, 10, 10, 10, 10);
        artBackdrop.preserveAspect = false;
        artBackdrop.raycastTarget = false;

        var artImage = CreateImage("ArtImage", artFrame.transform, null, Color.white);
        Stretch(artImage.rectTransform, 10, 10, 10, 10);
        artImage.preserveAspect = true;
        artImage.raycastTarget = false;

        var typeBg = CreateImage("TypeBg", root.transform, typeBar, new Color(1f, 1f, 1f, 0.92f));
        SetAnchors(typeBg.rectTransform, new Vector2(0.12f, 0.405f), new Vector2(0.88f, 0.485f));
        typeBg.raycastTarget = false;

        var typeText = CreateText("TypeText", typeBg.transform, "Атака", 17, TextAlignmentOptions.Center, FontStyles.Normal);
        Stretch(typeText.rectTransform, 4, 4, 1, 1);
        typeText.color = new Color(0.93f, 0.85f, 0.67f, 1f);

        var descriptionText = CreateText("DescriptionText", root.transform, "Нанести 6 урона", 17, TextAlignmentOptions.Center, FontStyles.Normal);
        SetAnchors(descriptionText.rectTransform, new Vector2(0.12f, 0.21f), new Vector2(0.88f, 0.36f));
        descriptionText.color = Color.white;

        var corruptedText = CreateText("CorruptedText", root.transform, "Искажено", 15, TextAlignmentOptions.Center, FontStyles.Bold);
        SetAnchors(corruptedText.rectTransform, new Vector2(0.18f, 0.145f), new Vector2(0.82f, 0.205f));
        corruptedText.color = new Color(0.90f, 0.23f, 0.18f, 1f);
        corruptedText.gameObject.SetActive(false);

        var flavorText = CreateText("FlavorText", root.transform, "Сначала бей. Потом думай.", 11, TextAlignmentOptions.Center, FontStyles.Italic);
        SetAnchors(flavorText.rectTransform, new Vector2(0.11f, 0.055f), new Vector2(0.89f, 0.115f));
        flavorText.color = new Color(0.73f, 0.66f, 0.56f, 0.92f);

        var vignette = CreateImage("VignetteOverlay", root.transform, vignetteOverlay, new Color(1f, 1f, 1f, 0.72f));
        Stretch(vignette.rectTransform, 0, 0, 0, 0);
        vignette.raycastTarget = false;

        var dirt = CreateImage("DirtOverlay", root.transform, dirtOverlay, new Color(1f, 1f, 1f, 0.68f));
        Stretch(dirt.rectTransform, 0, 0, 0, 0);
        dirt.raycastTarget = false;

        var ornaments = CreateImage("CornerOrnaments", root.transform, cornerOrnaments, Color.white);
        Stretch(ornaments.rectTransform, 0, 0, 0, 0);
        ornaments.raycastTarget = false;

        BindCardView(
            cardView,
            baseImage,
            artBackdrop,
            artImage,
            costText,
            titleText,
            typeText,
            descriptionText,
            corruptedText,
            flavorText,
            button,
            attackBase,
            skillBase,
            powerBase,
            curseBase
        );

        PrefabUtility.SaveAsPrefabAssetAndConnect(root, PrefabPath, InteractionMode.UserAction);
        Object.DestroyImmediate(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"CardView prefab created: {PrefabPath}");
    }

    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static void BindCardView(
        CardView cardView,
        Image borderImage,
        Image artBackdropImage,
        Image artImage,
        TMP_Text costText,
        TMP_Text titleText,
        TMP_Text typeText,
        TMP_Text descriptionText,
        TMP_Text corruptedText,
        TMP_Text flavorText,
        Button button,
        Sprite attackBase,
        Sprite skillBase,
        Sprite powerBase,
        Sprite curseBase)
    {
        var so = new SerializedObject(cardView);

        so.FindProperty("borderImage").objectReferenceValue = borderImage;
        so.FindProperty("artBackdropImage").objectReferenceValue = artBackdropImage;
        so.FindProperty("artImage").objectReferenceValue = artImage;
        so.FindProperty("costText").objectReferenceValue = costText;
        so.FindProperty("titleText").objectReferenceValue = titleText;
        so.FindProperty("typeText").objectReferenceValue = typeText;
        so.FindProperty("descriptionText").objectReferenceValue = descriptionText;
        so.FindProperty("corruptedText").objectReferenceValue = corruptedText;
        so.FindProperty("flavorText").objectReferenceValue = flavorText;
        so.FindProperty("button").objectReferenceValue = button;

        so.FindProperty("attackBaseSprite").objectReferenceValue = attackBase;
        so.FindProperty("skillBaseSprite").objectReferenceValue = skillBase;
        so.FindProperty("powerBaseSprite").objectReferenceValue = powerBase;
        so.FindProperty("curseBaseSprite").objectReferenceValue = curseBase;

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject CreateUI(string name)
    {
        return new GameObject(name, typeof(RectTransform));
    }

    private static Image CreateImage(string name, Transform parent, Sprite sprite, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);

        var image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.type = Image.Type.Simple;
        return image;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, string text, float size, TextAlignmentOptions alignment, FontStyles fontStyle)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);

        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.font = TMP_Settings.defaultFontAsset;
        tmp.fontSize = size;
        tmp.alignment = alignment;
        tmp.fontStyle = fontStyle;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;
        tmp.raycastTarget = false;
        return tmp;
    }

    private static void SetAnchors(RectTransform rt, Vector2 min, Vector2 max)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    private static void Stretch(RectTransform rt, float left, float right, float top, float bottom)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(left, bottom);
        rt.offsetMax = new Vector2(-right, -top);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        var parts = path.Split('/');
        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);

            current = next;
        }
    }
}