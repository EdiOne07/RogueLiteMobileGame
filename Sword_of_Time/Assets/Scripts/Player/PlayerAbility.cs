using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public bool canDash=false;
    public bool canRewind=false;
    public bool canShootFaster = false;
    public bool canShotgunShot=false;
    public bool canDoubleJump=false;
    public bool canHaveMoreHealth=false;
    public void unlockRewind()
    {
        canRewind = true;
        Debug.Log("Rewind unlocked!");
    }
    public void unlockDash()
    {
        canDash = true;
        Debug.Log("Wind Dash unlocked!");
    }
    public void unlockShootFaster()
    {
        canShootFaster = true;
        Debug.Log("Fast Shooting Unlocked");
    }
    public void unlockShotgunShot()
    {
        canShotgunShot = true;
        Debug.Log("Multiple Directions Shooting Unlocked");
    }
    public void unlockDoubleJump()
    {
        canDoubleJump=true;
        Debug.Log("Double Jump Unlocked");
    }
    public void unlockMoreHealth()
    {
        canHaveMoreHealth = true;
        Debug.Log("Extra Health");
    }
}
