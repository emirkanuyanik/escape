using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardGame.Data;
using CardGame.Core;

public class GameBootstrapper : MonoBehaviour
{
    // ========== GAME STATE ==========
    private GameState _currentState = GameState.Menu;
    private int _playerCount = 4;
    private List<PlayerModel> _players = new List<PlayerModel>();
    private List<CardData> _centerPile = new List<CardData>();
    private int _currentPlayerIndex = 0;

    // ========== UI ROOT ==========
    private Canvas _canvas;
    private CanvasScaler _scaler;

    // ========== MENU UI ==========
    private GameObject _menuPanel;

    // ========== GAME UI ==========
    private GameObject _gamePanel;
    private Text _turnText;
    private Text _scoreText;
    private Text _centerPileCountText;
    private Text _centerTopCardText;
    private GameObject _centerTopCardVisual;
    private Image _centerTopCardBg;
    private Text _centerTopName;
    private Text _centerTopClub;
    private Transform _handContainer;
    private Text _handCountText;

    // ========== PASS DEVICE UI ==========
    private GameObject _passDevicePanel;
    private Text _passDeviceText;

    // ========== MATCH NOTIFICATION ==========
    private GameObject _matchNotification;
    private Text _matchNotificationText;

    // ========== GAME OVER UI ==========
    private GameObject _gameOverPanel;
    private Text _gameOverText;
    private Text _finalScoresText;

    // ========== CARD POOL ==========
    private List<GameObject> _cardPool = new List<GameObject>();

    private Font _font;

    void Awake()
    {
        _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (_font == null)
            _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    void Start()
    {
        BuildUI();
        ShowMenu();
    }

    // ===========================================
    //  UI CONSTRUCTION (All runtime, no prefabs)
    // ===========================================

    void BuildUI()
    {
        // Canvas
        GameObject canvasObj = new GameObject("GameCanvas");
        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _scaler = canvasObj.AddComponent<CanvasScaler>();
        _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _scaler.referenceResolution = new Vector2(1080, 1920);
        _scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        // EventSystem
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        BuildMenuPanel();
        BuildGamePanel();
        BuildPassDevicePanel();
        BuildMatchNotification();
        BuildGameOverPanel();
    }

    // ---------- MENU ----------
    void BuildMenuPanel()
    {
        _menuPanel = CreatePanel("MenuPanel", new Color(0.05f, 0.15f, 0.05f, 1f));

        // Title
        CreateText(_menuPanel.transform, "TitleText", "SUPER LIG\nKART OYUNU",
            64, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0.1f, 0.65f), new Vector2(0.9f, 0.92f));

        // Subtitle
        CreateText(_menuPanel.transform, "SubtitleText", "FB | GS | BJK | TS",
            30, new Color(1f, 0.85f, 0f), TextAnchor.MiddleCenter,
            new Vector2(0.2f, 0.58f), new Vector2(0.8f, 0.65f));

        // Player count label
        CreateText(_menuPanel.transform, "PlayerLabel", "Oyuncu Sayisi Secin:",
            36, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.55f));

        // Player count buttons
        for (int i = 2; i <= 4; i++)
        {
            float xMin = 0.1f + (i - 2) * 0.3f;
            float xMax = xMin + 0.22f;
            int count = i;

            Color btnColor = (i == 2) ? new Color(0.2f, 0.6f, 0.2f) :
                             (i == 3) ? new Color(0.6f, 0.6f, 0.1f) :
                                        new Color(0.7f, 0.3f, 0.1f);

            CreateButton(_menuPanel.transform, $"Btn_{i}P", $"{i}\nOyuncu",
                btnColor, Color.white, 32,
                new Vector2(xMin, 0.35f), new Vector2(xMax, 0.47f),
                () => OnPlayerCountSelected(count));
        }

        // Instructions
        CreateText(_menuPanel.transform, "InstructionsText",
            "KURALLAR:\n" +
            "- Kartlar esit dagitilir\n" +
            "- Siraniz geldiginde bir kart secin\n" +
            "- Attiginiz kart ortadaki ust kartla\n  ayni oyuncuysa, tum kartlari alin!\n" +
            "- En cok kart toplayan kazanir!",
            22, new Color(0.8f, 0.8f, 0.8f), TextAnchor.UpperCenter,
            new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.32f));
    }

    // ---------- GAME PANEL ----------
    void BuildGamePanel()
    {
        _gamePanel = CreatePanel("GamePanel", new Color(0.08f, 0.2f, 0.08f, 1f));

        // Turn indicator (top)
        _turnText = CreateText(_gamePanel.transform, "TurnText", "",
            36, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0f, 0.92f), new Vector2(1f, 0.98f)).GetComponent<Text>();

        // Score text
        _scoreText = CreateText(_gamePanel.transform, "ScoreText", "",
            22, new Color(0.9f, 0.9f, 0.7f), TextAnchor.UpperLeft,
            new Vector2(0.02f, 0.82f), new Vector2(0.98f, 0.92f)).GetComponent<Text>();

        // Center pile area (middle of screen)
        GameObject centerArea = CreatePanel("CenterArea", new Color(0f, 0f, 0f, 0.4f),
            _gamePanel.transform, new Vector2(0.25f, 0.47f), new Vector2(0.75f, 0.8f));

        CreateText(centerArea.transform, "CenterLabel", "ORTA PILE",
            20, new Color(0.7f, 0.7f, 0.7f), TextAnchor.UpperCenter,
            new Vector2(0f, 0.85f), new Vector2(1f, 1f));

        _centerPileCountText = CreateText(centerArea.transform, "PileCount", "0 kart",
            18, Color.white, TextAnchor.LowerCenter,
            new Vector2(0f, 0f), new Vector2(1f, 0.12f)).GetComponent<Text>();

        // Top card visual
        _centerTopCardVisual = new GameObject("CenterTopCard");
        _centerTopCardVisual.transform.SetParent(centerArea.transform, false);
        RectTransform cRect = _centerTopCardVisual.AddComponent<RectTransform>();
        cRect.anchorMin = new Vector2(0.15f, 0.15f);
        cRect.anchorMax = new Vector2(0.85f, 0.85f);
        cRect.offsetMin = Vector2.zero;
        cRect.offsetMax = Vector2.zero;
        _centerTopCardBg = _centerTopCardVisual.AddComponent<Image>();
        _centerTopCardBg.color = new Color(0.3f, 0.3f, 0.3f);

        // Card name and club on center card
        _centerTopName = CreateText(_centerTopCardVisual.transform, "CenterName", "",
            22, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0.05f, 0.4f), new Vector2(0.95f, 0.85f)).GetComponent<Text>();

        _centerTopClub = CreateText(_centerTopCardVisual.transform, "CenterClub", "",
            18, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0.05f, 0.1f), new Vector2(0.95f, 0.4f)).GetComponent<Text>();

        _centerTopCardVisual.SetActive(false);

        // Hand area (bottom)
        GameObject handArea = CreatePanel("HandArea", new Color(0f, 0f, 0f, 0.3f),
            _gamePanel.transform, new Vector2(0f, 0f), new Vector2(1f, 0.44f));

        _handCountText = CreateText(handArea.transform, "HandLabel", "",
            22, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0f, 0.88f), new Vector2(1f, 1f)).GetComponent<Text>();

        // Scrollable hand container
        GameObject scrollObj = new GameObject("HandScroll");
        scrollObj.transform.SetParent(handArea.transform, false);
        RectTransform scrollRect = scrollObj.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.01f, 0.02f);
        scrollRect.anchorMax = new Vector2(0.99f, 0.87f);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;
        ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
        scroll.horizontal = true;
        scroll.vertical = false;
        Image scrollBg = scrollObj.AddComponent<Image>();
        scrollBg.color = new Color(0, 0, 0, 0.01f);
        scrollObj.AddComponent<Mask>();

        // Content inside scroll
        GameObject contentObj = new GameObject("HandContent");
        contentObj.transform.SetParent(scrollObj.transform, false);
        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 0f);
        contentRect.anchorMax = new Vector2(0f, 1f);
        contentRect.pivot = new Vector2(0, 0.5f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        HorizontalLayoutGroup hlg = contentObj.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.padding = new RectOffset(10, 10, 5, 5);

        ContentSizeFitter csf = contentObj.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        scroll.content = contentRect;
        _handContainer = contentObj.transform;

        _gamePanel.SetActive(false);
    }

    // ---------- PASS DEVICE ----------
    void BuildPassDevicePanel()
    {
        _passDevicePanel = CreatePanel("PassDevicePanel", new Color(0.05f, 0.05f, 0.2f, 0.97f));

        _passDeviceText = CreateText(_passDevicePanel.transform, "PassText", "",
            42, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.75f)).GetComponent<Text>();

        CreateButton(_passDevicePanel.transform, "ReadyBtn", "HAZIRIM!",
            new Color(0.2f, 0.7f, 0.2f), Color.white, 40,
            new Vector2(0.2f, 0.3f), new Vector2(0.8f, 0.45f),
            OnReadyButtonPressed);

        _passDevicePanel.SetActive(false);
    }

    // ---------- MATCH NOTIFICATION ----------
    void BuildMatchNotification()
    {
        _matchNotification = CreatePanel("MatchNotify", new Color(1f, 0.85f, 0f, 0.95f));
        // The panel itself covers the screen

        _matchNotificationText = CreateText(_matchNotification.transform, "MatchText", "",
            48, new Color(0.1f, 0.1f, 0.1f), TextAnchor.MiddleCenter,
            new Vector2(0.05f, 0.3f), new Vector2(0.95f, 0.7f)).GetComponent<Text>();

        _matchNotification.SetActive(false);
    }

    // ---------- GAME OVER ----------
    void BuildGameOverPanel()
    {
        _gameOverPanel = CreatePanel("GameOverPanel", new Color(0.02f, 0.02f, 0.08f, 0.98f));

        _gameOverText = CreateText(_gameOverPanel.transform, "GOText", "OYUN BITTI!",
            52, new Color(1f, 0.85f, 0f), TextAnchor.MiddleCenter,
            new Vector2(0.05f, 0.7f), new Vector2(0.95f, 0.9f)).GetComponent<Text>();

        _finalScoresText = CreateText(_gameOverPanel.transform, "FinalScores", "",
            30, Color.white, TextAnchor.UpperCenter,
            new Vector2(0.05f, 0.25f), new Vector2(0.95f, 0.7f)).GetComponent<Text>();

        CreateButton(_gameOverPanel.transform, "PlayAgainBtn", "TEKRAR OYNA",
            new Color(0.2f, 0.7f, 0.2f), Color.white, 36,
            new Vector2(0.2f, 0.08f), new Vector2(0.8f, 0.2f),
            OnPlayAgainPressed);

        _gameOverPanel.SetActive(false);
    }

    // ===========================================
    //  UI HELPERS
    // ===========================================

    GameObject CreatePanel(string name, Color color, Transform parent = null, Vector2? anchorMin = null, Vector2? anchorMax = null)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent != null ? parent : _canvas.transform, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin ?? Vector2.zero;
        rt.anchorMax = anchorMax ?? Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        Image img = panel.AddComponent<Image>();
        img.color = color;
        return panel;
    }

    GameObject CreateText(Transform parent, string name, string content, int size, Color color, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        Text txt = obj.AddComponent<Text>();
        txt.font = _font;
        txt.fontSize = size;
        txt.color = color;
        txt.alignment = alignment;
        txt.horizontalOverflow = HorizontalWrapMode.Wrap;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        return obj;
    }

    void CreateButton(Transform parent, string name, string label, Color bgColor, Color textColor, int fontSize, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        Image img = btnObj.AddComponent<Image>();
        img.color = bgColor;
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);

        // Button text
        CreateText(btnObj.transform, name + "Text", label, fontSize, textColor, TextAnchor.MiddleCenter,
            Vector2.zero, Vector2.one);
    }

    // ===========================================
    //  GAME FLOW
    // ===========================================

    void ShowMenu()
    {
        _currentState = GameState.Menu;
        _menuPanel.SetActive(true);
        _gamePanel.SetActive(false);
        _passDevicePanel.SetActive(false);
        _gameOverPanel.SetActive(false);
        _matchNotification.SetActive(false);
    }

    void OnPlayerCountSelected(int count)
    {
        _playerCount = count;
        StartGame();
    }

    void StartGame()
    {
        _currentState = GameState.Initialization;

        // Create players
        _players.Clear();
        for (int i = 0; i < _playerCount; i++)
        {
            _players.Add(new PlayerModel(i, $"Oyuncu {i + 1}"));
        }

        // Generate & shuffle deck
        List<CardData> deck = CardData.GenerateFullDeck();
        ShuffleDeck(deck);

        // Deal cards
        int idx = 0;
        while (deck.Count > 0)
        {
            _players[idx % _playerCount].HandCards.Add(deck[0]);
            deck.RemoveAt(0);
            idx++;
        }

        // Reset center
        _centerPile.Clear();
        _currentPlayerIndex = 0;

        // Switch UI
        _menuPanel.SetActive(false);
        _gamePanel.SetActive(true);

        // Start first turn
        StartTurn();
    }

    void ShuffleDeck(List<CardData> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            CardData temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }

    void StartTurn()
    {
        _currentState = GameState.TurnStart;

        // Skip players with no cards
        int safetyCounter = 0;
        while (!_players[_currentPlayerIndex].HasCardsRemaining && safetyCounter < _playerCount)
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _playerCount;
            safetyCounter++;
        }

        // Check if game over
        bool anyHasCards = _players.Any(p => p.HasCardsRemaining);
        if (!anyHasCards)
        {
            EndGame();
            return;
        }

        // Show pass device screen
        ShowPassDevice();
    }

    void ShowPassDevice()
    {
        _currentState = GameState.PassDevice;
        _passDevicePanel.SetActive(true);
        _passDeviceText.text = $"{_players[_currentPlayerIndex].PlayerName}\nSIRAN SENDE!\n\nCihazi al ve hazir ol.";
        ClearHandCards();
    }

    void OnReadyButtonPressed()
    {
        _passDevicePanel.SetActive(false);
        _currentState = GameState.WaitingForInput;
        RefreshGameUI();
    }

    void RefreshGameUI()
    {
        PlayerModel current = _players[_currentPlayerIndex];

        // Turn text
        _turnText.text = $"SIRA: {current.PlayerName} ({current.HandCards.Count} kart)";

        // Scores
        string scoreStr = "";
        for (int i = 0; i < _players.Count; i++)
        {
            scoreStr += $"{_players[i].PlayerName}: {_players[i].Score} puan";
            if (i < _players.Count - 1) scoreStr += "  |  ";
        }
        _scoreText.text = scoreStr;

        // Center pile
        _centerPileCountText.text = $"{_centerPile.Count} kart";
        if (_centerPile.Count > 0)
        {
            CardData topCard = _centerPile[_centerPile.Count - 1];
            _centerTopCardVisual.SetActive(true);
            _centerTopCardBg.color = CardData.GetClubColor(topCard.Club);
            _centerTopName.text = topCard.DisplayName;
            _centerTopName.color = CardData.GetClubTextColor(topCard.Club);
            _centerTopClub.text = topCard.Club.ToString().ToUpper();
            _centerTopClub.color = CardData.GetClubTextColor(topCard.Club);
        }
        else
        {
            _centerTopCardVisual.SetActive(false);
        }

        // Hand
        _handCountText.text = $"Elindeki kartlar: ({current.HandCards.Count})";
        RebuildHandCards(current);
    }

    void RebuildHandCards(PlayerModel player)
    {
        ClearHandCards();

        for (int i = 0; i < player.HandCards.Count; i++)
        {
            CardData card = player.HandCards[i];
            int cardIndex = i;

            // Card root
            GameObject cardObj = new GameObject($"Card_{i}");
            cardObj.transform.SetParent(_handContainer, false);

            LayoutElement le = cardObj.AddComponent<LayoutElement>();
            le.preferredWidth = 140;
            le.preferredHeight = 200;
            le.minWidth = 140;

            // Background
            Image bg = cardObj.AddComponent<Image>();
            bg.color = CardData.GetClubColor(card.Club);

            // Make it a button
            Button btn = cardObj.AddComponent<Button>();
            btn.targetGraphic = bg;
            ColorBlock cb = btn.colors;
            cb.highlightedColor = Color.Lerp(bg.color, Color.white, 0.3f);
            cb.pressedColor = Color.Lerp(bg.color, Color.black, 0.3f);
            btn.colors = cb;
            btn.onClick.AddListener(() => OnCardSelected(cardIndex));

            // Player name
            CreateText(cardObj.transform, "Name", card.DisplayName, 16,
                CardData.GetClubTextColor(card.Club), TextAnchor.MiddleCenter,
                new Vector2(0.05f, 0.35f), new Vector2(0.95f, 0.85f));

            // Club name
            CreateText(cardObj.transform, "Club", card.Club.ToString(), 14,
                CardData.GetClubTextColor(card.Club), TextAnchor.MiddleCenter,
                new Vector2(0.05f, 0.08f), new Vector2(0.95f, 0.33f));

            // Small ID indicator at top
            CreateText(cardObj.transform, "ID", card.PlayerID, 10,
                new Color(1, 1, 1, 0.5f), TextAnchor.UpperCenter,
                new Vector2(0f, 0.88f), new Vector2(1f, 1f));

            _cardPool.Add(cardObj);
        }
    }

    void ClearHandCards()
    {
        foreach (var card in _cardPool)
        {
            if (card != null) Destroy(card);
        }
        _cardPool.Clear();
    }

    void OnCardSelected(int cardIndex)
    {
        if (_currentState != GameState.WaitingForInput) return;

        PlayerModel current = _players[_currentPlayerIndex];
        if (cardIndex < 0 || cardIndex >= current.HandCards.Count) return;

        _currentState = GameState.Resolution;

        CardData playedCard = current.HandCards[cardIndex];
        current.HandCards.RemoveAt(cardIndex);

        // Check for match
        bool isMatch = false;
        if (_centerPile.Count > 0)
        {
            CardData topCard = _centerPile[_centerPile.Count - 1];
            if (playedCard.PlayerID == topCard.PlayerID)
            {
                isMatch = true;
            }
        }

        // Add to center
        _centerPile.Add(playedCard);

        if (isMatch)
        {
            int collected = _centerPile.Count;
            current.CollectedCards.AddRange(_centerPile);
            _centerPile.Clear();
            StartCoroutine(ShowMatchNotification(current, playedCard, collected));
        }
        else
        {
            RefreshGameUI();
            StartCoroutine(ProceedAfterDelay(0.5f));
        }
    }

    IEnumerator ShowMatchNotification(PlayerModel player, CardData matchedCard, int collectedCount)
    {
        _matchNotification.SetActive(true);
        _matchNotificationText.text = $"ESLESME!\n\n{player.PlayerName}\n{matchedCard.DisplayName}\n\n{collectedCount} kart topladi!";

        RefreshGameUI();

        yield return new WaitForSeconds(2f);

        _matchNotification.SetActive(false);
        AdvanceTurn();
    }

    IEnumerator ProceedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AdvanceTurn();
    }

    void AdvanceTurn()
    {
        _currentState = GameState.TurnEnd;

        bool anyHasCards = _players.Any(p => p.HasCardsRemaining);
        if (!anyHasCards)
        {
            EndGame();
            return;
        }

        _currentPlayerIndex = (_currentPlayerIndex + 1) % _playerCount;
        StartTurn();
    }

    void EndGame()
    {
        _currentState = GameState.GameOver;
        _gamePanel.SetActive(false);
        _passDevicePanel.SetActive(false);
        _matchNotification.SetActive(false);
        _gameOverPanel.SetActive(true);

        int maxScore = _players.Max(p => p.Score);
        List<PlayerModel> winners = _players.Where(p => p.Score == maxScore).ToList();

        string scoresText = "";
        // Sort by score descending
        var sorted = _players.OrderByDescending(p => p.Score).ToList();
        for (int i = 0; i < sorted.Count; i++)
        {
            string medal = (i == 0) ? ">>> " : "    ";
            bool isWinner = sorted[i].Score == maxScore;
            scoresText += $"{medal}{sorted[i].PlayerName}: {sorted[i].Score} kart";
            if (isWinner) scoresText += " !!!";
            scoresText += "\n";
        }
        _finalScoresText.text = scoresText;

        if (winners.Count == 1)
        {
            _gameOverText.text = $"KAZANAN:\n{winners[0].PlayerName}!";
        }
        else
        {
            string names = string.Join(" ve ", winners.Select(w => w.PlayerName));
            _gameOverText.text = $"BERABERE!\n{names}";
        }
    }

    void OnPlayAgainPressed()
    {
        _gameOverPanel.SetActive(false);
        ClearHandCards();
        ShowMenu();
    }
}
