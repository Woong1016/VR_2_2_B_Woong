using UnityEngine;

public class MagazineSpawner : MonoBehaviour
{
    [Header("¼³Á¤")]
    public GameObject magazinePrefab;
    public GameObject inpimagazinePrefab;
    public Transform spawnPoint;
    public Transform inpispawnPoint;
    public float destroyTime = 0f;    

    public void SpawnMagazine()
    {
        if (magazinePrefab != null && spawnPoint != null)
        {
            GameObject newMag = Instantiate(magazinePrefab, spawnPoint.position, spawnPoint.rotation);
            if (destroyTime > 0)
            {
                Destroy(newMag, destroyTime);
            }
        }
       
    }
    public void inpiSpawnMagazine()
    {
        if (inpimagazinePrefab != null && inpispawnPoint != null)
        {
            GameObject newMag = Instantiate(inpimagazinePrefab, inpispawnPoint.position, inpispawnPoint.rotation);
            if (destroyTime > 0)
            {
                Destroy(newMag, destroyTime);
            }
        }

    }
}