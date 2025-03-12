using UnityEngine;

public class LockedBox : MonoBehaviour
{
    public int correctCode = 327; // Set the correct code for the box 
    private bool isUnlocked = false; // Whether the box is unlocked
    private bool isInteracting = false; // Flag to start interaction
    private string enteredCode = ""; // Track the code input

    void Update()
    {
        if (isInteracting)
        {
            // Only allow input if the player is interacting with the box
            if(Input.anyKeyDown && enteredCode.Length < 3)
            {
                // Append the key input to entereCode
                string key = Input.inputString;
                if (int.TryParse(key, out int digit)) // Ensure the input is a digit
                {
                    enteredCode += key;
                    Debug.Log("Code so far:" + enteredCode);

                    if (enteredCode.Length == 3)
                    {
                        // Check is the entered code matches the correct code
                        if (int.TryParse(enteredCode, out int code) && code == correctCode)
                        {
                            UnlockBox();
                        }
                        else
                        {
                            Debug.Log("Incorrect code. Try again.");
                            enteredCode = ""; // Reset the code if incorrect
                        }
                    }
                }
            }
            // Allow the player to stop interacting with the box by pressing the interaction key again
            if (Input.GetKeyDown(KeyCode.E))
            {
                StopInteraction();
            }
        }
    }

    private void UnlockBox()
    {
        if (!isUnlocked)
        {
            isUnlocked = true;
            Debug.Log("The box is unlocked!");
            // can add code to open the box here, like playing an animation or enabling the object
        }
    }

    public void StartInteraction()
    {
        if (!isUnlocked)
        {
            isInteracting = true;
            enteredCode = ""; // Reset the code every time a new interaction begins
            Debug.Log("Enter 3-digit code:");
        }
    }

    public void StopInteraction()
    {
        isInteracting = false;
        enteredCode = ""; // Clear the entered code
        Debug.Log("Stopped interacting with the box.");
    }
}
