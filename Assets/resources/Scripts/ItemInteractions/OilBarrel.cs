using UnityEngine;

public class OilBarrel : MonoBehaviour
{
    private Lantern lanternNearby; // Reference to the player's lantern when in range
    private bool isPlayerInRange = false; // Tracks whether the player is currently inside the trigger zone

    public void Refill()
    {
        // Only refill if the player is in range and a lantern is linked
        if (isPlayerInRange && lanternNearby != null)
        {
            lanternNearby.UseRefill(); // Refill the lantern
            Debug.Log("Lantern refilled by Oil Barrel.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            
            if (other.TryGetComponent(out Lantern foundLantern))
            {
                lanternNearby = foundLantern; // Store reference to the lantern for refilling
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // When the player leaves the barrel's range
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            lanternNearby = null; // Clear reference to avoid unintended refill
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a wireframe sphere around the barrel to visualize the interaction range in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1.5f); // Fixed radius for now
    }
}
