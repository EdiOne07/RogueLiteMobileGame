using System;
using Ilumisoft.HealthSystem ;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private bool hit;
    private BoxCollider2D BoxCollider2D;
    private Animator anim;
    private Vector2 direction;
    private float lifetime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        BoxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if (hit)
        {
            return;
        }
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(direction*movementSpeed);
        lifetime += Time.deltaTime;
        if (lifetime > 5)
        {
            gameObject.SetActive(false);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        BoxCollider2D.enabled = false;
        anim.SetTrigger("Explode");
        if (collision.tag == "Boss")
        {
            collision.GetComponent<HealthBoss>().ApplyDamage(5);
        }
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<Health>().takeDamage(1);
        }
        if (collision.tag == "Box")
        {
            collision.gameObject.SetActive(false);
        }
    }


public void SetDirection(Vector2 _direction)
    {
        direction = _direction.normalized; // normalize to keep speed consistent
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        BoxCollider2D.enabled = true;

        // Flip sprite if needed
        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != Mathf.Sign(direction.x))
        {
            localScaleX = -localScaleX;
        }
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
}
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
