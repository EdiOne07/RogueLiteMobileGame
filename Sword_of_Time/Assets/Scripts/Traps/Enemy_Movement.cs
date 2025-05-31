using UnityEngine;

public class Enemy_Movement : MonoBehaviour, IRewindable
{
    [SerializeField] private float damage;
    [SerializeField]private float speed;
    [SerializeField]private float movementDistance;
    private bool movingLeft;
    private float leftEdge;
    private float rightEdge;
    public bool isRewinding=false;
    private float speed_reminder = 0f;
    private void Awake()
    {
        leftEdge = transform.position.x - movementDistance;
        rightEdge= transform.position.x + movementDistance;
        speed_reminder = speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().takeDamage(damage);
        }
    }
    private void Update()
    {
        if (isRewinding)
        {
            return;
        }
        if (movingLeft) {
            if (transform.position.x > leftEdge)
            {
                transform.position = new Vector3(transform.position.x-speed*Time.deltaTime,transform.position.y,transform.position.z);
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
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else
            {
                movingLeft=true;
            }
        }   
    }

    public void OnRewindStart()
    {
        isRewinding = true;
        speed= 0;
    }

    public void OnRewindStop()
    {
        isRewinding= false;
        speed = speed_reminder;
    }
}
