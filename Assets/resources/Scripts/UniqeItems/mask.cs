using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Mask : MonoBehaviour
{
    [Header("Breath Settings")] 
    public float breathDepletionRate = 10f; // Breath loss per second while in fog with mask on
    
    private bool isMaskActive = false;   // Whether the mask is currently active
    private bool isInFog = false;        // Whenther the player is currently in fog


    private float currentBreath = 100f;    // Remaining breath
    private float maxBreath = 100f;        // Maximum breath value

    void update()
    {
        if (isInFog && isMaskActive)
        {
            // Decrease breath over time, clamping to 0
            currentBreath = Mathf.Max(0, currentBreath - breathDepletionRate * Time.deltaTime);
        }
        // add behavior here if breath runs out
        
    }

    public void ToggleMask()
    {
        if (isMaskActive)
            TurnOffMask();
        else
            TurnOnMask();
    }
    
    private void TurnOnMask()
    {
        isMaskActive = true;
        Debug.Log("Mask is now active.");
    }

    private void TurnOffMask()
    {
        isMaskActive = false;
        Debug.Log("Mask is now inactive.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fog"))
        {
            isInFog = true; 
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fog"))
        {
            isInFog = false;
        }
    }
}
