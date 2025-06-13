using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private EnemyPatrol bossPatrol;
    [SerializeField] private BossEnemy bossEnemy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && bossPatrol != null)
        {
            bossEnemy.enablePatrol();
            bossPatrol.enabled = true;
            Destroy(gameObject); 
        }
    }
}
