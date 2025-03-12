using UnityEngine;

public class OilBarrel : MonoBehaviour
{
    public Lantern lantern; // Referemce to the lantern object
    public float refillDistance = 1.5f; // The distance within which the player can interact with the barrel


    void Update()
    {
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Check if the player is within range of the oil barrel
            if (Vector3.Distance(transform.position, player.transform.position) <= refillDistance)
            {
                // If the player presses the "E" key and the lantern is not null
                if (Input.GetKeyDown(KeyCode.E) && lantern != null)
                {
                    // Refill the lantern's charge regardless of its state
                    lantern.RefillCharge(); // Refill the lantern's charge
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        // Visualize the interaction range in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position, refillDistance);
    }
}
