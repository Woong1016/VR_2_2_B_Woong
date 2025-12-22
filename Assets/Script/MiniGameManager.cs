using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
     
    public Text timerText;     
    public Text scoreText;     
    public Text highScoreText; 

    
    public float gameDuration = 30f;      

    private bool isPlaying = false;
    private float currentTime;
    private int currentScore;
    private int highScore;

    public static MiniGameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();

        timerText.text = "Ready";
    }

    void Update()
    {
        if (isPlaying)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                EndGame();
            }

            timerText.text = currentTime.ToString("F1");
        }
    }

    public void StartGame()
    {
        if (isPlaying) return;  

        isPlaying = true;
        currentScore = 0;
        currentTime = gameDuration;

        UpdateUI();
        Debug.Log("미니게임 시작!");
    }

    // 게임 종료 처리
    private void EndGame()
    {
        isPlaying = false;
        currentTime = 0;
        timerText.text = "Finish!";

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);  
            highScoreText.text = $"High Score: {highScore}";
        }
    }

    public void AddScore(int amount)
    {
        if (isPlaying)
        {
            currentScore += amount;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {currentScore}";
        if (highScoreText != null) highScoreText.text = $"High Score: {highScore}";
    }
}