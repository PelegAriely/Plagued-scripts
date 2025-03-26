using UnityEngine;

public class Player_Controller : MonoBehaviour
{ 
    public KeyCode interactionKey = KeyCode.E; // Set interaction key

    private bool isNearInteractable = false;  // Whether the player is near an interactable object
    private GameObject interactableObject = null;  // The current interactable object (e.g., door, item, etc.)
    private bool hasKey = false; // Boolean to check is the player had the key
    private GameObject currentKey;  // Store the current key the player has

    private bool isPlayerNearbyUpgrade = false;
    private bool isPlayerNearbyUpgradeStation = false;
    private GameObject upgradeObject;
    private int lanternUpgradeParts = 0; // Count upgrade parts collected
    private Lantern lanternscript;

    private GameObject flammableObject = null; // Store nearby flammable object
    private mask maskScript; // Reference to the mask script
    private int maskUpgradeParts = 0; // Count mask upgrade parts collected
    private bool isPlayerNearbyMaskUpgrade = false;
    private GameObject maskUpgradeObject;
    
    private NoteInteraction noteInteraction; // Reference to the NoteInteraction script

    void Start()
    {
        // Find the first Lantern object in the scene
        lanternscript = Object.FindFirstObjectByType<Lantern>();
        if (lanternscript == null)
        {
            Debug.LogError("Lantern script is not found in the scene!");
        }
        
        maskScript = Object.FindFirstObjectByType<mask>(); // Find the mask in the scene
        
        noteInteraction = FindObjectOfType<NoteInteraction>();
        if(noteInteraction == null) Debug.LogError("Note interaction is not found in the scene!");
    }

        void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            if (isNearInteractable)
            {
                if (interactableObject.CompareTag("Door")) OpenDoor(interactableObject);
                else if (interactableObject.CompareTag("LockedDoor")) InteractWithLockedDoor(interactableObject);
                else if (interactableObject.CompareTag("Key")) PickupKey(interactableObject);
                else if (interactableObject.CompareTag("LockedBox")) InteractWithLockedBox(interactableObject);
                else if (interactableObject.CompareTag("Note"))
                    noteInteraction.HandleNoteInteraction(interactableObject);
            }
            else if (isPlayerNearbyUpgrade) PickUpUpgrade();
            else if (isPlayerNearbyUpgradeStation) UpgradeLantern();
            else if (isPlayerNearbyMaskUpgrade) PickUpMaskUpgrade();
            else if (isPlayerNearbyUpgradeStation) UpgradeMask();
            else if (lanternscript != null && lanternscript.IsLanternOn &&
                     lanternscript.isUpgraded && flammableObject != null) BurnObject();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door") || other.CompareTag("LockedDoor") ||
            other.CompareTag("Key") || other.CompareTag("LockedBox") || other.CompareTag("Note"))
        {
            isNearInteractable = true;
            interactableObject = other.gameObject;
        }

        if (other.CompareTag("Upgrade"))
        {
            isPlayerNearbyUpgrade = true;
            upgradeObject = other.gameObject;
        }

        if (other.CompareTag("UpgradeStation")) isPlayerNearbyUpgradeStation = true;
        if (other.CompareTag("UpgradeMask")) isPlayerNearbyMaskUpgrade = true;

        if (lanternscript != null && lanternscript.isUpgraded && other.CompareTag("Flammable"))
        {
            flammableObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door") || other.CompareTag("LockedDoor") ||
            other.CompareTag("Key") || other.CompareTag("LockedBox") || other.CompareTag("Note"))
        {
            isNearInteractable = false;
            interactableObject = null;
        }

        if (other.CompareTag("Upgrade")) isPlayerNearbyUpgrade = false;
        if (other.CompareTag("UpgradeStation")) isPlayerNearbyUpgradeStation = false;
        if (other.CompareTag("UpgradeMask")) isPlayerNearbyMaskUpgrade = false;
        if (other.CompareTag("Flammable") && other.gameObject == flammableObject) flammableObject = null;
    }

    void OpenDoor(GameObject door)
    {
        Animator doorAnimator = door.GetComponentInChildren<Animator>();

        if (doorAnimator != null)
        {
            bool isOpen = doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Open");
            doorAnimator.SetTrigger(isOpen ? "close" : "open");
            Debug.Log(isOpen ? "Closing the door." : "Opening the door.");
        }
        else
        {
            door.SetActive(!door.activeSelf);
        }
    }

    void InteractWithLockedDoor(GameObject lockedDoor)
    {
        LockedDoor doorScript = lockedDoor.GetComponent<LockedDoor>();
        if (doorScript != null)
        {
            // Check if the player has the correct key
            if (currentKey == doorScript.requiredKey)
            {
                doorScript.ToggleDoor(currentKey);  // Pass the key object to the door's toggle function
            }
            else
            {
                // If the player doesn't have the key, show a message or sound
                Debug.Log("You need the correct key to open this door!");
            }
        }
    }

    void PickupKey(GameObject key)
    {
        // Store the key the player picked up
        currentKey = key;
        hasKey = true; // Set the flag that the player has a key
        Debug.Log("Key picked up: " + key.name); // Log key pickup for debugging
        Destroy(key); // Remove the key object from the scene (optional)
    }

    void InteractWithLockedBox(GameObject lockedBox)
    {
        LockedBox box = lockedBox.GetComponent<LockedBox>();
        if (box != null) box.StartInteraction();
    }

    private void PickUpUpgrade()
    {
        if (upgradeObject != null)
        {
            lanternUpgradeParts++;
            Debug.Log("Upgrade part collected! Total: " + lanternUpgradeParts);
            Destroy(upgradeObject);
        }
    }

    private void UpgradeLantern()
    {
        if (lanternscript == null)
        {
            Debug.LogError("Lantern script is not assigned!");
            return;
        }

        if (lanternUpgradeParts >= 3)
        {
            lanternUpgradeParts = 0;
            lanternscript.UpgradeLantern();
        }
        else
        {
            Debug.Log("Not enough parts to upgrade! Need " + (3 - lanternUpgradeParts) + " more.");
        }
    }

    private void BurnObject()
    {
        if (flammableObject != null)
        {
            lanternscript.currentCharge -= lanternscript.burnChargeCost; // Consume charge when burning
            Debug.Log($"Burning {flammableObject.name}! Charge left: {lanternscript.currentCharge} / {lanternscript.maxCharge}");
            Destroy(flammableObject); // Remove the flammable object from the scene
            flammableObject = null; // Reset after burning
        }
        else
        {
            Debug.Log("No flammable object detected!");
        }
    }

    private void PickUpMaskUpgrade()
    {
        if (maskUpgradeObject != null)
        {
            maskUpgradeParts++;
            Debug.Log("Mask upgrade part collected! Total: " + maskUpgradeParts);
            Destroy(maskUpgradeObject);
        }
    }

    private void UpgradeMask()
    {
        if (maskScript == null)
        {
            Debug.LogError("Mask script is not assigned!");
            return;
        }

        if (maskUpgradeParts >= 3)
        {
            maskUpgradeParts = 0; // Reset collected upgrades
            maskScript.UpgradeMask();
            Debug.Log("Mask upgraded successfully!");
        }
        else
        {
            Debug.Log("Not enough parts to upgrade the mask! Need " + (3 - maskUpgradeParts) + " more.");
        }
    }
}

