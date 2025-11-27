using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletHolePrefab;
    public float holeOffset = 0.01f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            ContactPoint contact = collision.contacts[0];

            TargetScore targetScore = collision.gameObject.GetComponent<TargetScore>();

            if (targetScore != null)
            {
                targetScore.OnHit(contact.point);
            }

            if (bulletHolePrefab != null)
            {
                Vector3 hitPosition = contact.point + (contact.normal * holeOffset);
                Quaternion hitRotation = Quaternion.LookRotation(contact.normal);

                GameObject hole = Instantiate(bulletHolePrefab, hitPosition, hitRotation * Quaternion.Euler(90, 0, 0));
                hole.transform.SetParent(collision.gameObject.transform);
            }
        }

        Destroy(gameObject);
    }
}