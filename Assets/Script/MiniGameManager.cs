using UnityEngine;
using TMPro;

public class MiniGameManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI timerText;    // 남은 시간 표시
    public TextMeshProUGUI scoreText;    // 현재 점수 표시
    public TextMeshProUGUI highScoreText;// 최고 점수 표시

    [Header("게임 설정")]
    public float gameDuration = 30f;     // 게임 시간 (30초)

    // 게임 상태 변수
    private bool isPlaying = false;
    private float currentTime;
    private int currentScore;
    private int highScore;

    // 싱글톤 (다른 스크립트에서 쉽게 접근하기 위함)
    public static MiniGameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 저장된 최고 점수 불러오기
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();

        // 게임 시작 전엔 타이머 텍스트 초기화
        timerText.text = "Ready";
    }

    void Update()
    {
        if (isPlaying)
        {
            // 타이머 감소
            currentTime -= Time.deltaTime;

            // 시간 종료 체크
            if (currentTime <= 0)
            {
                EndGame();
            }

            // UI 갱신 (소수점 1자리까지)
            timerText.text = currentTime.ToString("F1");
        }
    }

    // 버튼에서 호출할 함수 (게임 시작)
    public void StartGame()
    {
        if (isPlaying) return; // 이미 게임 중이면 무시

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

        // 최고 점수 갱신 확인
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore); // 저장
            highScoreText.text = $"High Score: {highScore}";
        }
    }

    // 표적이 맞았을 때 호출할 함수
    public void AddScore(int amount)
    {
        // 게임 중일 때만 점수 인정
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