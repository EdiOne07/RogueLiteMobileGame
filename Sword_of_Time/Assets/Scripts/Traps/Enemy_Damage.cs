using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private BoxCollider2D triggerCollider; 

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (TimeRewind.isRewinding)
        {
            return;
        }
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.takeDamage(damage);
            }
        }
    }
}
