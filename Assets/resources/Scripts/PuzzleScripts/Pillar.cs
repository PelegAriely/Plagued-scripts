using UnityEngine;

public class Pillar : MonoBehaviour
{
    public Transform gizmoSpawnPoint;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (gizmoSpawnPoint != null)
            Gizmos.DrawWireSphere(gizmoSpawnPoint.position, 0.2f);
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}
