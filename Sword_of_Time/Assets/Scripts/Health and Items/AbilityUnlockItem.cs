using UnityEngine;

public class AbilityUnlockItem : MonoBehaviour
{
    public enum AbilityType { Dash, Rewind, Double_Jump, Fast_Shooting, Multiple_Directions_Shooting,Extra_Health }
    public AbilityType abilityToUnlock;
    public ItemUnlockedHint uiHint;
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
                        uiHint.ShowHint("Unlocked:Dash!");
                        break;
                    case AbilityType.Rewind:
                        abilities.unlockRewind();
                        uiHint.ShowHint("Unlocked:Time Rewind!");
                        break;
                    case AbilityType.Double_Jump: 
                        abilities.unlockDoubleJump();
                        uiHint.ShowHint("Unlocked:Double Jump!");
                        break ;
                    case AbilityType.Fast_Shooting:
                        abilities.unlockShootFaster();
                        uiHint.ShowHint("Unlocked: Fast Shooting!");
                        break;
                    case AbilityType.Multiple_Directions_Shooting :
                        abilities.unlockShootMultipleDirections();
                        break;
                    case AbilityType.Extra_Health :
                        abilities.unlockMoreHealth(); 
                        break;
                }

                Destroy(gameObject);
            }
        }
    }
}
