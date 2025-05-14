using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private bool hit;
    private BoxCollider2D BoxCollider2D;
    private Animator anim;
    private float direction;
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
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);
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
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<Health>().takeDamage(1);
        }
        if (collision.tag == "Ground")
        {
            collision.gameObject.SetActive(false);
        }
    }
    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        BoxCollider2D.enabled = true;
        float localScaleX = transform.localScale.x;
        if (MathF.Sign(localScaleX) != _direction)
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
