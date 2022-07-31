using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameController : MonoSingleton<GameController>
{
    [Header("Card")]
    [SerializeField]
    private GridLayoutGroup cardsParent;
    [SerializeField]
    private Sprite[] foregroundTextures;
    [SerializeField]
    private Sprite backgroundTexture;
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    public float cardFlipSpeed = 10.0f;

    [Header("UI")]
    [SerializeField]
    private RectTransform gamePanel;
    [SerializeField]
    private RectTransform scorePanel;
    [SerializeField]
    private TextMeshProUGUI counterText;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private float counterTime;
    private Queue<GameObject> cardPoolObjects;
    private CardClass[] cardObjects;
    private int sameCardCount = 0;
    private int matchCount = 0;
    private int firstId = -1, secondId = -1;//For compare open cards's IDs
    private CardClass firstCard, secondCard;

    [System.NonSerialized]
    public bool globalLock = false;
    private bool isPlaying = false;
    private string currentGameSize;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Awake()
    {
        cardPoolObjects = new Queue<GameObject>();
        for(int i = 0;i<20;i++){
            GameObject card = Instantiate(cardPrefab);
            card.GetComponent<CardClass>().SetValues(backgroundTexture,0,backgroundTexture);
            card.SetActive(false);
            cardPoolObjects.Enqueue(card);
        }
    }

    void Update()
    {
        if (isPlaying)
        {
            if (firstId != -1 && secondId != -1)
            {
                if (firstId == secondId)
                {
                    firstCard.onMatch();
                    secondCard.onMatch();
                    Debug.Log("Matched");
                    matchCount++;
                    if (matchCount == sameCardCount)
                    {
                        Debug.Log("You Win");
                        isPlaying = false;
                        GameStop();
                        showScore();
                    }
                }
                else
                {
                    firstCard.flipTheCard(true);
                    firstCard.Unlock();
                    secondCard.flipTheCard(true);
                    secondCard.Unlock();
                    Debug.Log("Not Matching");
                }
                firstId = secondId = -1;
            }
            counterTime += Time.deltaTime;
            counterText.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(counterTime / 60), Mathf.FloorToInt(counterTime % 60));
        }
    }

    CardClass[] shuffleCards(CardClass[] cardObjects)
    {
        for (int i = cardObjects.Length - 1; i >= 0; i--)
        {
            int index = Random.Range(0, i);
            CardClass temp = cardObjects[i];
            cardObjects[i] = cardObjects[index];
            cardObjects[i].SetParent(cardsParent.transform);
            cardObjects[i].SetLocalScale(Vector3.one);
            cardObjects[index] = temp;
        }
        return cardObjects;
    }

    Sprite[] shuffleTextures(Sprite[] textures)
    {
        for (int i = textures.Length - 1; i >= 0; i--)
        {
            int index = Random.Range(0, i);
            Sprite temp = textures[i];
            textures[i] = textures[index];
            textures[index] = temp;
        }
        return textures;
    }

    public void addIdForMatch(CardClass card)
    {
        if (firstId == -1)
        {
            firstCard = card;
            firstCard.Lock();
            firstId = card.matchId;
        }
        else if (secondId == -1)
        {
            secondCard = card;
            secondCard.Lock();
            secondId = card.matchId;
        }
    }

    public void GameStart(int sizeX, int sizeY)
    {
        currentGameSize = sizeX.ToString() + "x" + sizeY.ToString();
        matchCount = 0;
        firstId = secondId = -1;
        sameCardCount = sizeX * sizeY / 2;
        float gridWidth = cardsParent.GetComponent<RectTransform>().sizeDelta.x;
        cardsParent.GetComponent<RectTransform>().sizeDelta = new Vector2(gridWidth, gridWidth / sizeX * 320 / 240 * sizeY);
        cardsParent.cellSize = new Vector2(gridWidth / sizeX, gridWidth / sizeX * 320 / 240);
        cardObjects = new CardClass[sameCardCount * 2];
        foregroundTextures = shuffleTextures(foregroundTextures);
        for (int i = 0; i < cardObjects.Length; i++)
        {
            cardObjects[i] = cardPoolObjects.Dequeue().GetComponent<CardClass>();
            cardObjects[i].gameObject.SetActive(true);
            cardObjects[i].SetValues(foregroundTextures[i / 2], i / 2, backgroundTexture);
            cardPoolObjects.Enqueue(cardObjects[i].gameObject);
        }
        cardObjects = shuffleCards(cardObjects);
        counterTime = 0;
        isPlaying = true;
    }

    public void GameStop()
    {
        isPlaying = false;
        matchCount = 0;
        firstId = secondId = -1;
        for (int i = 0; i < cardObjects.Length; i++)
        {
            cardObjects[i].transform.SetParent(null);
            cardObjects[i].gameObject.SetActive(false);
        }
        globalLock = false;
    }

    public void GamePause()
    {
        isPlaying = false;
        globalLock = true;
    }

    public void GameResume()
    {
        isPlaying = true;
        globalLock = false;
    }

    void showScore()
    {
        float bestScore = PlayerPrefs.GetFloat((currentGameSize + "BestScore"), -1);
        if (bestScore == -1 || (bestScore > 0 && counterTime < bestScore))
        {
            PlayerPrefs.SetFloat((currentGameSize + "BestScore"), counterTime);
            string prevBestScoreStr = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(bestScore / 60), Mathf.FloorToInt(bestScore % 60));
            string bestScoreStr = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(counterTime / 60), Mathf.FloorToInt(counterTime % 60));
            scoreText.text = (prevBestScoreStr == bestScoreStr) ? "Repeated Record\n" + bestScoreStr : "New Record\n" + bestScoreStr;
        }
        else
        {
            string scoreStr = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(counterTime / 60), Mathf.FloorToInt(counterTime % 60));
            string bestScoreStr = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(bestScore / 60), Mathf.FloorToInt(bestScore % 60));
            scoreText.text = (scoreStr == bestScoreStr) ? "Repeated Record\n" + bestScoreStr : "Your Score " + scoreStr + "\nBest Score " + bestScoreStr;
        }
        MenuController.Instance.startTransition(gamePanel, scorePanel);
    }
}
