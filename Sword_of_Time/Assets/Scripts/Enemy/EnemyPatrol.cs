using System;
using System.IO;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour, IRewindable
{
    [Header("Patrol Values")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    [SerializeField] private float speed;
    private Vector3 initScale;
    private Boolean movingRight;
    [SerializeField] private float idleTime;
    private float idleTimeCounter;
    [SerializeField] private Animator animator;
    public bool isRewinding=false;
    private float speed_reminder = 0f;
    private void Awake()
    {
        initScale = enemy.localScale;
        speed_reminder = speed;
    }
    private void Update()
    {
        if (movingRight)
        {
            if (enemy.position.x <= rightEdge.position.x)
            {
                MoveInDirection(1);
            }
            else
            {
                ChangeDirection();
            }

        }
        else
        {
            if (enemy.position.x >= leftEdge.position.x)
            {
                MoveInDirection(-1);
            }
            else
            {
                ChangeDirection();
            }
        }

    }
    private void OnDisable()
    {
        animator.SetBool("Move", false);
    }
    private void ChangeDirection()
    {
        animator.SetBool("Move", false);
        idleTimeCounter += Time.deltaTime;
        if (idleTimeCounter >= idleTime)
        {
            movingRight = !movingRight;
        }

    }
    public void MoveInDirection(int _direction)
    {
      
            idleTimeCounter = 0;
            animator.SetBool("Move", true);
            enemy.localScale = new Vector3(-initScale.x * _direction, initScale.y, initScale.z);
            enemy.position = new Vector3(enemy.position.x + (Time.deltaTime * _direction * speed), enemy.position.y, enemy.position.z);

    }

    public void OnRewindStart()
    {
        isRewinding = true;
        speed = 0;
        GetComponentInChildren<Animator>().enabled = false;
    }

    public void OnRewindStop()
    {
        isRewinding=false;
        GetComponentInChildren<Animator>().enabled = true;
        speed=speed_reminder;
    }
}
