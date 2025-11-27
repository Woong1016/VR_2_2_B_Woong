using UnityEngine;
using TMPro; // TextMeshPro 사용

public class TargetScore : MonoBehaviour
{
    [Header("연결해야 할 것들")]
    public Transform centerPoint;   // 1단계에서 만든 CenterPoint
    public TextMeshProUGUI scoreText; // 1단계에서 만든 점수 Text

    [Header("점수 설정 (반지름 미터 단위)")]
    // 이 값들은 에디터에서 테스트하며 조절해야 합니다.
    public float radius10 = 0.05f; // 10점 원의 반지름
    public float radius9 = 0.15f;  // 9점 원의 반지름
    public float radius8 = 0.25f;  // 8점 원의 반지름
    public float radius7 = 0.35f;  // 7점 원의 반지름

    private int currentScore = 0;   // 현재 점수
    private int maxScore = 300;     // 최대 점수

    void Start()
    {
        UpdateScoreUI(); // 시작할 때 0/300 표시
    }

    // 총알이 맞았을 때 호출될 함수
    public void OnHit(Vector3 hitPoint)
    {
        // 300점이 넘으면 더 이상 점수 계산 안 함 (선택사항)
        if (currentScore >= maxScore) return;

        // 1. 맞은 위치와 중앙 사이의 거리 계산 (미터 단위)
        float distance = Vector3.Distance(hitPoint, centerPoint.position);
        int hitScore = 0;

        // 2. 거리에 따른 점수 판정
        if (distance <= radius10) hitScore = 10;
        else if (distance <= radius9) hitScore = 9;
        else if (distance <= radius8) hitScore = 8;
        else if (distance <= radius7) hitScore = 7;
        else hitScore = 0; // 너무 멀면 0점

        // 3. 점수 반영 및 UI 갱신
        if (hitScore > 0)
        {
            currentScore += hitScore;
            // 300점 초과 방지
            if (currentScore > maxScore) currentScore = maxScore;

            UpdateScoreUI();
            Debug.Log($"거리: {distance:F4}m / 점수: {hitScore}");
        }
    }

    // 점수 초기화 함수
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    // UI 텍스트 갱신
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{currentScore} / {maxScore}";
        }
    }
}