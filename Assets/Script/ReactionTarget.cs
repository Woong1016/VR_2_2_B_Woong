using UnityEngine;

public class ReactionTarget : MonoBehaviour
{
    [Header("설정")]
    public BoxCollider spawnArea; // 이동할 범위 박스
    public int scorePoint = 1;    // 맞췄을 때 점수

    [Header("이펙트")]
    public GameObject hitEffect; // 터지는 효과 (선택)

    private float fixedZ; // Z값 고정용

    void Start()
    {
        // 처음 배치된 Z축 깊이를 기억함
        fixedZ = transform.position.z;
    }

    // Gun.cs에서 호출
    public void OnHit()
    {
        // 1. 매니저에게 점수 추가 요청
        if (MiniGameManager.instance != null)
        {
            MiniGameManager.instance.AddScore(scorePoint);
        }

        // 2. 이펙트 생성
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // 3. 랜덤 위치로 즉시 이동
        MoveToRandomPosition();
    }

    private void MoveToRandomPosition()
    {
        if (spawnArea == null) return;

        Bounds bounds = spawnArea.bounds;

        // X, Y는 랜덤, Z는 고정
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        transform.position = new Vector3(randomX, randomY, fixedZ);
    }
}