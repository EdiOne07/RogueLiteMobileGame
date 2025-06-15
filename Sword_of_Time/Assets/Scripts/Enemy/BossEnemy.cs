using UnityEngine;

public class BossEnemy : MonoBehaviour, IEnemy
{
    [Header("Attack Params")]
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float defaultRange;
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
    private Transform directionHolder;
    private Health playerHealth;
    public bool patrolEnable=false;
    private enum AttackType { Melee, Ranged, DashAttack }
    private AttackType chosenAttack;
    [SerializeField] private TrailRenderer dashTrail;
    [Header("SFX")]
    [SerializeField] private AudioClip meleeSound;
    [SerializeField] private AudioClip rangedSound;
    [SerializeField] private AudioClip windDashSound;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        directionHolder = transform.parent;
    }
    private void Update()
    {

        if (isRewinding)
            return;
        if (!patrolEnable)
        {
            return;
        }
        cooldownTimer += Time.deltaTime;
        if (PlayerVisibility())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;

                // Randomly choose an attack
                float attackRoll = Random.value;

                if (attackRoll < 0.33f)
                    chosenAttack = AttackType.Melee;
               else if (attackRoll < 0.66f)
                   chosenAttack = AttackType.Ranged;
                else
                    chosenAttack = AttackType.DashAttack;


                switch (chosenAttack)
                {
                    case AttackType.Melee:
                        range = defaultRange;
                        SoundManager.instance.PlaySound(meleeSound);
                        animator.SetTrigger("Attack");
                        break;

                    case AttackType.Ranged:
                        range = defaultRange * 2.5f;
                        SoundManager.instance.PlaySound(rangedSound);
                        animator.SetTrigger("RangedAttack");
                        break;

                    case AttackType.DashAttack:
                        range = defaultRange * 2f; // Dash has some range to detect
                        SoundManager.instance.PlaySound(meleeSound);
                        SoundManager.instance.PlaySound(windDashSound);
                        animator.SetTrigger("DashAttack");
                        break;
                }
  
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
    public void DamagePlayer()
    {
        if (PlayerVisibility())
        {
            playerHealth.takeDamage(attackDamage);
        }
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
    public void PerformDash()
    {
        if (playerHealth == null)
            return;

        Transform playerTransform = playerHealth.transform;

        float playerDirection = Mathf.Sign(playerTransform.localScale.x);
        Vector3 offset = new Vector3(-1.5f * playerDirection, 0f, 0f);
        Vector3 targetPosition = playerTransform.position + offset;

        RaycastHit2D hit = Physics2D.Raycast(targetPosition, Vector2.down, 5f, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            targetPosition.y = hit.point.y + GetComponent<Collider2D>().bounds.extents.y;
        }

        transform.position = targetPosition;

       
        float bossDirection = (playerTransform.position.x > transform.position.x) ? 1f : -1f;
        directionHolder.localScale = new Vector3(
            bossDirection,
            directionHolder.localScale.y,
            directionHolder.localScale.z
        );

        if (enemyPatrol != null)
        {
            enemyPatrol.SetFacingDirection(bossDirection);
        }
    }


    public void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        Gizmos.color = chosenAttack switch
        {
            AttackType.Ranged => Color.cyan,
            AttackType.DashAttack => Color.green,
            _ => Color.red
        };


        Vector3 castOrigin = boxCollider.bounds.center + Vector3.right * colliderDistance * transform.localScale.x;
        Vector3 castSize = new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z);
        Gizmos.DrawWireCube(castOrigin, castSize);
    }



    public void OnRewindStart()
    {
        isRewinding = true;

        GetComponent<Animator>().enabled = false;
    }
   
    public void OnRewindStop()
    {
        isRewinding = false;
        GetComponent<Animator>().enabled = true;
    }
    public void enablePatrol()
    {
        patrolEnable = true;
    }
}
