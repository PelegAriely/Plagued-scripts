using UnityEngine;

public class PuzzleObject : MonoBehaviour
{
    [SerializeField] private Pillar correctPillar; // Assigned in Inspector — the pillar this object should go on
    
    private Rigidbody rb;
    private Collider col;

    private bool isHeld = false;
    private Pillar currentPillar = null;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Used to freeze object after placement
        col = GetComponent<Collider>(); // Disable collider after placement
    }

    public void PickUp(Transform hand)
    {
        isHeld = true;
        currentPillar = null;
        
        rb.isKinematic = true;
        col.enabled = false;
        
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(Vector3 dropPos)
    {
        isHeld = false;
        
        transform.SetParent(null);
        transform.position = dropPos;

        rb.isKinematic = false;
        col.enabled = true;

        currentPillar = null;

        PuzzleManager.Instance.EvaluatePuzzleState();
    }

    public void PlaceOnPillar(Pillar pillar)
    {
        isHeld = false;
        currentPillar = pillar;
        
        transform.SetParent(null);
        transform.position = pillar.gizmoSpawnPoint.position;
        transform.rotation = pillar.gizmoSpawnPoint.rotation;

        rb.isKinematic = true;
        col.enabled = false;

        PuzzleManager.Instance.EvaluatePuzzleState();
    }
    
    // Called by PuzzleManager to see if this object is correctly placed
    public bool IsCorrectlyPlaced()
    {
        return currentPillar == correctPillar;
    }

    public void LockInPlace()
    {
        rb.isKinematic = true;
        col.enabled = false;
        gameObject.tag = "Placed";
    }
}
