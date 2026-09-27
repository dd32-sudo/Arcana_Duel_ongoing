using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RulesScene : MonoBehaviour
{
    // =========================================================
    // 字體
    // =========================================================

    [Header("哥德字體設定")]
    public TMP_FontAsset customFont;


    // =========================================================
    // Panel
    // =========================================================

    [Header("頁面 Panel")]
    public GameObject page1Panel;
    public GameObject page2Panel;


    // =========================================================
    // 按鈕
    // =========================================================

    [Header("頁面切換與返回按鈕")]
    public Button toPage2Button;
    public Button toPage1Button;
    public Button backButton;


    // =========================================================
    // Content
    // =========================================================

    [Header("Content 容器")]
    public Transform page1ContentParent;
    public Transform page2ContentParent;


    // =========================================================
    // 顏色配置
    // =========================================================

    // 頁面大標題
    private readonly Color colorTitle =
        new Color32(255, 215, 0, 255);          // #FFD700

    // Tarot 卡牌名稱
    private readonly Color colorCardTitle =
        new Color32(255, 255, 240, 255);        // #FFFFF0

    // 正位
    private readonly Color colorUpright =
        new Color32(0, 255, 204, 255);          // #00FFCC

    // 逆位
    private readonly Color colorReversed =
        new Color32(255, 107, 107, 255);        // #FF6B6B

    // 內文
    private readonly Color colorBody =
        new Color32(245, 238, 220, 255);        // #F5EEDC

    // 次標題
    private readonly Color colorSubHeader =
        new Color32(255, 240, 165, 255);        // #FFF0A5

    // Tarot 卡片 Panel
    private readonly Color colorCardPanel =
        new Color32(26, 12, 2, 225);             // #1A0C02

    // 小區塊 Panel
    private readonly Color colorSectionPanel =
        new Color32(15, 8, 2, 210);

    // 分隔線
    private readonly Color colorDivider =
        new Color32(255, 215, 0, 110);

    // 陰影
    private readonly Color colorShadow =
        new Color32(0, 0, 0, 220);


    // =========================================================
    // 尺寸設定
    // =========================================================

    private const float TAROT_IMAGE_WIDTH = 420f;

    private const float CARD_PADDING = 30f;

    private const float CARD_SPACING = 35f;

    private const float TEXT_SPACING = 14f;

    private const float SECTION_PADDING = 22f;


    // =========================================================
    // Unity
    // =========================================================

    private void Awake()
    {
        if (customFont == null)
        {
            customFont =
                Resources.Load<TMP_FontAsset>(
                    "Fonts/JingNanBoBoHei-Bold-2 SDF"
                );
        }
    }


    private void Start()
    {
        // -----------------------------------------------------
        // 初始頁面
        // -----------------------------------------------------

        if (page1Panel != null)
            page1Panel.SetActive(true);

        if (page2Panel != null)
            page2Panel.SetActive(false);


        // -----------------------------------------------------
        // 按鈕
        // -----------------------------------------------------

        if (toPage2Button != null)
        {
            toPage2Button.onClick.AddListener(() =>
            {
                page1Panel.SetActive(false);
                page2Panel.SetActive(true);

                StartCoroutine(RefreshPage(page2ContentParent));
            });
        }

        if (toPage1Button != null)
        {
            toPage1Button.onClick.AddListener(() =>
            {
                page2Panel.SetActive(false);
                page1Panel.SetActive(true);

                StartCoroutine(RefreshPage(page1ContentParent));
            });
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBack);
        }


        // -----------------------------------------------------
        // Content Layout
        // -----------------------------------------------------

        SetupContentParent(page1ContentParent);
        SetupContentParent(page2ContentParent);


        // -----------------------------------------------------
        // 建立內容
        // -----------------------------------------------------

        PopulatePage1();
        PopulatePage2();


        // -----------------------------------------------------
        // 強制刷新
        // -----------------------------------------------------

        StartCoroutine(ForceRefreshLayout());
    }


    // =========================================================
    // Layout Refresh
    // =========================================================

    private IEnumerator ForceRefreshLayout()
    {
        yield return null;

        Canvas.ForceUpdateCanvases();

        if (page1ContentParent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                page1ContentParent as RectTransform
            );
        }

        if (page2ContentParent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                page2ContentParent as RectTransform
            );
        }

        yield return null;

        Canvas.ForceUpdateCanvases();

        if (page1ContentParent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                page1ContentParent as RectTransform
            );
        }

        if (page2ContentParent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                page2ContentParent as RectTransform
            );
        }

        Canvas.ForceUpdateCanvases();
    }


    private IEnumerator RefreshPage(Transform content)
    {
        yield return null;

        Canvas.ForceUpdateCanvases();

        if (content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                content as RectTransform
            );
        }

        yield return null;

        Canvas.ForceUpdateCanvases();
    }


    // =========================================================
    // Content Parent
    // =========================================================

    private void SetupContentParent(Transform parent)
    {
        if (parent == null)
            return;

        RectTransform rect =
            parent as RectTransform;

        if (rect == null)
            return;


        // -----------------------------------------------------
        // Vertical Layout
        // -----------------------------------------------------

        VerticalLayoutGroup vlg =
            parent.GetComponent<VerticalLayoutGroup>();

        if (vlg == null)
            vlg = parent.gameObject.AddComponent<VerticalLayoutGroup>();

        vlg.spacing = 25f;

        vlg.padding = new RectOffset(
            40,
            40,
            40,
            60
        );

        vlg.childAlignment =
            TextAnchor.UpperCenter;

        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;


        // -----------------------------------------------------
        // Content Size Fitter
        // -----------------------------------------------------

        ContentSizeFitter csf =
            parent.GetComponent<ContentSizeFitter>();

        if (csf == null)
            csf = parent.gameObject.AddComponent<ContentSizeFitter>();

        csf.horizontalFit =
            ContentSizeFitter.FitMode.Unconstrained;

        csf.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;
    }


    // =========================================================
    // 返回
    // =========================================================

    private void OnBack()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager
                .LoadScene("MainMenu");
        }
    }


    // =========================================================
    // PAGE 1
    // =========================================================

    private void PopulatePage1()
    {
        if (page1ContentParent == null)
            return;


        AddHeader(
            page1ContentParent,
            "遊戲規則 / GAME RULES"
        );


        AddInfoPanel(
            page1ContentParent,
            "遊戲說明 / GAME DESCRIPTION",
            "這是一款雙人本地對戰的塔羅格鬥遊戲。\n" +
            "This is a local 2-player Tarot fighting game.\n\n" +

            "雙方透過近戰攻擊與技能互相對抗，" +
            "率先將對方血量歸零的一方獲勝。\n" +
            "Defeat your opponent by reducing their health to zero " +
            "using melee attacks and skills.\n\n" +

            "善用場景中的漂浮平台來跳躍與躲避攻擊。\n" +
            "Utilize floating platforms to jump and dodge attacks."
        );


        AddDivider(page1ContentParent);


        AddHeader(
            page1ContentParent,
            "基礎操作 / CONTROLS"
        );


        AddControlPanel(
            page1ContentParent,
            "【 玩家一 Player 1 】",
            "移動 Move：A / D\n" +
            "跳躍 Jump：W\n" +
            "格擋 Block：S（按住 Hold）\n" +
            "攻擊 Attack：F\n" +
            "技能 Skill：G\n" +
            "大招 Ultimate：H"
        );


        AddControlPanel(
            page1ContentParent,
            "【 玩家二 Player 2 】",
            "移動 Move：← / →\n" +
            "跳躍 Jump：↑\n" +
            "格擋 Block：↓（按住 Hold）\n" +
            "攻擊 Attack：K\n" +
            "技能 Skill：L\n" +
            "大招 Ultimate：;（分號）"
        );
    }


    // =========================================================
    // PAGE 2
    // =========================================================

    private void PopulatePage2()
    {
        if (page2ContentParent == null)
            return;


        AddHeader(
            page2ContentParent,
            "塔羅牌詳解 / TAROT CARDS"
        );


        AddInfoPanel(
            page2ContentParent,
            "22 張大阿爾克那 / MAJOR ARCANA",
            "以下為 22 張大阿爾克那塔羅牌的深度占卜與正逆位含義。\n" +
            "Detailed divinatory meanings of the 22 Major Arcana cards."
        );


        AddDivider(page2ContentParent);


        List<TarotCardData> tarotDeck =
            CreateTarotDeck();


        for (int i = 0; i < tarotDeck.Count; i++)
        {
            string fileName =
                "Tarot_" + i.ToString("D2");


            Sprite sprite =
                Resources.Load<Sprite>(
                    "Tarot/" + fileName
                );


            AddTarotCard(
                tarotDeck[i],
                sprite,
                page2ContentParent
            );


            if (i < tarotDeck.Count - 1)
            {
                AddDivider(page2ContentParent);
            }
        }
    }


    // =========================================================
    // Header
    // =========================================================

    private void AddHeader(
        Transform parent,
        string text)
    {
        GameObject obj =
            CreateText(
                parent,
                text,
                58f,
                colorTitle,
                FontStyles.Bold
            );

        LayoutElement le =
            obj.GetComponent<LayoutElement>();

        le.minHeight = 80f;


        AddSpacer(parent, 15f);
    }


    // =========================================================
    // Info Panel
    // =========================================================

    private void AddInfoPanel(
        Transform parent,
        string title,
        string body)
    {
        GameObject panel =
            CreatePanel(
                parent,
                colorCardPanel
            );


        VerticalLayoutGroup layout =
            panel.AddComponent<VerticalLayoutGroup>();

        layout.padding =
            new RectOffset(
                28,
                28,
                25,
                25
            );

        layout.spacing = 12f;

        layout.childControlWidth = true;
        layout.childControlHeight = true;

        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;


        ContentSizeFitter fitter =
            panel.AddComponent<ContentSizeFitter>();

        fitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;

        fitter.horizontalFit =
            ContentSizeFitter.FitMode.Unconstrained;


        CreateText(
            panel.transform,
            title,
            38f,
            colorSubHeader,
            FontStyles.Bold
        );


        CreateText(
            panel.transform,
            body,
            32f,
            colorBody,
            FontStyles.Normal
        );


        AddSpacer(parent, 15f);
    }


    // =========================================================
    // 操作 Panel
    // =========================================================

    private void AddControlPanel(
        Transform parent,
        string title,
        string body)
    {
        GameObject panel =
            CreatePanel(
                parent,
                colorCardPanel
            );


        VerticalLayoutGroup layout =
            panel.AddComponent<VerticalLayoutGroup>();

        layout.padding =
            new RectOffset(
                30,
                30,
                25,
                25
            );

        layout.spacing = 12f;

        layout.childControlWidth = true;
        layout.childControlHeight = true;

        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;


        ContentSizeFitter fitter =
            panel.AddComponent<ContentSizeFitter>();

        fitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;


        CreateText(
            panel.transform,
            title,
            40f,
            colorSubHeader,
            FontStyles.Bold
        );


        CreateText(
            panel.transform,
            body,
            34f,
            colorBody,
            FontStyles.Normal
        );


        AddSpacer(parent, 15f);
    }


    // =========================================================
    // Tarot Card
    // =========================================================

    private void AddTarotCard(
        TarotCardData data,
        Sprite sprite,
        Transform parent)
    {
        // =====================================================
        // 最外層 Card
        // =====================================================

        GameObject card =
            CreatePanel(
                parent,
                colorCardPanel
            );


        HorizontalLayoutGroup cardLayout =
            card.AddComponent<HorizontalLayoutGroup>();


        cardLayout.padding =
            new RectOffset(
                (int)CARD_PADDING,
                (int)CARD_PADDING,
                (int)CARD_PADDING,
                (int)CARD_PADDING
            );


        cardLayout.spacing =
            CARD_SPACING;


        cardLayout.childAlignment =
            TextAnchor.UpperLeft;


        cardLayout.childControlWidth = true;
        cardLayout.childControlHeight = true;


        cardLayout.childForceExpandWidth = false;
        cardLayout.childForceExpandHeight = false;


        ContentSizeFitter cardFitter =
            card.AddComponent<ContentSizeFitter>();


        cardFitter.horizontalFit =
            ContentSizeFitter.FitMode.Unconstrained;


        cardFitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;


        LayoutElement cardLE =
            card.AddComponent<LayoutElement>();


        cardLE.flexibleWidth = 1;


        // =====================================================
        // 左邊圖片
        // =====================================================

        GameObject imageContainer =
            new GameObject(
                "TarotImageContainer",
                typeof(RectTransform)
            );


        imageContainer.transform.SetParent(
            card.transform,
            false
        );


        VerticalLayoutGroup imageLayout =
            imageContainer.AddComponent<VerticalLayoutGroup>();


        imageLayout.childAlignment =
            TextAnchor.UpperCenter;


        imageLayout.childControlWidth = true;
        imageLayout.childControlHeight = true;


        imageLayout.childForceExpandWidth = false;
        imageLayout.childForceExpandHeight = false;


        LayoutElement imageContainerLE =
            imageContainer.AddComponent<LayoutElement>();


        imageContainerLE.minWidth =
            TAROT_IMAGE_WIDTH;

        imageContainerLE.preferredWidth =
            TAROT_IMAGE_WIDTH;

        imageContainerLE.flexibleWidth = 0;


        // =====================================================
        // Tarot Image
        // =====================================================

        GameObject imageObject =
            new GameObject(
                "TarotImage",
                typeof(RectTransform)
            );


        imageObject.transform.SetParent(
            imageContainer.transform,
            false
        );


        Image image =
            imageObject.AddComponent<Image>();


        image.sprite = sprite;

        image.preserveAspect = true;

        image.raycastTarget = false;


        LayoutElement imageLE =
            imageObject.AddComponent<LayoutElement>();


        imageLE.minWidth =
            TAROT_IMAGE_WIDTH;

        imageLE.preferredWidth =
            TAROT_IMAGE_WIDTH;

        imageLE.flexibleWidth = 0;


        // -----------------------------------------------------
        // 如果沒有圖片
        // -----------------------------------------------------

        if (sprite == null)
        {
            image.color =
                new Color32(
                    120,
                    80,
                    20,
                    100
                );

            imageLE.preferredHeight = 600f;
            imageLE.minHeight = 600f;
        }


        // =====================================================
        // 右邊文字
        // =====================================================

        GameObject textColumn =
            new GameObject(
                "TarotTextColumn",
                typeof(RectTransform)
            );


        textColumn.transform.SetParent(
            card.transform,
            false
        );


        VerticalLayoutGroup textLayout =
            textColumn.AddComponent<VerticalLayoutGroup>();


        textLayout.spacing =
            TEXT_SPACING;


        textLayout.childAlignment =
            TextAnchor.UpperLeft;


        textLayout.childControlWidth = true;
        textLayout.childControlHeight = true;


        textLayout.childForceExpandWidth = true;
        textLayout.childForceExpandHeight = false;


        LayoutElement textLE =
            textColumn.AddComponent<LayoutElement>();


        textLE.flexibleWidth = 1;

        textLE.minWidth = 0;


        ContentSizeFitter textFitter =
            textColumn.AddComponent<ContentSizeFitter>();


        textFitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;


        textFitter.horizontalFit =
            ContentSizeFitter.FitMode.Unconstrained;


        // =====================================================
        // Tarot Title
        // =====================================================

        CreateText(
            textColumn.transform,
            data.title,
            46f,
            colorCardTitle,
            FontStyles.Bold
        );


        AddSmallDivider(
            textColumn.transform
        );


        // =====================================================
        // Core
        // =====================================================

        CreateLabelValue(
            textColumn.transform,
            "核心｜CORE",
            data.core,
            colorSubHeader
        );


        AddSpacer(
            textColumn.transform,
            8f
        );


        // =====================================================
        // Upright
        // =====================================================

        GameObject uprightPanel =
            CreateSectionPanel(
                textColumn.transform,
                colorUpright
            );


        VerticalLayoutGroup uprightLayout =
            uprightPanel.AddComponent<VerticalLayoutGroup>();


        uprightLayout.padding =
            new RectOffset(
                (int)SECTION_PADDING,
                (int)SECTION_PADDING,
                (int)SECTION_PADDING,
                (int)SECTION_PADDING
            );


        uprightLayout.spacing = 10f;


        uprightLayout.childControlWidth = true;
        uprightLayout.childControlHeight = true;


        uprightLayout.childForceExpandWidth = true;
        uprightLayout.childForceExpandHeight = false;


        ContentSizeFitter uprightFitter =
            uprightPanel.AddComponent<ContentSizeFitter>();


        uprightFitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;


        CreateText(
            uprightPanel.transform,
            "✦ 正位 UPRIGHT",
            36f,
            colorUpright,
            FontStyles.Bold
        );


        CreateText(
            uprightPanel.transform,
            "關鍵字｜KEYWORDS",
            28f,
            colorUpright,
            FontStyles.Bold
        );


        CreateText(
            uprightPanel.transform,
            data.uprightKeywords,
            30f,
            colorBody,
            FontStyles.Normal
        );


        CreateText(
            uprightPanel.transform,
            "含義｜MEANING",
            28f,
            colorUpright,
            FontStyles.Bold
        );


        CreateText(
            uprightPanel.transform,
            data.uprightMeaning,
            30f,
            colorBody,
            FontStyles.Normal
        );


        // =====================================================
        // Reversed
        // =====================================================

        GameObject reversedPanel =
            CreateSectionPanel(
                textColumn.transform,
                colorReversed
            );


        VerticalLayoutGroup reversedLayout =
            reversedPanel.AddComponent<VerticalLayoutGroup>();


        reversedLayout.padding =
            new RectOffset(
                (int)SECTION_PADDING,
                (int)SECTION_PADDING,
                (int)SECTION_PADDING,
                (int)SECTION_PADDING
            );


        reversedLayout.spacing = 10f;


        reversedLayout.childControlWidth = true;
        reversedLayout.childControlHeight = true;


        reversedLayout.childForceExpandWidth = true;
        reversedLayout.childForceExpandHeight = false;


        ContentSizeFitter reversedFitter =
            reversedPanel.AddComponent<ContentSizeFitter>();


        reversedFitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;


        CreateText(
            reversedPanel.transform,
            "✦ 逆位 REVERSED",
            36f,
            colorReversed,
            FontStyles.Bold
        );


        CreateText(
            reversedPanel.transform,
            "關鍵字｜KEYWORDS",
            28f,
            colorReversed,
            FontStyles.Bold
        );


        CreateText(
            reversedPanel.transform,
            data.reversedKeywords,
            30f,
            colorBody,
            FontStyles.Normal
        );


        CreateText(
            reversedPanel.transform,
            "含義｜MEANING",
            28f,
            colorReversed,
            FontStyles.Bold
        );


        CreateText(
            reversedPanel.transform,
            data.reversedMeaning,
            30f,
            colorBody,
            FontStyles.Normal
        );


        // =====================================================
        // 卡片之間間距
        // =====================================================

        AddSpacer(
            parent,
            15f
        );
    }


    // =========================================================
    // Section Panel
    // =========================================================

    private GameObject CreateSectionPanel(
        Transform parent,
        Color accentColor)
    {
        GameObject panel =
            CreatePanel(
                parent,
                colorSectionPanel
            );


        // 加一條左側色條
        GameObject accent =
            new GameObject(
                "Accent",
                typeof(RectTransform)
            );


        accent.transform.SetParent(
            panel.transform,
            false
        );


        Image accentImage =
            accent.AddComponent<Image>();


        accentImage.color =
            new Color(
                accentColor.r,
                accentColor.g,
                accentColor.b,
                0.8f
            );


        accentImage.raycastTarget = false;


        RectTransform accentRect =
            accent.GetComponent<RectTransform>();


        accentRect.anchorMin =
            new Vector2(0f, 0f);

        accentRect.anchorMax =
            new Vector2(0f, 1f);

        accentRect.pivot =
            new Vector2(0f, 0.5f);


        accentRect.anchoredPosition =
            Vector2.zero;


        accentRect.sizeDelta =
            new Vector2(
                5f,
                0f
            );


        // -----------------------------------------------------
        // 注意：
        // Accent 只是視覺元素，不參與 Layout
        // -----------------------------------------------------

        LayoutElement accentLE =
            accent.AddComponent<LayoutElement>();


        accentLE.ignoreLayout = true;


        return panel;
    }


    // =========================================================
    // Create Panel
    // =========================================================

    private GameObject CreatePanel(
        Transform parent,
        Color color)
    {
        GameObject panel =
            new GameObject(
                "Panel",
                typeof(RectTransform)
            );


        panel.transform.SetParent(
            parent,
            false
        );


        Image image =
            panel.AddComponent<Image>();


        image.color = color;

        image.raycastTarget = false;


        LayoutElement le =
            panel.AddComponent<LayoutElement>();


        le.flexibleWidth = 1;


        return panel;
    }


    // =========================================================
    // Create Text
    // =========================================================

    private GameObject CreateText(
        Transform parent,
        string text,
        float fontSize,
        Color color,
        FontStyles style)
    {
        GameObject go =
            new GameObject(
                "Text",
                typeof(RectTransform)
            );


        go.transform.SetParent(
            parent,
            false
        );


        TextMeshProUGUI tmp =
            go.AddComponent<TextMeshProUGUI>();


        if (customFont != null)
        {
            tmp.font = customFont;
        }


        tmp.text = text;

        tmp.fontSize = fontSize;

        tmp.color = color;

        tmp.fontStyle = style;


        // -----------------------------------------------------
        // 重要：
        // 讓文字自己計算高度
        // -----------------------------------------------------

        tmp.enableWordWrapping = true;

        tmp.overflowMode =
            TextOverflowModes.Overflow;


        tmp.alignment =
            TextAlignmentOptions.Left;


        tmp.raycastTarget = false;


        // -----------------------------------------------------
        // 行距
        // -----------------------------------------------------

        tmp.lineSpacing = 8f;


        // -----------------------------------------------------
        // 陰影
        // -----------------------------------------------------

        Shadow shadow =
            go.AddComponent<Shadow>();


        shadow.effectColor =
            colorShadow;


        shadow.effectDistance =
            new Vector2(
                2f,
                -2f
            );


        // -----------------------------------------------------
        // Layout
        // -----------------------------------------------------

        LayoutElement le =
            go.AddComponent<LayoutElement>();


        le.flexibleWidth = 1;

        le.minWidth = 0;


        ContentSizeFitter fitter =
            go.AddComponent<ContentSizeFitter>();


        fitter.horizontalFit =
            ContentSizeFitter.FitMode.Unconstrained;


        fitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;


        return go;
    }


    // =========================================================
    // Label + Value
    // =========================================================

    private void CreateLabelValue(
        Transform parent,
        string label,
        string value,
        Color labelColor)
    {
        CreateText(
            parent,
            label,
            28f,
            labelColor,
            FontStyles.Bold
        );


        CreateText(
            parent,
            value,
            30f,
            colorBody,
            FontStyles.Normal
        );
    }


    // =========================================================
    // Divider
    // =========================================================

    private void AddDivider(
        Transform parent)
    {
        AddSpacer(
            parent,
            12f
        );


        GameObject divider =
            new GameObject(
                "Divider",
                typeof(RectTransform)
            );


        divider.transform.SetParent(
            parent,
            false
        );


        Image image =
            divider.AddComponent<Image>();


        image.color =
            colorDivider;


        image.raycastTarget = false;


        LayoutElement le =
            divider.AddComponent<LayoutElement>();


        le.preferredHeight = 3f;

        le.minHeight = 3f;

        le.flexibleWidth = 1;


        AddSpacer(
            parent,
            18f
        );
    }


    // =========================================================
    // 小 Divider
    // =========================================================

    private void AddSmallDivider(
        Transform parent)
    {
        GameObject divider =
            new GameObject(
                "SmallDivider",
                typeof(RectTransform)
            );


        divider.transform.SetParent(
            parent,
            false
        );


        Image image =
            divider.AddComponent<Image>();


        image.color =
            new Color(
                colorTitle.r,
                colorTitle.g,
                colorTitle.b,
                0.45f
            );


        image.raycastTarget = false;


        LayoutElement le =
            divider.AddComponent<LayoutElement>();


        le.preferredHeight = 2f;

        le.minHeight = 2f;

        le.flexibleWidth = 1;
    }


    // =========================================================
    // Spacer
    // =========================================================

    private void AddSpacer(
        Transform parent,
        float height)
    {
        GameObject spacer =
            new GameObject(
                "Spacer",
                typeof(RectTransform)
            );


        spacer.transform.SetParent(
            parent,
            false
        );


        LayoutElement le =
            spacer.AddComponent<LayoutElement>();


        le.preferredHeight =
            height;


        le.minHeight =
            height;


        le.flexibleWidth = 1;
    }


    // =========================================================
    // Tarot Data
    // =========================================================

    private List<TarotCardData> CreateTarotDeck()
    {
        return new List<TarotCardData>
        {
            new TarotCardData(
                "0. 愚者 The Fool",

                "新開始、自由、冒險、未知 / New beginnings, freedom, adventure, the unknown",

                "新開始、純真、自由、spontaneity、冒險、好奇、信任、跳出舒適圈\n" +
                "New beginnings, innocence, freedom, spontaneity, adventure, curiosity, trust, taking a leap of faith.",

                "愚者代表一段旅程的起點。你可能還不知道結果會怎樣，但願意嘗試新的道路。它鼓勵你保持開放、接受未知，不要因為害怕犯錯而完全不開始。\n" +
                "The Fool represents the beginning of a journey. You may not know the outcome yet, but you are willing to explore. It encourages openness, curiosity, and taking a calculated leap into the unknown.",

                "魯莽、衝動、缺乏準備、天真、冒險過度、逃避責任\n" +
                "Recklessness, impulsiveness, lack of preparation, naivety, unnecessary risk-taking, irresponsibility.",

                "逆位不是單純「不能開始」，而是提醒你檢查自己是否在沒有考慮後果的情況下行動。也可能代表害怕開始、過度謹慎。\n" +
                "Reversed, The Fool can indicate acting without considering consequences, but it can also indicate holding yourself back because of fear."
            ),

            new TarotCardData(
                "I. 魔術師 The Magician",

                "意志、能力、創造、顯化 / Willpower, ability, creation, manifestation",

                "意志力、資源、技能、創造力、專注、溝通、行動、顯化\n" +
                "Willpower, resourcefulness, skill, creativity, concentration, communication, action, manifestation.",

                "魔術師代表「我有能力做這件事」。你手上已經存在某些資源，需要把想法轉化成實際行動。這張牌很強調主動性與運用現有能力。\n" +
                "The Magician represents having the tools and ability to turn an idea into reality. It emphasizes initiative, skill, communication, and making effective use of available resources.",

                "操控、欺騙、技巧被濫用、計畫不佳、才能未發揮\n" +
                "Manipulation, deception, misuse of skills, poor planning, untapped talent.",

                "可能是有能力卻沒有好好使用，也可能代表有人利用語言、魅力或技巧操控他人。\n" +
                "It can indicate wasted potential or poorly directed abilities, and in some contexts manipulation or deception."
            ),

            new TarotCardData(
                "II. 女祭司 The High Priestess",

                "直覺、秘密、潛意識 / Intuition, mystery, the subconscious",

                "直覺、內在智慧、神秘、秘密、潛意識、沉默、觀察\n" +
                "Intuition, inner wisdom, mystery, hidden knowledge, subconscious, silence, observation.",

                "女祭司通常不是叫你「馬上行動」，而是叫你先觀察。某些資訊可能還沒有公開，或者答案需要從自己的直覺與內在理解中尋找。\n" +
                "The High Priestess suggests listening, observing, and allowing hidden information to emerge rather than forcing immediate action.",

                "秘密、資訊隱藏、與直覺失去連結、沉默、退縮\n" +
                "Secrets, hidden information, disconnection from intuition, silence, withdrawal.",

                "可能代表忽略自己的直覺，也可能表示某件事沒有被完整說出來。\n" +
                "It may indicate ignoring intuition, withholding information, or becoming disconnected from one’s inner voice."
            ),

            new TarotCardData(
                "III. 皇后 The Empress",

                "豐盛、滋養、創造 / Abundance, nurturing, creativity",

                "豐盛、愛、創造力、自然、滋養、美、成長、感官享受\n" +
                "Abundance, love, creativity, nature, nurturing, beauty, growth, sensuality.",

                "皇后代表讓某件事情成長的能力。可以是感情、創意、事業、家庭或個人成長。她的核心不是「強迫」，而是提供適合成長的環境。\n" +
                "The Empress represents nurturing something so that it can grow—whether that is a relationship, creative project, family matter, or personal development.",

                "創意阻塞、依賴、過度付出、自我忽視、缺乏滋養\n" +
                "Creative block, dependence, over-giving, neglecting oneself, lack of nourishment.",

                "可能太過於照顧別人而忽略自己，也可能是創造力被壓抑。\n" +
                "It can indicate creative blockage, excessive dependence, or giving too much while neglecting your own needs."
            ),

            new TarotCardData(
                "IV. 皇帝 The Emperor",

                "結構、秩序、權威 / Structure, order, authority",

                "權威、結構、紀律、穩定、責任、領導、界線\n" +
                "Authority, structure, discipline, stability, responsibility, leadership, boundaries.",

                "皇帝代表建立秩序、制度與穩定性。它要求你負起責任，用理性和規劃處理問題。\n" +
                "The Emperor represents structure, discipline, leadership, responsibility, and creating stability.",

                "控制、支配、僵化、缺乏紀律、權力濫用\n" +
                "Control, domination, rigidity, lack of discipline, abuse of authority.",

                "可能表示某人控制慾太強，也可能是完全缺乏結構與自律。\n" +
                "It can indicate excessive control and domination, or the opposite problem: a lack of discipline and structure."
            ),

            new TarotCardData(
                "V. 教皇 The Hierophant",

                "傳統、信仰、制度、教導 / Tradition, belief, institutions, teaching",

                "傳統、教育、信仰、制度、規範、老師、師徒、正式承諾\n" +
                "Tradition, education, belief, institutions, conformity, mentorship, formal commitment.",

                "教皇代表既有的知識、傳統與制度。也可能代表尋求老師、專業人士或某個成熟體系的指引。\n" +
                "The Hierophant points toward established systems, traditions, education, mentorship, and conventional structures.",

                "打破傳統、個人信念、不服從、質疑制度\n" +
                "Breaking tradition, personal beliefs, nonconformity, questioning institutions.",

                "你可能開始懷疑原本接受的規則，想建立自己的價值觀，而不是照著別人的標準生活。\n" +
                "It can indicate challenging conventional beliefs and developing your own philosophy rather than simply following established rules."
            ),

            new TarotCardData(
                "VI. 戀人 The Lovers",

                "愛、選擇、價值觀 / Love, choice, alignment of values",

                "愛情、吸引、關係、選擇、和諧、價值觀一致、結合\n" +
                "Love, attraction, relationship, choice, harmony, alignment, union.",

                "戀人不只是「戀愛」。它同樣是一張關於重大選擇的牌：你選擇的道路是否與你的價值觀一致？\n" +
                "The Lovers is about romantic connection, but also meaningful choices and alignment between decisions and personal values.",

                "不和諧、價值觀不一致、關係失衡、錯誤選擇、內在衝突\n" +
                "Disharmony, misalignment, imbalance, difficult choices, inner conflict.",

                "可能是兩個人的需求不同，也可能是你知道自己真正想要什麼，卻選擇了與自己價值觀不一致的道路。\n" +
                "It can indicate relationship disharmony or a choice that conflicts with one’s values."
            ),

            new TarotCardData(
                "VII. 戰車 The Chariot",

                "意志、前進、控制 / Willpower, movement, control",

                "決心、成功、野心、行動、專注、自律、控制、前進\n" +
                "Determination, success, ambition, action, focus, self-discipline, control, progress.",

                "戰車是「我要往前走」的力量。它要求你集中力量、保持方向，即使有阻力也不要輕易放棄。\n" +
                "The Chariot represents determination, focused movement, self-discipline, and pushing forward despite obstacles.",

                "失去方向、失控、阻礙、攻擊性、強行推進、動力下降\n" +
                "Lack of direction, loss of control, obstacles, aggression, forcefulness, loss of motivation.",

                "可能不是「再努力一點」就能解決，而是需要重新確認方向。一直用力往錯的方向走，只會消耗更多。\n" +
                "It can suggest that forcing the situation is counterproductive and that a change of direction may be necessary."
            ),

            new TarotCardData(
                "VIII. 力量 Strength",

                "內在力量、勇氣、耐心 / Inner strength, courage, patience",

                "勇氣、自信、耐心、同理心、內在力量、自我控制\n" +
                "Courage, confidence, patience, compassion, inner power, self-control.",

                "力量不是靠暴力壓制，而是能夠理解並駕馭自己的情緒、本能與恐懼。\n" +
                "Strength represents calm mastery rather than brute force. It is courage combined with patience, compassion, and emotional self-control.",

                "自我懷疑、低自信、恐懼、無力感、情緒失控、過度強硬\n" +
                "Self-doubt, low confidence, fear, weakness, emotional instability, forcefulness.",

                "可能是你低估自己，也可能是在壓力下失去平衡，用強硬方式處理問題。\n" +
                "It may indicate self-doubt or difficulty managing fear and emotions."
            ),

            new TarotCardData(
                "IX. 隱者 The Hermit",

                "內省、獨處、尋找真理 / Introspection, solitude, seeking truth",

                "獨處、反思、智慧、自我探索、內在指引、尋找答案\n" +
                "Solitude, reflection, wisdom, introspection, inner guidance, soul-searching.",

                "隱者要求你暫時離開外界噪音，認真思考自己真正相信什麼、想要什麼。\n" +
                "The Hermit encourages solitude and reflection so that you can discover your own answers rather than relying entirely on external opinions.",

                "孤立、孤獨、退縮、封閉、逃避、過度思考\n" +
                "Isolation, loneliness, withdrawal, avoidance, overthinking.",

                "獨處本身沒有問題，但逆位可能表示你已經從「健康的獨處」變成「把自己與世界隔絕」。\n" +
                "Reversed, solitude may have become isolation or avoidance."
            ),

            new TarotCardData(
                "X. 命運之輪 Wheel of Fortune",

                "變化、循環、轉折 / Change, cycles, turning points",

                "轉機、變化、命運、週期、機會、幸運、轉折\n" +
                "Turning point, change, destiny, cycles, opportunity, luck.",

                "命運之輪提醒你，人生一直處於循環。現在的狀態不會永遠維持，新的變化可能正在形成。\n" +
                "The Wheel of Fortune represents cycles, turning points, changing circumstances, and opportunities.",

                "阻力、壞運、抗拒改變、重複舊循環、停滯\n" +
                "Resistance, setbacks, resistance to change, repeating cycles, stagnation.",

                "可能一直重複同一個模式，也可能是在抗拒本來就正在發生的變化。\n" +
                "It can suggest resistance to change or repeating an old cycle instead of moving through it."
            ),

            new TarotCardData(
                "XI. 正義 Justice",

                "真相、公平、因果 / Truth, fairness, consequences",

                "公平、真相、法律、責任、誠實、因果、客觀\n" +
                "Fairness, truth, law, accountability, honesty, cause and effect, objectivity.",

                "正義要求你面對事實，並承擔自己的選擇所帶來的結果。\n" +
                "Justice emphasizes truth, fairness, accountability, and the consequences of one’s choices.",

                "不公平、不誠實、偏見、逃避責任、腐敗\n" +
                "Injustice, dishonesty, bias, avoiding accountability, corruption.",

                "可能存在資訊不完整、不公平待遇，或者有人不願意承認自己的責任。\n" +
                "It may indicate unfair treatment, dishonesty, bias, or avoidance of responsibility."
            ),

            new TarotCardData(
                "XII. 吊人 The Hanged Man",

                "暫停、放下、換角度 / Pause, surrender, new perspective",

                "暫停、等待、放下控制、犧牲、新視角、接受\n" +
                "Pause, waiting, surrender, sacrifice, new perspective, acceptance.",

                "吊人不是單純「什麼都不做」，而是停止原本的推進方式，換一個角度看問題。\n" +
                "The Hanged Man represents pausing, surrendering control, and looking at a situation from a different perspective.",

                "拖延、停滯、抗拒、無謂犧牲、優柔寡斷\n" +
                "Delay, stagnation, resistance, needless sacrifice, indecision.",

                "可能已經到了應該改變方法的時候，卻仍然卡在原地。\n" +
                "It may indicate being stuck because you are resisting a necessary change in perspective."
            ),

            new TarotCardData(
                "XIII. 死神 Death",

                "結束、轉化、重生 / Ending, transformation, rebirth",

                "結束、轉化、過渡、放下、重生、新階段\n" +
                "Ending, transformation, transition, release, rebirth, new phase.",

                "在塔羅傳統解讀中，死神通常象徵某個階段、模式或身份的結束，而不是字面上的死亡。\n" +
                "Death traditionally symbolizes transformation and the ending of a phase or pattern rather than literal death.\n" +
                "它問的是：「什麼已經完成了它的作用，而你需要放下？」",

                "抗拒改變、無法放下、停留過去、延遲轉型\n" +
                "Resistance to change, inability to let go, attachment to the past, delayed transformation.",

                "你可能知道某件事已經需要改變，但仍然抓住舊狀態不放。\n" +
                "It can indicate resisting a necessary transition or holding onto something that has already reached its natural ending."
            ),

            new TarotCardData(
                "XIV. 節制 Temperance",

                "平衡、融合、適度 / Balance, integration, moderation",

                "平衡、耐心、和諧、療癒、融合、適度、調整\n" +
                "Balance, patience, harmony, healing, integration, moderation, adjustment.",

                "節制不是「什麼都不要」，而是找到適合自己的比例，把看似不同的東西整合起來。\n" +
                "Temperance is about finding the right balance, blending different elements, and avoiding extremes.",

                "失衡、過度、極端、急躁、缺乏協調\n" +
                "Imbalance, excess, extremes, impatience, lack of harmony.",

                "可能某個生活領域已經過度，例如工作太多、休息太少，或者情緒與理性完全失去平衡。\n" +
                "It indicates excess, imbalance, impatience, or difficulty integrating different parts of life."
            ),

            new TarotCardData(
                "XV. 惡魔 The Devil",

                "束縛、慾望、依附 / Attachment, desire, bondage",

                "慾望、依附、成癮、限制、物質主義、誘惑、性吸引\n" +
                "Desire, attachment, addiction, restriction, materialism, temptation, sexuality.",

                "惡魔不是簡單的「邪惡」。它更常指向讓你失去自由的東西：依賴、恐懼、慾望、成癮、權力關係或不健康模式。\n" +
                "The Devil often represents attachment and bondage—patterns, desires, fears, or dependencies that reduce one’s sense of freedom.",

                "解脫、斷開束縛、釋放、重新獲得自由、看清依附\n" +
                "Liberation, releasing attachment, breaking free, regained autonomy, recognizing unhealthy patterns.",

                "逆位常表示你開始看見自己被什麼東西控制，並且準備打破這個模式。\n" +
                "Reversed, The Devil can indicate recognizing and beginning to release unhealthy attachments."
            ),

            new TarotCardData(
                "XVI. 高塔 The Tower",

                "崩解、突變、真相揭露 / Collapse, upheaval, revelation",

                "突然改變、崩解、混亂、震撼、揭露、覺醒\n" +
                "Sudden change, upheaval, chaos, shock, revelation, awakening.",

                "高塔代表原本不穩固的結構突然被打破。它可能令人震撼，但核心並不是「災難」本身，而是虛假或不穩定的結構被揭露。\n" +
                "The Tower represents a sudden disruption that exposes or destroys an unstable structure. It can be shocking, but its deeper theme is revelation and dismantling what cannot remain as it is.",

                "抗拒改變、內在崩解、害怕改變、避免危機、延遲面對\n" +
                "Resistance to change, internal upheaval, fear of change, avoiding crisis, delayed confrontation.",

                "可能已經知道某個結構有問題，但一直試圖避免它真正崩解。\n" +
                "It can indicate resisting an inevitable transformation or trying to prevent a major disruption."
            ),

            new TarotCardData(
                "XVII. 星星 The Star",

                "希望、療癒、信念 / Hope, healing, faith",

                "希望、信念、療癒、更新、靈感、樂觀、平靜\n" +
                "Hope, faith, healing, renewal, inspiration, optimism, peace.",

                "星星代表經歷困難後重新找到希望。它不是保證事情一定成功，而是代表仍然存在繼續前進的信念與可能性。\n" +
                "The Star represents hope, renewal, healing, inspiration, and restored faith after difficulty.",

                "失望、悲觀、缺乏信念、絕望、自我懷疑\n" +
                "Disappointment, pessimism, lack of faith, despair, self-doubt.",

                "不是「希望永遠消失」，而是你目前可能看不到希望，或者對未來失去信任。\n" +
                "It often reflects difficulty accessing hope or faith rather than proving that hope is objectively absent."
            ),

            new TarotCardData(
                "XVIII. 月亮 The Moon",

                "不確定、潛意識、幻象 / Uncertainty, subconscious, illusion",

                "恐懼、焦慮、幻象、夢境、潛意識、直覺、模糊\n" +
                "Fear, anxiety, illusion, dreams, subconscious, intuition, uncertainty.",

                "月亮出現時，資訊往往不是完全透明的。你可能感受到某些東西，但還沒有足夠證據知道它究竟是真是假。\n" +
                "The Moon represents uncertainty, subconscious material, fear, dreams, and situations where appearances or assumptions may not reveal the whole truth.",

                "恐懼釋放、真相浮現、壓抑情緒、內在混亂逐漸清晰\n" +
                "Release of fear, emerging clarity, repressed emotions, inner confusion becoming clearer.",

                "逆位可能代表之前模糊的資訊逐漸明朗，也可能表示你開始面對之前一直壓抑的恐懼。\n" +
                "It can indicate confusion beginning to clear or previously repressed emotions coming to the surface."
            ),

            new TarotCardData(
                "XIX. 太陽 The Sun",

                "清晰、喜悅、生命力 / Clarity, joy, vitality",

                "成功、快樂、清晰、自信、活力、樂觀、成就、溫暖\n" +
                "Success, joy, clarity, confidence, vitality, optimism, achievement, warmth.",

                "太陽代表事情變得清楚、能量上升，以及對自身方向更有信心。\n" +
                "The Sun represents clarity, vitality, joy, confidence, success, and illumination.",

                "暫時失望、延遲、過度樂觀、活力下降、內在陰影\n" +
                "Temporary disappointment, delay, excessive optimism, reduced vitality, inner shadows.",

                "逆位並不等於「失敗」。它通常表示太陽的正面能量被遮蔽或延遲，例如明明有成果，卻無法真正感受到快樂。\n" +
                "Reversed, The Sun does not automatically mean failure. It can indicate delayed joy, reduced confidence, or difficulty fully experiencing a positive situation."
            ),

            new TarotCardData(
                "XX. 審判 Judgement",

                "覺醒、反省、召喚 / Awakening, reflection, calling",

                "覺醒、重生、自我評估、召喚、原諒、重新開始\n" +
                "Awakening, rebirth, self-evaluation, calling, absolution, renewal.",

                "審判代表重新審視過去，理解自己從過去經歷中學到了什麼，然後做出更清醒的選擇。\n" +
                "Judgement represents awakening, self-evaluation, renewal, and answering an inner calling after reflecting on the past.",

                "自我懷疑、自我批判、忽略召喚、不願學習過去的教訓\n" +
                "Self-doubt, inner criticism, ignoring the call, failure to learn from the past.",

                "你可能一直批判自己，或者明明知道過去的經驗告訴了你什麼，卻不願真正改變。\n" +
                "It can indicate excessive self-criticism, self-doubt, or refusing to learn from previous experiences."
            ),

            new TarotCardData(
                "XXI. 世界 The World",

                "完成、整合、圓滿 / Completion, integration, fulfillment",

                "完成、成就、圓滿、整合、歸屬、和諧、畢業、旅程完成\n" +
                "Completion, achievement, fulfillment, integration, belonging, harmony, graduation, completed journey.",

                "世界代表一個完整週期走到終點。長期目標、學業、事業、關係或人生階段可能完成並得到 closure。\n" +
                "The World represents completion, achievement, fulfillment, wholeness, and the successful completion of a major cycle.",

                "未完成、缺乏 closure、不完整、最後一步、空虛、延遲\n" +
                "Incomplete cycle, lack of closure, incompletion, final obstacle, emptiness, delay.",

                "你可能已經非常接近終點，但仍有某件事沒有真正處理完，因此很難產生完整的結束感。\n" +
                "Reversed, The World often points to something nearly complete but lacking closure or integration."
            )
        };
    }


    // =========================================================
    // Tarot Data Class
    // =========================================================

    [System.Serializable]
    public class TarotCardData
    {
        public string title;
        public string core;
        public string uprightKeywords;
        public string uprightMeaning;
        public string reversedKeywords;
        public string reversedMeaning;


        public TarotCardData(
            string title,
            string core,
            string uprightKeywords,
            string uprightMeaning,
            string reversedKeywords,
            string reversedMeaning)
        {
            this.title =
                title;

            this.core =
                core;

            this.uprightKeywords =
                uprightKeywords;

            this.uprightMeaning =
                uprightMeaning;

            this.reversedKeywords =
                reversedKeywords;

            this.reversedMeaning =
                reversedMeaning;
        }
    }
}