using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public bool canDash=false;
    public bool canRewind=false;
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
}
