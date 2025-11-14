using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletHolePrefab;
    public float holeOffset = 0.01f;  

    void OnCollisionEnter(Collision collision)
    {
               
        if (collision.gameObject.CompareTag("Target"))
        {
             

            if (bulletHolePrefab != null)
            {
                ContactPoint contact = collision.contacts[0];

               
                Vector3 hitPosition = contact.point + (contact.normal * holeOffset);
                 

                Quaternion hitRotation = Quaternion.LookRotation(contact.normal);
                GameObject hole = Instantiate(bulletHolePrefab, hitPosition, hitRotation * Quaternion.Euler(90, 0, 0));
                hole.transform.SetParent(collision.gameObject.transform);
            }
             
        }

        Destroy(gameObject);
    }
}