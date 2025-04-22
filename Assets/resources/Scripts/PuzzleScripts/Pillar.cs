using UnityEngine;

public class Pillar : MonoBehaviour
{
    public Transform gizmoSpawnPoint;
    private PuzzleObject placedObject;
    public bool HasObject => placedObject != null;

    public bool TryPlace(PuzzleObject puzzleObject)
    {
     if (placedObject != null) 
         return false;
     
     placedObject = puzzleObject;
     return true;
    }

    public void RemoveObject()
    {
        placedObject  = null;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (gizmoSpawnPoint != null)
            Gizmos.DrawWireSphere(gizmoSpawnPoint.position, 0.2f);
    }
}
