using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockedDoor : MonoBehaviour
{
    public GameObject requiredKey; // Reference to the key object required to open this door
    private bool isUnlocked = false; // Whether the door is open or closed
    private Animator animator;

    // Method to open or close the locked door
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ToggleDoor(GameObject playerKey)
    {
        if (isUnlocked || playerKey == requiredKey)
        {
            isUnlocked = true; // Unlock the door permanently
            animator.SetTrigger("Toggle"); // Play the door animation
        }
        else
        {
            Debug.Log("You need the correct key to open this door!");
        }
    }
}
