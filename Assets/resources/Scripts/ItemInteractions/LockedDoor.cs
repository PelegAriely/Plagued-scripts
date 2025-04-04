using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockedDoor : MonoBehaviour
{
    public GameObject requiredKey; // Reference to the key object required to open this door
    private bool isOpen = false; // Whether the door is open or closed

    // Method to open or close the locked door
    public void ToggleDoor (GameObject key)
    {
        if (key == requiredKey)
        {
            isOpen = !isOpen;
            
            // Find the animator in the child object
            Animator doorAnimator = GetComponentInChildren<Animator>();
            if (doorAnimator != null )
            {
                if(isOpen)
                {
                    doorAnimator.SetTrigger("open");
                }
                else
                {
                    doorAnimator.SetTrigger("close");
                }
            }
            else
            {
                Debug.LogError("No Animator found in LockedDoor children!");
            }

            Debug.Log(isOpen ? "The door is now open." : "The door is now closed.");
        }
        else
        {
            Debug.Log("You need the correct key to open this door.");
        }
    }

}
