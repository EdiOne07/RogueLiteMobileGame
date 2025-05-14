using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform previousRoom;
    [SerializeField] private Transform nextRoom;
    [SerializeField] private CameraBehaviour cameraController;
    private void Awake()
    {

        cameraController = Camera.main.GetComponent<CameraBehaviour>();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.transform.position.x < transform.position.x)
            {
                cameraController.moveToNewRoom(nextRoom);
                nextRoom.GetComponent<RoomReset>().ActivateRoom(true);
                previousRoom.GetComponent<RoomReset>().ActivateRoom(false);
            }
            else
            {
                cameraController.moveToNewRoom(previousRoom);
                previousRoom.GetComponent<RoomReset>().ActivateRoom(true);
                nextRoom.GetComponent<RoomReset>().ActivateRoom(false);
            }
        }
    }

}
