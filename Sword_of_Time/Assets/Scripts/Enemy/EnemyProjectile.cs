using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    private Animator animator;
    private float lifetime;
    private bool hit;
    private BoxCollider2D boxCollider2D;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D=GetComponent<BoxCollider2D>();
        
    }
    public void ActivateProjectile()
    {
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        boxCollider2D.enabled = true;
    }
    private void Update()
    {
        if (hit)
        {
            return;
        }
        float movementSpeed=speed*Time.deltaTime;
        transform.Translate(movementSpeed,0,0);
        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit=true;
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Health>().takeDamage(1);
        }
        if (animator != null) {
            animator.SetTrigger("Explode");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
