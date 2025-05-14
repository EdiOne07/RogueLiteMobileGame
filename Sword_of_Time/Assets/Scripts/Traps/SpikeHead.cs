using System.Runtime.CompilerServices;
using UnityEngine;

public class SpikeHead : Enemy_Damage
{
    [SerializeField] private float speed;
    private Vector3 destination;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;
    [SerializeField] private LayerMask playerLayer;
    private float checkTimer;
    private bool attacking;
    private Vector3[] directions = new Vector3[4];
    private void OnEnable()
    {
        Stop();
    }
    private void Update()
    {
        if (attacking)
        {
            transform.Translate(destination * Time.deltaTime * speed);
        }
        else
        {
            checkTimer += Time.deltaTime;
            if ((checkTimer > checkDelay))
            {
                CheckforPlayer();
            }
        }
    }
    private void CheckforPlayer()
    {
        CalculateDirections();
        for (int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i], Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);
            if (hit.collider != null && !attacking)
            {
                attacking = true;
                destination = directions[i].normalized;
                checkTimer = 0;
            }
        }
    }
    private void Stop()
    {
        destination = transform.position;
        attacking = false;
    }

    private void CalculateDirections()
    {
        directions[0] = transform.right * range;
        directions[1] = -transform.right * range;
        directions[2] = transform.up * range;
        directions[3] = -transform.up * range;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        Stop();
    }
}
