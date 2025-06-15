using System;
using UnityEngine;

public class PlayerAttack:MonoBehaviour
{
    [SerializeField]private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireBalls;
    private PlayerAbility player;
    private Animator anim;
    private Movement playerMovement;
    private float cooldownTimer=Mathf.Infinity;
    private Vector2 originalFirePointPos;
    private float reduceCooldown = 0.25f;
    [SerializeField] private AudioClip attackSound;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<Movement>();
        player=GetComponent<PlayerAbility>();
        originalFirePointPos = firePoint.localPosition;

    }
    private void Update()
    {
        if (Input.GetMouseButton(0) && cooldownTimer>attackCooldown && playerMovement.canAttack())
        {
            Attack();
        }
        cooldownTimer += Time.deltaTime;
    }
    private void Attack()
    {
        if (isCrouching())
        {
            SoundManager.instance.PlaySound(attackSound);
            anim.SetTrigger("CrouchAttack");
        }
        else
        {
            SoundManager.instance.PlaySound(attackSound);
            anim.SetTrigger("Attack");

        }
        cooldownTimer = 0;
        float facingDirection= Mathf.Sign(transform.localScale.x);
        if (player.canShotgunShot)
        {
            firePoint.localPosition = originalFirePointPos + new Vector2(0.5f, 0f);
            Vector2[] fireDirections = new Vector2[]
        {
            new Vector2(facingDirection, 0),
            new Vector2(facingDirection,50f).normalized,
            new Vector2(facingDirection, -100f).normalized
        };
            foreach (Vector2 dir in fireDirections)
            {
                int index = findFireball();
                fireBalls[index].transform.position = firePoint.position;
                fireBalls[index].GetComponent<Projectile>().SetDirection(dir);
            }
        }
        else
        {
            int index=findFireball();
            fireBalls[findFireball()].transform.position = firePoint.position;
            fireBalls[findFireball()].GetComponent<Projectile>().SetDirection(new Vector2(facingDirection,0));
        }
            
    }
    public int findFireball()
    {
        for (int i = 0; i < fireBalls.Length; i++) {
            if (!fireBalls[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
    public Boolean isCrouching()
    {
        return Input.GetKey(KeyCode.LeftControl);
    }
    public void ModifyAttackCooldown()
    {
        attackCooldown-=reduceCooldown;
    }
}
