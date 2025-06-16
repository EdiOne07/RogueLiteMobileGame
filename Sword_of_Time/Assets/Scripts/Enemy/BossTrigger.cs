using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private EnemyPatrol bossPatrol;
    [SerializeField] private BossEnemy bossEnemy;
    [SerializeField] private GameObject wallActive;
    private void Awake()
    {
        wallActive.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && bossPatrol != null)
        {
            bossEnemy.enablePatrol();
            bossPatrol.enabled = true;
            Destroy(gameObject); 
            if(wallActive != null)
            {
                wallActive.SetActive(true);
            }
        }
    }
}
