using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator animator;
    private Boolean dead;
    [Header("IFrames")]
    [SerializeField] private float playerInvulnerabilityWindow;
    [SerializeField] private float playerFlashes;
    private SpriteRenderer spriteRenderer;
    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private UIManager uiManager;
    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiManager = FindFirstObjectByType<UIManager>();
    }
    public void takeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        if (currentHealth > 0)
        {
            animator.SetTrigger("Hurt");
            StartCoroutine(Invulnerability());
        }
        else
        {
            if (!dead)
            {

                foreach (Behaviour behaviour in components)
                {
                    behaviour.enabled = false;
                }
                animator.SetBool("grounded", true);
                animator.SetTrigger("Die");
                dead = true;
                if (tag.Equals("Player"))
                {
                    uiManager.GameOver(dead);

                }
            }

        }
    }
    public void getHealth(float _health)
    {
        currentHealth = Mathf.Clamp(currentHealth + _health, 0, startingHealth);
    }
    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        for (int i = 0; i < playerFlashes; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(playerInvulnerabilityWindow / (playerFlashes * 2));
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(playerInvulnerabilityWindow / (playerFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

}
