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

    void Start()
    {
        // Find the first Lantern object in the scene
        lanternscript = Object.FindFirstObjectByType<Lantern>();

        // If lanternscript is null, log an error to help with debugging
        if (lanternscript == null)
        {
            Debug.LogError("Lantern script is not found in the scene!");
        }
        maskScript = Object.FindFirstObjectByType<mask>(); // Find the mask in the scene
    }

        void Update()
    {
        // check if the player is near an interactable object and press the interaction key
        if (isNearInteractable && Input.GetKeyDown(interactionKey))
        {
            // Interact base on the type of object 
            if (interactableObject.CompareTag("Door"))
            {
                OpenDoor(interactableObject); // Call method to open/close door
            }
            else if (interactableObject.CompareTag("LockedDoor"))
            {
                InteractWithLockedDoor(interactableObject); // Call method for locked door
            }
            else if (interactableObject.CompareTag("Key"))
            {
                PickupKey(interactableObject); // Call methods to pick up key
            }
            else if (interactableObject.CompareTag("LockedBox"))
            {
                InteractWithLockedBox(interactableObject); // Call method for locked box
            }
        }
        // Picking up Upgrade Parts
        if (isPlayerNearbyUpgrade && Input.GetKeyDown(interactionKey))
        {
            PickUpUpgrade();
        }

        // Upgrading Lantern at Station
        if (isPlayerNearbyUpgradeStation && Input.GetKeyDown(interactionKey))
        {
            UpgradeLantern();
        }

        // Burn action: Only works if the lantern is upgraded, a flammable object is in range, and the lantern is on
        if (lanternscript != null && lanternscript.IsLanternOn && lanternscript.isUpgraded && flammableObject != null && Input.GetKeyDown(KeyCode.E))
        {
            BurnObject();
        }

        // Picking up Mask Upgrade Parts
        if (isPlayerNearbyMaskUpgrade && Input.GetKeyDown(interactionKey))
        {
            PickUpMaskUpgrade();
        }

        // Upgrading Mask at Station
        if (isPlayerNearbyUpgradeStation && Input.GetKeyDown(interactionKey))
        {
            UpgradeMask();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       // Check if the player is near an interactable object (use tags for categorization)
       if (other.CompareTag("Door") || other.CompareTag("LockedDoor") || other.CompareTag("Key") || other.CompareTag("LockedBox"))
        {
            isNearInteractable = true;
            interactableObject = other.gameObject;
        }

        if (other.CompareTag("Upgrade"))
        {
            upgradeObject = other.gameObject;
            isPlayerNearbyUpgrade = true;
        }

        if (other.CompareTag("UpgradeStation"))
        {
            isPlayerNearbyUpgradeStation = true;
        }

        // Detecting flammable objects only if the lantern is upgraded
        if (lanternscript != null && lanternscript.isUpgraded)
        {
            if (other.CompareTag("Flammable"))
            {
                flammableObject = other.gameObject;
                Debug.Log($"Flammable object detected: {flammableObject.name}");
            }
        }
        if (other.CompareTag("UpgradeMask"))
        {
            maskUpgradeObject = other.gameObject;
            isPlayerNearbyMaskUpgrade = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When leaving the trigger zone of an interactable object
        if (other.CompareTag("Door") || other.CompareTag("LockedDoor") || other.CompareTag("Key") || other.CompareTag("LockedBox"))
        {
            isNearInteractable = false;
            interactableObject = null;
        }

        if (other.CompareTag("Upgrade"))
        {
            isPlayerNearbyUpgrade = false;
            upgradeObject = null;
        }

        if (other.CompareTag("UpgradeStation"))
        {
            isPlayerNearbyUpgradeStation = false;
        }

        // Leaving flammable object
        if (other.CompareTag("Flammable") && other.gameObject == flammableObject)
        {
            Debug.Log($"Left flammable object: {flammableObject.name}");
            flammableObject = null;
        }
        if (other.CompareTag("UpgradeMask"))
        {
            isPlayerNearbyMaskUpgrade = false;
            maskUpgradeObject = null;
        }
    }

    void OpenDoor(GameObject door)
    {
        Animator doorAnimator = door.GetComponent<Animator>();
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Toggle");
        }
        else
        {
            Debug.LogWarning("No Animator component found on the door!");
        }
    }

    void InteractWithLockedDoor(GameObject lockedDoor)
    {
        LockedDoor doorScript = lockedDoor.GetComponent<LockedDoor>();
        if (doorScript != null)
        {
            if (currentKey == doorScript.requiredKey) // Check if player has the right key
            {
                doorScript.ToggleDoor(currentKey);  // Pass the key object to the door's function
            }
            else
            {
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
        if (box != null)
        {
            if (box != null)
            {
                box.StartInteraction();
            }
        }
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

