using UnityEngine;

public class AbilityUnlockItem : MonoBehaviour
{
    public enum AbilityType { Dash, Rewind }
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
                }

                Destroy(gameObject);
            }
        }
    }
}
