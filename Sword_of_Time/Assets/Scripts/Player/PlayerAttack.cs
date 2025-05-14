using System;
using UnityEngine;

public class PlayerAttack:MonoBehaviour
{
    [SerializeField]private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireBalls;
    private Animator anim;
    private Movement playerMovement;
    private float cooldownTimer=Mathf.Infinity;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<Movement>();
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
            anim.SetTrigger("CrouchAttack");
        }
        else
        {
            anim.SetTrigger("Attack");

        }
        cooldownTimer = 0;
        fireBalls[findFireball()].transform.position = firePoint.position;
        fireBalls[findFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
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
}
