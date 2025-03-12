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
            Animator doorAnimator = GetComponent<Animator>();
            if (doorAnimator != null )
            {
                if(isOpen)
                {
                    doorAnimator.SetTrigger("Open");
                }
                else
                {
                    doorAnimator.SetTrigger("Close");
                }
            }
            else
            {
                gameObject.SetActive(isOpen); // Toggle the door's active state
            }

            Debug.Log(isOpen ? "The door is now open." : "The door is now closed.");
        }
        else
        {
            Debug.Log("You need the correct key to open this door.");
        }
    }

}
