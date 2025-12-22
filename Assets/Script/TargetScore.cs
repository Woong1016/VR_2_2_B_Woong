using UnityEngine;
using TMPro;  

public class TargetScore : MonoBehaviour
{
    [Header("연결해야 할 것들")]
    public Transform centerPoint;    
    public TextMeshProUGUI scoreText; 

    public float radius10 = 0.05f;  
    public float radius9 = 0.15f;   
    public float radius8 = 0.25f;   
    public float radius7 = 0.35f;   
                                    
    private int currentScore = 0;   
    private int maxScore = 300;

    void Start()
    {
        UpdateScoreUI();
    }
    public void OnHit(Vector3 hitPoint)
    {
        if (currentScore >= maxScore) return;

        float distance = Vector3.Distance(hitPoint, centerPoint.position);
        int hitScore = 0;

        if (distance <= radius10) hitScore = 10;
        else if (distance <= radius9) hitScore = 9;
        else if (distance <= radius8) hitScore = 8;
        else if (distance <= radius7) hitScore = 7;
        else hitScore = 0;  

        if (hitScore > 0)
        {
            currentScore += hitScore;
            if (currentScore > maxScore) currentScore = maxScore;

            UpdateScoreUI();
            Debug.Log($"거리: {distance:F4}m / 점수: {hitScore}");
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{currentScore} / {maxScore}";
        }
    }
}