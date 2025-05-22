using UnityEngine;

public class PointInTime 
{
    public Vector2 position;
    public Quaternion rotation;
    public PointInTime(Vector2 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
