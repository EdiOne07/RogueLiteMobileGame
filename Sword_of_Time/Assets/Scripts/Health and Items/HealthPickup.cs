using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField]private float healthValue;
    [SerializeField] private AudioClip healthSound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SoundManager.instance.PlaySound(healthSound);
            collision.GetComponent<Health>().getHealth(healthValue);
            gameObject.SetActive(false);
        }
    }
}
