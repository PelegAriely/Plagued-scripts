using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PlayerInteraction : MonoBehaviour
{
    // Key bindings for different interactions
    public KeyCode interactLanternKey = KeyCode.E;  // Key to interact with the lantern
    public KeyCode interactMaskKey = KeyCode.Q;     // Key to interact with the mask
    public KeyCode useFillObjectKey = KeyCode.R;    // Key to refill the lantern
    
    // UI elements to display interaction prompts
    public TextMeshProUGUI interactionLanternText; 
    public TextMeshProUGUI interactionMaskText;

    // References to the Lantern and Mask objects the player is interacting with
    private Lantern lantern;
    private Mask mask;

    void Start()
    {
        // Hide the interaction text prompts at the beginning
        interactionLanternText.gameObject.SetActive(false);
        interactionMaskText.gameObject.SetActive(false);
    }

    void Update()
    {
        // If the player has a lantern nearby and presses the interaction key, toggle it
        if (lantern && Input.GetKeyDown(interactLanternKey))
        {
            lantern.ToggleLantern();
        }

        // If the player has a mask nearby and presses the interaction key, toggle it
        if (mask && Input.GetKeyDown(interactMaskKey))
        {
            mask.ToggleMask();
        }

        // If the player has a lantern and presses the refill key, refill it
        if (lantern && Input.GetKeyDown(useFillObjectKey))
        {
            lantern.UseRefill();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player enters a collider with a Lantern component, store a reference to it
        if (other.TryGetComponent(out Lantern foundLantern))
        {
            lantern = foundLantern;
            interactionLanternText.text = $"Press {interactLanternKey} to interact with Lantern";
            interactionLanternText.gameObject.SetActive(true); // Show interaction prompt
        }
        
        // If the player enters a collider with a Mask component, store a reference to it
        if (other.TryGetComponent(out Mask mask))
        {
            mask = foundMask;
            interactionMaskText.text = $"Press {interactMaskKey} to interact with Mask";
            interactionMaskText.gameObject.SetActive(true); // Show interaction prompt
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player leaves the area of a lantern, remove the reference and hide the prompt
        if (other.TryGetComponent(out Lantern _))
        {
            lantern = null;
            interactionLanternText.gameObject.SetActive(false);
        }
        
        // If the player leaves the area of a mask, remove the reference and hide the prompt
        if (other.TryGetComponent(out Mask _))
        {
            mask = null;
            interactionMaskText.gameObject.SetActive(false);
        }
    }
}
