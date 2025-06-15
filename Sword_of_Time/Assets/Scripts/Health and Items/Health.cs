using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour,IRewindable
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    public float maxHealth;

    private Animator animator;
    private Boolean dead;
    [Header("IFrames")]
    [SerializeField] private float playerInvulnerabilityWindow;
    [SerializeField] private float playerFlashes;
    private SpriteRenderer spriteRenderer;
    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private UIManager uiManager;
    public bool isRewinding=false;
    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;
    private void Awake()
    {
        maxHealth =startingHealth;
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiManager = FindFirstObjectByType<UIManager>();
    }
    public void takeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, maxHealth);
        if (currentHealth > 0 || isRewinding)
        {
            animator.SetTrigger("Hurt");
            StartCoroutine(Invulnerability());
            SoundManager.instance.PlaySound(hurtSound);
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
                SoundManager.instance.PlaySound(deathSound);
                if (tag.Equals("Player"))
                {
                    GameStatsTracker.Instance?.RecordDeath();

                    uiManager.GameOver(dead);

                }
            }

        }
    }
    public void getHealth(float _health)
    {
        currentHealth = Mathf.Clamp(currentHealth + _health, 0, maxHealth);
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
    public void addHealth(float extraHealth)
    {
        maxHealth += extraHealth;
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void OnRewindStart()
    {
        isRewinding = true;
    }

    public void OnRewindStop()
    {
       isRewinding=false;
    }
}
