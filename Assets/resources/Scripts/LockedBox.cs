using UnityEngine;
using StarterAssets;
using TMPro;

public class LockedBox : MonoBehaviour
{
    public int correctCode = 327; // Set the correct code for the box 
    private bool isUnlocked = false; // Whether the box is unlocked
    private bool isInteracting = false; // Flag for interaction
    private string enteredCode = ""; // Track the code input
    private bool isPlayerNearby = false; // Track is the player is near the box

    [Header("References")]
    public GameObject objectInside; // The object inside the box (disabled until unlocked)
    public Animator boxAnimator; // Animator for the box opening
    public Transform lockModel; // The lock model that will move in front of the camera
    public Transform lockViewingPosition; // The position in front of the camera for interaction

    [Header("Text Elements")]
    public TMP_Text lockText; // Displays messages like "Incorrect code"
    public TMP_Text enteredCodeText; // Displays the numbers entered by the player
    
    private Vector3 originalLockPosition;
    private Quaternion originalLockRotation;
    private Transform originalLockParent;
    
    public ThirdPersonController playerController; // Reference to player movement script (Starter Assets)



    void Start()
    {
        if (objectInside != null)
        {
            objectInside.SetActive(false); // Hide the object inside initially
        }

        // Store the original position of the lock
        if (lockModel != null)
        {
            originalLockPosition = lockModel.position;
            originalLockRotation = lockModel.rotation;
            originalLockParent = lockModel.parent;
        }

        // Disable text elements at start
        if (lockText != null) lockText.gameObject.SetActive(false);
        if (enteredCodeText != null) enteredCodeText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (isInteracting)
            {
                StopInteraction();
            }
            else
            {
                StartInteraction();
            }
        }

        if (isInteracting && Input.anyKeyDown && enteredCode.Length < 3)
        {
            string key = Input.inputString;
            if (int.TryParse(key, out int digit))
            {
                enteredCode += key;
                UpdateEnteredCodeText(); // Update the text displayed on the lock

                if (enteredCode.Length == 3)
                {
                    if (int.Parse(enteredCode) == correctCode)
                    {
                        UnlockBox();
                    }
                    else
                    {
                        lockText.text = "Incorrect code. Try again.";
                        enteredCode = ""; // Reset code if incorrect
                    }
                }
            }
        }
    }

    private void UnlockBox()
    {
        if (!isUnlocked)
        {
            isUnlocked = true;
            lockText.text = "Unlocked!";
            Debug.Log("The box is unlocked!");
            boxAnimator.SetTrigger("Open"); // Trigger the box animation

            if (objectInside != null)
            {
                objectInside.SetActive(true); // Show the object inside
            }

            StopInteraction(); // Exit interaction mode after unlocking
        }
    }

    public void StartInteraction()
    {
        if (!isUnlocked && lockModel != null && lockViewingPosition != null)
        {
            isInteracting = true;
            enteredCode = ""; // Reset the entered code
            
            // Enable text elements
            if (lockText != null)
            {
                lockText.gameObject.SetActive(true);
                lockText.text = "Enter 3-digit code"; // Reset display text
            }
            
            if (enteredCodeText != null)
            {
                enteredCodeText.gameObject.SetActive(true);
                enteredCodeText.text = ""; // Reset the entered code display
            }

            // Move the lock model in front of the camera
            lockModel.SetParent(lockViewingPosition);
            lockModel.localPosition = Vector3.zero;
            lockModel.localRotation = Quaternion.identity;

            LockPlayerMovement(true);
        }
    }

    public void StopInteraction()
    {
        if (lockModel != null)
        {
            // Return the lock model to its original position
            lockModel.SetParent(originalLockParent);
            lockModel.position = originalLockPosition;
            lockModel.rotation = originalLockRotation;
        }

        // Disable text elements
        if (lockText != null) lockText.gameObject.SetActive(false);
        if (enteredCodeText != null) enteredCodeText.gameObject.SetActive(false);

        
        isInteracting = false;
        enteredCode = ""; // Clear entered code
        LockPlayerMovement(false);
    }


    private void LockPlayerMovement(bool lockMovement)
    {
        if (playerController != null)
        {
            playerController.enabled = !lockMovement; // Disable player movement when interacting
        }
    }
    
    private void UpdateEnteredCodeText()
    {
        if (enteredCodeText != null)
        {
            enteredCodeText.text = enteredCode; // Display the current entered code on the lock
        }
    }
    
    // Trigger Detection to Check if Player is Nearby
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Press E to interact with the box.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
