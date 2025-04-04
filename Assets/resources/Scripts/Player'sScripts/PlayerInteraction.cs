using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;


public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactLanternKey = KeyCode.E; // Key to interact with the lantern (turn on/off)
    public KeyCode pickUpFillObjectKey = KeyCode.E; // Key to pick up a refill object
    public KeyCode useFillObjectKey = KeyCode.R; // Key to use a pick-up refill object
    public KeyCode interactMaskKey = KeyCode.Q; // Key to interact with the mask (pick up, put on, put away)
    public TextMeshProUGUI interactionLanternText; // UI Text to show perompt for lantern interaction
    public TextMeshProUGUI interactionMaskText; // UI text to show prompt for mask interaction

    public float maxBreath = 100f; // Maximum breath charge
    public float currentBreath; // Current breath charge
    public float normalBreathDepletionRate = 10f; // Rate at which breath depletes in the fog when mask is not active
    public float maskBreathDepletionRate = 5f; // Rate at which breath depletes in the for when mask is active
    private bool isPlayerInFog = false; // Whetnher the player is in the fog and the mask is not active
    private bool isMaskActive = false; // Whether the mask is currently active

    private GameObject lantern; // Reference to the lantern object
    public GameObject emptyHandObject; // Reference to the empty hand object in the inspector
    public GameObject emptyHandObjectPutAway; // reference to the empty object where the lantern goes when turned off
    private Lantern lanternScript; // Reference to the lantern script
    private bool isLanternHeld = false; // Flag to check if the lantern is held by the player
    private bool isPlayerNearbyLantern = false; // Is the player near a lantern
    private bool isPlayerNearbyRefill = false; // Is the player near a refill object
    private GameObject nearRefillObject; // Reference to the nearby refill object

    private GameObject mask; // Reference to the mask object
    private mask maskScript; // reference to the mask's script
    private bool isMaskHeld = false; // Flag to check if the mask is held by the player
    private bool isPlayerNearbyMask = false; // Is the player near the mask
    private float previousBreath = 100f; // Store the previous breath to compare for changes

    public GameObject emptyHeadObject; // Reference to the empty head object in the inspector (for attaching the mask)
    public GameObject emptyHeadObjectPutAway; // Reference to the empty object where the mask goes when put away

    private List<GameObject> fillObjectsHeld = new List<GameObject>(); // List to hold multiple fill objects
    public int maxFillObjects = 3; // maximum number of refill objects the player can hold at once
    private int replenishMaskCount = 0; // Number of collected replenish mask items
    public float replenishAmount = 50f; // Amount of breath restored per item


    void Start()
    {
        lanternScript = null;
        lantern = null;
        interactionLanternText.gameObject.SetActive(false); // hide prompt initially

        maskScript = null;
        mask = null;
        interactionMaskText.gameObject.SetActive(false); // Hide prompt initially

        currentBreath = maxBreath; // Initialize the breath charge
        previousBreath = currentBreath; // Initialize the previuos breath value
    }

    void Update()
    {
        // lantern interaction
        // If player is near a lantern, show the interaction text
        if (isPlayerNearbyLantern)
        {
            interactionLanternText.gameObject.SetActive(true); // Show lantern text when near lantern

            // interact with lantern (turn on/off)
            if (Input.GetKeyDown(interactLanternKey) && lanternScript != null)
            {
                if (!isLanternHeld)
                {
                    PickUpLantern();
                }
                else
                {
                    if (lanternScript.IsLanternOn)
                    {
                        lanternScript.TurnOffLantern(); // Turn off the lantern
                        PutAwayLantern(); // Move Lantern to the "put away" position when turned off
                    }
                    else
                    {
                        lanternScript.TurnOnLantern(); // Turn on the lantern
                        RetrieveLantern(); // Move lantern back to the player's hand when turned on
                    }
                }
            }
        }
        else
        {
            interactionLanternText.gameObject.SetActive(false); // Hide the text when not near lantern
        }

        // Mask interaction
        // If player is near the mask, show the interaction prompt
        if (isPlayerNearbyMask)
        {
            interactionMaskText.gameObject.SetActive(true); // Show interaction text when near the mask

            // Interact with the mask (pick up, put on, or put away)
            if (Input.GetKeyDown(interactMaskKey) && maskScript != null)
            {
                if (!isMaskHeld) // If the mask is not held
                {
                    PickUpMask(); // Pick up the mask
                    maskScript.TurnOnMask();
                }
                else if (isMaskHeld && !maskScript.IsMaskActive) // If the mask is held but not active
                {
                    maskScript.TurnOnMask(); // Activate the mask
                    RetrieveMask(); // Put the mask on the player's head
                    Debug.Log("Mask is now active.");
                }
                else if (isMaskHeld && maskScript.IsMaskActive) // If the mask is held and active
                {
                    maskScript.TurnOffMask(); // Deactivate the mask
                    PutAwayMask(); // Put the mask away
                    Debug.Log("Mask put away and is now inactive.");
                }
            }
        }

        else
        {
            interactionMaskText.gameObject.SetActive(false); // Hide interaction text when not near the mask
        }

        // Handle refill object interaction
        if (isPlayerNearbyRefill && Input.GetKeyUp(pickUpFillObjectKey))
        {
            PickUpFillObject(nearRefillObject); // Pick up the refill object
        }

        // refill object use
        if (Input.GetKeyDown(useFillObjectKey))
        {
            UseFillObject(); // Call the methos to use the refill object
        }

        // Mask berath handeling
        if (isPlayerInFog)
        {
            if (maskScript.IsMaskActive)
            {
                float currentDepletionRate = maskScript.IsUpgraded ? maskScript.upgradedBreathDepletionRate : maskBreathDepletionRate;
                currentBreath -= currentDepletionRate * Time.deltaTime;
            }
            else
            {
                currentBreath -= normalBreathDepletionRate * Time.deltaTime; // Deplete breath normally without mask
            }
        }

        // Update breath status only when breath changes
        if (currentBreath != previousBreath)
        {
            Debug.Log("Breath: " + currentBreath.ToString("F1") + "/" + maxBreath);
            previousBreath = currentBreath; // update previous breath value
        }

        // Regain breath if not in fog
        if (!isPlayerInFog && currentBreath < maxBreath)
        {
            currentBreath += normalBreathDepletionRate * Time.deltaTime; // Regain breath charge when out of fog
        }

        if (Input.GetKeyDown(KeyCode.F) && replenishMaskCount > 0)
        {
            UseReplenishMask();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lantern"))
        {
            lantern = other.gameObject;
            lanternScript = lantern.GetComponent<Lantern>(); // Get reference to the Lantern's script
            isPlayerNearbyLantern = true;
            interactionLanternText.text = "press " + interactLanternKey.ToString() + " to pick up lantern or turn it on / off";
        }

        if (other.CompareTag("RefillObject"))
        {
            isPlayerNearbyRefill = true;
            nearRefillObject = other.gameObject; // Save reference to the refill object
            Debug.Log("Can pick refill"); // Message for nearby refill object
        }

        if (other.CompareTag("Mask"))
        {
            mask = other.gameObject; // Get reference to the mask object
            maskScript = mask.GetComponent<mask>(); //get reference to the mask's script
            isPlayerNearbyMask = true; // Set the flag that player is near the mask
            interactionMaskText.text = "press " + interactMaskKey.ToString() + " to pick up the maskor turn it on/off"; // Set the interaction text
        }

        if (other.CompareTag("Fog"))
        {
            isPlayerInFog = true; // Player enters the fog
        }

        if (other.CompareTag("ReplenishMask") && maskScript.IsUpgraded)
        {
            replenishMaskCount++;
            Debug.Log("Replenish Mask collected! Total: " + replenishMaskCount);
            other.gameObject.SetActive(false); // Hide item after pickup
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Lantern"))
        {
            isPlayerNearbyLantern = false;
            lantern = null;
            lanternScript = null; // clear lantern reference
            interactionLanternText.gameObject.SetActive(false); // Hide interaction text when leaving lantern
        }

        if (other.CompareTag("RefillObject"))
        {
            isPlayerNearbyRefill = false;
            nearRefillObject = null; // Clear reference to refill object when leaving collider
        }
        if (other.CompareTag("Mask"))
        {
            isPlayerNearbyMask = false; // Player leaves the mask's interaction range
            mask = null;
            maskScript = null; // clear mask reference
            interactionMaskText.gameObject.SetActive(false); // Hide the interaction text
        }

        if (other.CompareTag("Fog"))
        {
            isPlayerInFog = false; // Player exits the fog
        }
    }

    private void PickUpLantern()
    {
        if (lantern != null)
        {
            // set the lantern to be held by the player (attach it to the player's hand)
            lantern.transform.SetParent(emptyHandObject.transform); // Attach lantern to hand
            lantern.transform.localPosition = Vector3.zero; // Reset position relative to hand
            lantern.transform.localRotation = Quaternion.identity; // Reset rotation
            isLanternHeld = true; // Mark the lantern as held
            interactionLanternText.gameObject.SetActive(false); // Hide the interaction prompt after picking up
            lanternScript.TurnOnLantern(); // Turn on the lantern light
        }
    }

    private void PutAwayLantern()
    {
        if (lantern != null)
        {
            // Move lantern to the "put away" position
            lantern.transform.SetParent(emptyHandObjectPutAway.transform); // attach lantern to "put away" object
            lantern.transform.localPosition = Vector3.zero; // Reset position relative to putision
            lantern.transform.localRotation = Quaternion.identity; // Reset rotation
            lanternScript.TurnOffLantern(); // Turn off the lantern light
        }
    }

    private void RetrieveLantern()
    {
        if (lantern != null)
        {
            // Move lantern back to the player's hand
            lantern.transform.SetParent(emptyHandObject.transform); //attach lantern back to hand
            lantern.transform.localPosition = Vector3.zero; // Reset position relative to hand
            lantern.transform.localRotation = Quaternion.identity; // Reset rotation
            lanternScript.TurnOnLantern(); // Turn on the lantern light if it wasn't depleted
        }
    }

    private void PickUpFillObject(GameObject refill)
    {
        if (fillObjectsHeld.Count < maxFillObjects)
        {
            fillObjectsHeld.Add(refill);
            refill.SetActive(false); // Hide the refill object in the workd
            Debug.Log("Picked refill! Current refill objects: " + fillObjectsHeld.Count); // Message with number of refills held
            Destroy(refill); // Destroy the picked-up refill object
        }
        else
        {
            Debug.Log("Cannot carry more refill objects"); // Inform the player they can't carry more
        }
    }

    private void UseFillObject()
    {
        if (lanternScript != null && fillObjectsHeld.Count > 0)
        {
            lanternScript.RefillCharge(); // Refill the lantern's charge
            GameObject refillObject = fillObjectsHeld[0]; // Get the first refill object
            fillObjectsHeld.RemoveAt(0); // remove it from the list
            Debug.Log("Used refill"); // Message when refill is used
        }
    }

    private void PickUpMask()
    {
        if (mask != null)
        {
            // Set the mask to be held by the player (attach it to the player's head)
            mask.transform.SetParent(emptyHeadObject.transform);
            mask.transform.localPosition = Vector3.zero;
            mask.transform.localRotation = Quaternion.identity;

            // Disable the mask's collider and stop physics calculations
            Collider col = mask.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Rigidbody rb = mask.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            isMaskHeld = true; // Mark the mask as held
            interactionMaskText.gameObject.SetActive(false); // Hide interaction prompt once pick up
            Debug.Log("Mask picked up and is now held");
        }
    }

    private void PutAwayMask()
    {
        if (mask != null)
        {
            // move the mask to the "put away" position
            mask.transform.SetParent(emptyHeadObjectPutAway.transform);
            mask.transform.localPosition = Vector3.zero;
            mask.transform.localRotation = Quaternion.identity;

            Debug.Log("Mask put away.");
        }
    }

    private void RetrieveMask()
    {
        if (mask != null)
        {
            // Attach the mask to the player's head and move it to the correct position
            mask.transform.SetParent(emptyHeadObject.transform);
            mask.transform.localPosition = Vector3.zero;
            mask.transform.localRotation = Quaternion.identity;

            isMaskHeld = true; // Mark the mask as held again
            Debug.Log("Mask retrieved and placed back on the player's head.");
        }
    }

    public void UpgradeMask()
    {
        if (maskScript != null && !maskScript.IsUpgraded)
        {
            maskScript.UpgradeMask(); // Call the UpgradeMask() function in mask.cs
        }
    }

    public void UseReplenishMask()
    {
        if (currentBreath < maxBreath)
        {
            float restoreAmount = Mathf.Min(replenishAmount, maxBreath - currentBreath); // Prevent overfilling
            currentBreath += restoreAmount;
            replenishMaskCount--; // Use one item

            Debug.Log("Used Replenish Mask! Breath restored: " + restoreAmount);
        }
        else
        {
            Debug.Log("Breath is already full!");
        }
    }
}
