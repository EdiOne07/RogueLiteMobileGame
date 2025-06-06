using UnityEngine;

public class AbilityUnlockItem : MonoBehaviour
{
    public enum AbilityType { Dash, Rewind, Double_Jump, Fast_Shooting, Shotgun_Shot,Extra_Health }
    public AbilityType abilityToUnlock;
    public ItemUnlockedHint uiHint;
    private void Start()
    {
        if (uiHint == null)
        {
            uiHint = FindFirstObjectByType<ItemUnlockedHint>();
        }
    }
    
private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Player"))
        {
            PlayerAbility abilities = collider2D.GetComponent<PlayerAbility>();

            if (abilities != null)
            {
                switch (abilityToUnlock)
                {
                    case AbilityType.Dash:
                        abilities.unlockDash();
                        uiHint.ShowHint("Unlocked: Dash!");
                        break;
                    case AbilityType.Rewind:
                        abilities.unlockRewind();
                        uiHint.ShowHint("Unlocked: Time Rewind!");
                        break;
                    case AbilityType.Double_Jump: 
                        abilities.unlockDoubleJump();
                        uiHint.ShowHint("Unlocked: Double Jump!");
                        break ;
                    case AbilityType.Fast_Shooting:
                        abilities.unlockShootFaster();
                        collider2D.GetComponent<PlayerAttack>().ModifyAttackCooldown();
                        uiHint.ShowHint("Unlocked: Fast Shooting!");
                        break;
                    case AbilityType.Shotgun_Shot :
                        abilities.unlockShotgunShot();
                        uiHint.ShowHint("Unlocked: Shotgun Shot!");
                        break;
                    case AbilityType.Extra_Health :
                        abilities.unlockMoreHealth();
                        Health health = collider2D.GetComponent<Health>();
                        if(health != null)
                        {
                            health.addHealth(3);
                        }
                        uiHint.ShowHint("Unlocked: Extra Health!");
                        break;
                }

                Destroy(gameObject);
            }
        }
    }
}
