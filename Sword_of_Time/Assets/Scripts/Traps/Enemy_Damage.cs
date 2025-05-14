using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private BoxCollider2D triggerCollider; // Assign this in Inspector (Is Trigger = true)

    public void OnTriggerEnter2D(Collider2D collision)
    {
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
