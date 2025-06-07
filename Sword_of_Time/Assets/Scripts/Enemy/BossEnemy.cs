using UnityEngine;

public class BossEnemy : MonoBehaviour, IEnemy
{
    [Header("Attack Params")]
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [Header("Collision Params")]
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private float colliderDistance;
    [Header("Layer Param")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;
    [Header("Ranged Attack")]
    [SerializeField] private Transform firepoint;
    [SerializeField] private GameObject[] fireballs;
    private Animator animator;
    private EnemyPatrol enemyPatrol;
    private bool isRewinding = false;
    private int facingDirection = 1;
    private Transform directionHolder;
    private Health playerHealth;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        directionHolder = transform.parent;
    }
    private void Update()
    {
        facingDirection = Mathf.RoundToInt(directionHolder.localScale.x);

        if (isRewinding)
            return;
        cooldownTimer += Time.deltaTime;
        if (PlayerVisibility())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                animator.SetTrigger("Attack");
            }
        }
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = !PlayerVisibility();
        }

    }
    public void RangedAttack()
    {
        cooldownTimer = 0;
        fireballs[findFireball()].transform.position = firepoint.position;
        fireballs[findFireball()].GetComponent<EnemyProjectile>().ActivateProjectile();
    }
    private int findFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
    public bool PlayerVisibility()
    {
        Vector3 castOrigin = boxCollider.bounds.center + Vector3.right * colliderDistance * transform.localScale.x;
        Vector3 castSize = boxCollider.bounds.size;

        RaycastHit2D hit = Physics2D.BoxCast(
            castOrigin,
            castSize,
            0f,
            Vector2.zero,
            0f,
            playerLayer
        );

        if (hit.collider != null)
        {
            playerHealth = hit.collider.GetComponent<Health>();
            return true;
        }

        return false;
    }


    public void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        Gizmos.color = Color.red;
        Vector3 castOrigin = boxCollider.bounds.center + Vector3.right * colliderDistance * transform.localScale.x;
        Vector3 castSize = new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z);
        Gizmos.DrawWireCube(castOrigin, castSize);
    }

    public void OnRewindStart()
    {
        isRewinding = true;

        GetComponent<Animator>().enabled = false;
    }
    private void DamagePlayer()
    {
        if (PlayerVisibility())
        {
            playerHealth.takeDamage(attackDamage);
        }
    }
    public void OnRewindStop()
    {
        isRewinding = false;
        GetComponent<Animator>().enabled = true;
    }
}
