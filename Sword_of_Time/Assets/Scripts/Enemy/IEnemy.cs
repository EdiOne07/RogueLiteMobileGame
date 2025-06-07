using UnityEngine;

public interface IEnemy: IRewindable
{
     bool PlayerVisibility();
     void OnDrawGizmos();
}
