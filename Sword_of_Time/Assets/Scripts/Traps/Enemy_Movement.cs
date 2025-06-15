using UnityEngine;

public class Enemy_Movement : MonoBehaviour, IRewindable
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float movementDistance;
    [SerializeField] private Transform wallCheck; // empty GameObject in front of saw
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private LayerMask wallLayer;

    private bool movingLeft;
    private float leftEdge;
    private float rightEdge;
    public bool isRewinding = false;
    private float speed_reminder = 0f;

    private void Awake()
    {
        leftEdge = transform.position.x - movementDistance;
        rightEdge = transform.position.x + movementDistance;
        speed_reminder = speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>()?.takeDamage(damage);
        }
    }

    private void Update()
    {
        if (isRewinding) return;

        // Check for wall in front
        if (IsHittingWall())
        {
            movingLeft = !movingLeft;
        }

        if (movingLeft)
        {
            if (transform.position.x > leftEdge)
            {
                transform.position += Vector3.left * speed * Time.deltaTime;
            }
            else
            {
                movingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightEdge)
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
            }
            else
            {
                movingLeft = true;
            }
        }
    }

    private bool IsHittingWall()
    {
        Vector2 direction = movingLeft ? Vector2.left : Vector2.right;
        return Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, wallLayer);
    }

    public void OnRewindStart()
    {
        isRewinding = true;
        speed = 0;
    }

    public void OnRewindStop()
    {
        isRewinding = false;
        speed = speed_reminder;
    }
}
