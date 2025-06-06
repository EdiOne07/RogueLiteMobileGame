using UnityEngine;
using UnityEngine.UI;

public class HealthBarFunctionality : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;
    private PlayerAbility playerAbility;
   
    void Start()
    {
        totalHealthBar.fillAmount = playerHealth.currentHealth/10;
        playerAbility=playerHealth.GetComponent<PlayerAbility>();
    }


    void Update()
    {
        if (playerAbility.canHaveMoreHealth)
        {
            totalHealthBar.fillAmount = playerHealth.maxHealth / 10;
           
        }
            currentHealthBar.fillAmount = playerHealth.currentHealth / 10;     
        
    }
}
