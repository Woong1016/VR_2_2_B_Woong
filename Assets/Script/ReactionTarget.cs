using UnityEngine;

public class ReactionTarget : MonoBehaviour
{
    public BoxCollider spawnArea;  
    public int scorePoint = 1;     

    public GameObject hitEffect;   

    public void OnHit()
    {
        if (MiniGameManager.instance != null)
        {
            MiniGameManager.instance.AddScore(scorePoint);
        }

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        MoveToRandomPosition();
    }

    private void MoveToRandomPosition()
    {
        if (spawnArea == null) return;
        Bounds bounds = spawnArea.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        transform.position = new Vector3(randomX, randomY, randomZ);
    }

    void OnDrawGizmos()
    {
        if (spawnArea != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);  
            Gizmos.DrawCube(spawnArea.bounds.center, spawnArea.bounds.size);
        }
    }
}