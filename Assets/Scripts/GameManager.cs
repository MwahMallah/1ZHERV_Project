using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] private CardManager cardManager;

    [SerializeField] private TrainPosition[] carriageSchemeRaw;
    private TrainPosition[,] carriageScheme;

    [SerializeField] private Camera mainCamera;


    [SerializeField] private Transform gameOverScreen;
    [SerializeField] private Transform gameWinnerScreen;
    [SerializeField] private CinemachineVirtualCamera shakingCamera;

    [SerializeField] float timeBetweenEnemyCardShow = 2f;
    [SerializeField] float timeBeforeCombat = 3f;
    [SerializeField] float timeForShowingCard = 1f;
    [SerializeField] private RectTransform menuLeftBlock;
    [SerializeField] private RectTransform menuRightBlock;
    [SerializeField] private float menuSpeed;
    [SerializeField] private Transform enemyIndicator;
    private Vector3 menuLeftBlockOrigin;
    private Vector3 menuRightBlockOrigin;

    [SerializeField] private Transform trainTransform;
    [SerializeField] float timeBeforeCardsGiven = 2f;
    [SerializeField] float trainSpeed = 1f;

    [SerializeField] private AudioSource startGameSound;
    public static GameManager Instance { get; private set; }

    private Queue<CardToPlay> actionQueue;
    private List<Enemy> enemyList;
    private int insertedCards;
    private Player player;
    private bool isPaused;
    private bool isGameStarted;
    private int originalEnemyCount;

    private Coroutine openMenuCoroutine;
    private Coroutine closeMenuCoroutine;

    public event EventHandler<OnPlayerInsertedCardEventArgs> OnPlayerInsertedCard;
    public class OnPlayerInsertedCardEventArgs : EventArgs {
        public bool slotMachineProcessed;
    }

    private void Awake() {
        Instance = this;
        isPaused = true;
        isGameStarted = false;
        actionQueue = new Queue<CardToPlay>();
        enemyList = new List<Enemy>();
        carriageScheme = new TrainPosition[2, 5];
        insertedCards = 0;
        player = Player.Instance;
        player.OnPlayerDies += ShowGameOverScreen;
        menuLeftBlockOrigin = menuLeftBlock.anchoredPosition;
        menuRightBlockOrigin = menuRightBlock.anchoredPosition;
        originalEnemyCount = 4;

        int j = 0;
        for (int i = 0; i < carriageSchemeRaw.Length; i++) {
            carriageScheme[0, j] = carriageSchemeRaw[i];
            carriageScheme[1, j++] = carriageSchemeRaw[++i];
        }
    }

    private void Start() {
        enemyManager.AddEnemiesToScene(originalEnemyCount);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isGameStarted) {
                //Game is Started
                if (isPaused) {
                    StartCoroutine(ContinueGame());
                } else {
                    StartCoroutine(PauseGame());
                }
            } else {
                //Game is not Started
                StartCoroutine(StartGame());
            }
        }
    }

    public IEnumerator StartGame() {
        isGameStarted = true;
        closeMenuCoroutine = StartCoroutine(CloseMenu());
        startGameSound.Play();
        StartCoroutine(MoveTrainToOrigin());
        yield return new WaitForSeconds(timeBeforeCardsGiven);
        isPaused = false;
        shakingCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0.1f;
        StartCoroutine(cardManager.GiveCards());
    }

    public IEnumerator ContinueGame() {
        if (openMenuCoroutine != null) {
            StopCoroutine(openMenuCoroutine);
        }
        closeMenuCoroutine = StartCoroutine(CloseMenu());
        yield return new WaitForSeconds(1.5f);
        isPaused = false;
        shakingCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0.1f;
    }

    private IEnumerator PauseGame() {
        isPaused = true;
        shakingCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
        if (closeMenuCoroutine != null) {
            StopCoroutine(closeMenuCoroutine);
        }
        openMenuCoroutine = StartCoroutine(OpenMenu());
        yield return null;
    }

    private IEnumerator CloseMenu() {
        float elapsedTime = 0f;
        Vector2 targetLeftMenuPosition = menuLeftBlockOrigin;
        targetLeftMenuPosition.x -= 1200;

        Vector2 targetRightMenuPosition = menuRightBlockOrigin;
        targetRightMenuPosition.x += 1200;

        while (menuLeftBlock.anchoredPosition != targetLeftMenuPosition && menuRightBlock.anchoredPosition != targetRightMenuPosition) {
            elapsedTime += Time.deltaTime * menuSpeed / 100;
            menuLeftBlock.anchoredPosition = Vector2.Lerp(menuLeftBlock.anchoredPosition, targetLeftMenuPosition, elapsedTime);
            menuRightBlock.anchoredPosition = Vector2.Lerp(menuRightBlock.anchoredPosition, targetRightMenuPosition, elapsedTime);
            yield return null;
        }

        yield return null;
    }

    private IEnumerator OpenMenu() {
        float elapsedTime = 0f;
        Vector2 targetLeftMenuPosition = menuLeftBlockOrigin;
        Vector2 targetRightMenuPosition = menuRightBlockOrigin;

        while (menuLeftBlock.anchoredPosition != targetLeftMenuPosition && menuRightBlock.anchoredPosition != targetRightMenuPosition) {
            elapsedTime += Time.deltaTime * menuSpeed / 100;
            menuLeftBlock.anchoredPosition = Vector2.Lerp(menuLeftBlock.anchoredPosition, targetLeftMenuPosition, elapsedTime);
            menuRightBlock.anchoredPosition = Vector2.Lerp(menuRightBlock.anchoredPosition, targetRightMenuPosition, elapsedTime);
            yield return null;
        }

        yield return null;
    }

    private IEnumerator MoveTrainToOrigin() {
        yield return new WaitForSeconds(1.5f);

        float elapsedTime = 0f;
        Vector3 targetTrainPosition = new Vector3(-1.52f, 0.7f, -0.2f);

        while (trainTransform.position != targetTrainPosition) {
            elapsedTime += Time.deltaTime * trainSpeed / 1000;
            trainTransform.position = Vector3.Lerp(trainTransform.position, targetTrainPosition, elapsedTime);
            yield return null;
        }

        //trainSound.Play();
        yield return null;
    }

    public class CardToPlay {
        public CardType.actionType actionType;
        public IPlayableByCard character;

        public CardToPlay(CardType.actionType actionType, IPlayableByCard character) {
            this.actionType = actionType;
            this.character = character;
        }
    }


    public void AddPlayerCard(CardType.actionType actionType) {
        actionQueue.Enqueue(new CardToPlay(actionType, Player.Instance));

        EnablePlayableCards(false);
        StartCoroutine(AddEnemiesCard());
        insertedCards++;
        if (insertedCards >= 3) {
            insertedCards = 0;
            EnablePlayableCards(false);
            StartCoroutine(PerformCombat());
        }
    }

    private IEnumerator AddEnemiesCard() {
        foreach (Enemy enemy in enemyList) {
            while (isPaused) {
                yield return null;
            }
            StartCoroutine(enemy.ShowCard());
            //actionQueue.Enqueue(new CardToPlay(CardType.actionType.Fight, enemy));
            if (PlayerIsFarFromEnemyHorizontally(enemy)) {
                actionQueue.Enqueue(cardManager.GetEnemyCardGreaterPossibility(enemy, CardType.actionType.MoveLeft));
            } else {
                actionQueue.Enqueue(cardManager.GetRandomEnemyCard(enemy));
            }
            yield return new WaitForSeconds(timeBetweenEnemyCardShow);
        }
        EnablePlayableCards(true);
    }

    private IEnumerator PerformCombat() {
        yield return new WaitForSeconds(timeBeforeCombat * enemyList.Count);

        while (actionQueue.Count > 0) {
            while (isPaused) {
                yield return null; 
            }
            EnablePlayableCards(false);
            CardToPlay cardToPlay = actionQueue.Dequeue();
            if (cardToPlay.character.isObjectValid()) {
                yield return StartCoroutine(PlayCard(cardToPlay));
            }
        }

        EnablePlayableCards(true);
        StartCoroutine(cardManager.GiveCards());
    }

    private IEnumerator PlayCard(CardToPlay cardToPlay) {
        while (isPaused) {
            yield return null;
        }
        CardType.actionType cardsActionType = cardToPlay.actionType;
        IPlayableByCard character = cardToPlay.character;

        EnableShowingActionCard(cardsActionType, true);
        StartCoroutine(character.ShowCard());
        yield return new WaitForSeconds(timeForShowingCard);
        EnableShowingActionCard(cardsActionType, false);
        yield return new WaitForSeconds(timeForShowingCard);

        switch (cardsActionType) {
            case CardType.actionType.MoveLeft:
                yield return StartCoroutine(character.MoveLeft());
                break;
            case CardType.actionType.MoveRight:
                yield return StartCoroutine(character.MoveRight());
                break;
            case CardType.actionType.MoveVertical:
                yield return StartCoroutine(character.MoveVertical());
                break;
            case CardType.actionType.Shoot:
                yield return StartCoroutine(character.Shoot());
                break;
            case CardType.actionType.Fight:
                yield return StartCoroutine(character.Fight());
                break;
            case CardType.actionType.Dodge:
                yield return StartCoroutine(character.Dodge());
                break;
        }

    }

    private bool PlayerIsFarFromEnemyHorizontally(Enemy enemy) {
        TrainPosition enemyTrainPosition = enemy.GetPositionInTrain();
        TrainPosition playerTrainPosition = player.GetPositionInTrain();

        return Math.Abs(enemyTrainPosition.GetHorizontalCarriage() - playerTrainPosition.GetHorizontalCarriage()) > 2;
    }
    private void ShowGameOverScreen(object sender, EventArgs e) {
        gameOverScreen.gameObject.SetActive(true);
        EnablePlayableCards(false);
        StopAllCoroutines();
    }

    private void ShowGameWinScreen() {
        gameWinnerScreen.gameObject.SetActive(true);
        StopAllCoroutines();
    }

    private void EnableShowingActionCard(CardType.actionType actionType, bool showActionCard) {
        cardManager.EnableActionCard(actionType, showActionCard);
    }

    public void RegisterEnemy(Enemy enemy) {
        enemyList.Add(enemy);
    }

    public TrainPosition GetCarriage(int verticalPos, int horizontalPos) {
        if (verticalPos < 0 || verticalPos > 1 || horizontalPos < 0 || horizontalPos > 4) {
            return null;
        } else {
            return carriageScheme[verticalPos, horizontalPos];
        }
    }

    public Transform GetRandomTrainTransform() {
        int horizontalPos = UnityEngine.Random.Range(0, 5);
        int verticalPos = UnityEngine.Random.Range(0, 2);
            
        TrainPosition randomTrainPosition = GetCarriage(verticalPos, horizontalPos);  
        return randomTrainPosition.transform;
    }

    private void EnablePlayableCards(bool show) {
        OnPlayerInsertedCard?.Invoke(this, new OnPlayerInsertedCardEventArgs { slotMachineProcessed = show});
    }

    public Camera GetMainCamera() {
        return mainCamera;
    }

    public bool IsGamePaused() {
        return isPaused;
    }

    public bool IsGameStarted() {
        return isGameStarted;
    }

    public void RemoveEnemy(Enemy enemyToRemove) {
        if (enemyList.Contains(enemyToRemove)) {
            enemyList.Remove(enemyToRemove);
        }

        for (int i = 0; i < 4 - enemyList.Count; i++) {
            enemyIndicator.GetChild(i).gameObject.SetActive(false);
        }

        if (enemyList.Count == 0) {
            ShowGameWinScreen();
        }
    }
}
