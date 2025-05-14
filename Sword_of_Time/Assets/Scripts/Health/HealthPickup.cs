using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField]private float healthValue;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().getHealth(healthValue);
            gameObject.SetActive(false);
        }
    }
}
