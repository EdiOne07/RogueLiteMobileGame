using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    //Room Camera
    [SerializeField] private float speed;
    private float currentPosx;
    private Vector3 velocity = Vector3.zero;
    //Follow Player
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float CameraSpeed;
    private float lookAhead;
    private void Update()
    {
        //transform.position=Vector3.SmoothDamp(transform.position,new Vector3(currentPosx,transform.position.y,transform.position.z),ref velocity,speed);
        transform.position = new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z);
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * speed);
    }
    public void moveToNewRoom(Transform _newRoom)
    {
        currentPosx = _newRoom.position.x;
    }
}
