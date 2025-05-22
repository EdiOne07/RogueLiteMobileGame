using UnityEngine;

public class RangedEnemy : MonoBehaviour,IRewindable
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
    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }
    private void Update()
    {
        if (isRewinding)
            return;
        cooldownTimer += Time.deltaTime;
        if (PlayerVisibility())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                animator.SetTrigger("RangedAttack");
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
        fireballs[findFireball()].transform.position=firepoint.position;
        fireballs[findFireball()].GetComponent<EnemyProjectile>().ActivateProjectile();
    }
    private int findFireball()
    {
        for(int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
    private bool PlayerVisibility()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z), 0, Vector2.left, 0, playerLayer);
        return hit.collider != null;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
    public void OnRewindStart()
    {
        isRewinding = true;
        // stop animations, sounds, etc.
        GetComponent<Animator>().enabled = false;
    }

    public void OnRewindStop()
    {
        isRewinding = false;
        GetComponent<Animator>().enabled = true;
    }
}
