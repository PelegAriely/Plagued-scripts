using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class mask : MonoBehaviour
{
    public bool IsMaskActive { get; private set; } = false; // Track is the mask is active 

    [Header("Upgrade Settings")]
    public bool IsUpgraded = false; // Track if the mask has been upgraded
    public PlayerInteraction PlayerInteractionScript;
    public float upgradedBreathDepletionRate = 0.1f; // New depletion rate after upgrade

    // Method to turn on the mask
    public void TurnOnMask()
    {
        if (!IsMaskActive)
        {
            // Perform any actions related to turning on the mask, e.g., enable mask visuals
            IsMaskActive = true;
            Debug.Log("Mask is now active.");
            // Optionally, you can add code here to handle things like mask animations or effects
        }
    }
    // Method to turn off the mask
    public void TurnOffMask()
    {
        if (IsMaskActive)
        {
            IsMaskActive = false;
            // Perform any actions related to turning off the mask, e.g., disable mask visuals
            Debug.Log("Mask is now inactive.");
            // Optionally, you can add code here to handle things like mask animations or effects
        }

    }

    // Optionally: Visual effects for the mask when active (can be modified)
    public void ActivateMaskVisuals()
    {
        // Add your code to handle mask visual effects here (e.g., activating particles or animations)
    }

    public void DeactivateMaskVisuals()
    {
        // Add your code to handle mask visual effects here (e.g., deactivating particles or animations)
    }

    // Method to upgrade the mask
    public void UpgradeMask()
    {
        if (!IsUpgraded)
        {
            IsUpgraded = true; // Mark mask as upgraded
            if (PlayerInteractionScript != null)
            {
                PlayerInteractionScript.maskBreathDepletionRate = upgradedBreathDepletionRate; // Reduce depletion rate
            }
            Debug.Log("Mask upgraded! Breath depletion rate reduced.");
        }
    }
}
