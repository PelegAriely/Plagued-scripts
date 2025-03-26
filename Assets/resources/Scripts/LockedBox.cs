using UnityEngine;
using UnityEngine.UI;
using StarterAssets; // Import Starter Assets
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
    public GameObject uiPanel; // The UI panel for entering the code
    public TMP_Text enteredCodeText; // UI TextMeshPro text for entered code
    public TMP_Text feedbackText; // UI TextMeshPro text for feedback
    public ThirdPersonController playerController; // Reference to player movement script (Starter Assets)



    void Start()
    {
        if (objectInside != null)
        {
            objectInside.SetActive(false); // Hide the object inside the box initially
        }

        uiPanel.SetActive(false); // Hide UI at the start
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

        if (isInteracting)
        {
            if (Input.anyKeyDown && enteredCode.Length < 3)
            {
                string key = Input.inputString;
                if (int.TryParse(key, out int digit)) // Ensure the input is a digit
                {
                    enteredCode += key;
                    enteredCodeText.text = enteredCode; // Update UI display

                    if (enteredCode.Length == 3)
                    {
                        if (int.Parse(enteredCode) == correctCode)
                        {
                            UnlockBox();
                        }
                        else
                        {
                            feedbackText.text = "Incorrect code. Try again"; // 
                            enteredCode = ""; // Reset the code if incorrect
                        }
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
            feedbackText.text = "Box unlocked!"; // Show unlock message in UI
            Debug.Log("The box is unlocked!");
            boxAnimator.SetTrigger("Open"); // Trigger the animation

            if (objectInside != null)
            {
                objectInside.SetActive(true); // Enable the object inside
            }

            StopInteraction(); // Close UI after unlocking
        }
    }


    public void StartInteraction()
    {
        if (!isUnlocked)
        {
            isInteracting = true;
            enteredCode = ""; // Reset the code every time a new interaction begins
            enteredCodeText.text = ""; // Clear UI display
            feedbackText.text = "Enter 3-digit code:"; // Instruction message
            uiPanel.SetActive(true); // Show the UI
            LockPlayerMovement(true); // Disable player movement
            Debug.Log("Enter 3-digit code:");
        }
    }

    public void StopInteraction()
    {
        isInteracting = false;
        enteredCode = ""; // Clear the entered code
        uiPanel.SetActive(false); // Hide the UI
        LockPlayerMovement(false); // Enable player movement
        Debug.Log("Stopped interacting with the box.");
    }


    private void LockPlayerMovement(bool lockMovement)
    {
        if (playerController != null)
        {
            playerController.enabled = !lockMovement; // Disable player movement when interacting
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
