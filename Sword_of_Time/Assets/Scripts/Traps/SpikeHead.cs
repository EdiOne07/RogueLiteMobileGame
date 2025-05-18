using UnityEngine;

public class SpikeHead : Enemy_Damage
{
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float maxChaseDistance = 20f;

    private float checkTimer;
    private bool attacking;
    private Vector3 destination;
    private Vector3[] directions = new Vector3[4];
    private Rigidbody2D rb;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void OnEnable()
    {
        Stop();
    }

    private void FixedUpdate()
    {
        if (attacking)
        {
            rb.MovePosition(rb.position + (Vector2)(destination * speed * Time.fixedDeltaTime));

            if (player != null && Vector2.Distance(transform.position, player.position) > maxChaseDistance)
            {
                Stop();
            }
        }
    }

    private void Update()
    {
        if (!attacking)
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > checkDelay)
            {
                CheckForPlayer();
            }
        }
    }

    private void CheckForPlayer()
    {
        CalculateDirections();

        foreach (var dir in directions)
        {
            Debug.DrawRay(transform.position, dir, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, range, playerLayer | wallLayer);

            if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & playerLayer) != 0 && !attacking)
            {
                attacking = true;
                destination = dir.normalized;
                checkTimer = 0;
                break;
            }
        }
    }

    private void CalculateDirections()
    {
        directions[0] = transform.right * range;
        directions[1] = -transform.right * range;
        directions[2] = transform.up * range;
        directions[3] = -transform.up * range;
    }

    private void Stop()
    {
        destination = Vector3.zero;
        attacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        Stop();
    }
}
