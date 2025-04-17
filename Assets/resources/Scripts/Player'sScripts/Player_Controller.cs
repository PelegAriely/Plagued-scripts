using System;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E;

    [Header("Lantern & Upgrade Systems")]
    [SerializeField] private Lantern lanternScript;
    [SerializeField] private LanternUpgradeSystem lanternUpgradeSystem;

    [Header("Note Interaction")]
    [SerializeField] private NoteInteraction noteInteraction;

    private GameObject interactableObject = null;
    private GameObject flammableObject = null;
    private GameObject currentKey = null;
    private CombinationLock activeCombinationLock = null;

    private bool isAtUpgradeStation = false;
    private int lanternUpgradeParts = 0;

    private Dictionary<string, Action<GameObject>> interactionMap;
    private GameObject lastHighlightedObject = null;
    private PuzzleObject heldPuzzleObject = null;
    [SerializeField] private Transform handSlot; // empty child object in front of player
    private Collider currentPillarCollider = null;

    void Start()
    {
        if (lanternScript == null)
            lanternScript = FindObjectOfType<Lantern>();

        if (noteInteraction == null)
            noteInteraction = FindObjectOfType<NoteInteraction>();

        if (lanternUpgradeSystem == null)
            lanternUpgradeSystem = FindObjectOfType<LanternUpgradeSystem>();

        interactionMap = new Dictionary<string, Action<GameObject>>
        {
            { "Door", OpenDoor },
            { "LockedDoor", InteractWithLockedDoor },
            { "Key", PickupKey },
            { "LockedBox", InteractWithLockedBox },
            { "PuzzleObject", InteractWithPuzzleObject },
            { "CombinationLock", InteractWithCombinationLock },
            { "Note", obj => noteInteraction?.HandleNoteInteraction(obj) },
            { "Upgrade", PickUpUpgrade },
            { "UpgradeStation", obj => UpgradeLantern() }
        };
    }

    void Update()
    {
        if (!Input.GetKeyDown(interactionKey)) return;

        // ✅ If holding an object, prioritize dropping or placing
        if (heldPuzzleObject != null)
        {
            HandleHeldObjectInteraction();
            return;
        }

        // Normal interaction
        if (interactableObject != null && interactionMap.TryGetValue(interactableObject.tag, out var action))
        {
            action.Invoke(interactableObject);
        }
        else if (CanBurnFlammable())
        {
            BurnObject();
        }
    }

    private bool CanBurnFlammable()
    {
        return flammableObject != null &&
               lanternScript != null &&
               lanternScript.IsLanternOn &&
               lanternScript.IsUpgraded; // ✅ Now accessible
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isInteractable = interactionMap.ContainsKey(other.tag) && other.tag != "Placed";
        bool hasOutline = other.GetComponent<Outline>() != null;

        if (isInteractable || (hasOutline && other.GetComponent<Pillar>() != null && heldPuzzleObject != null))
        {
            interactableObject = other.gameObject;
            HighlightObject(interactableObject);
        }
        else if (heldPuzzleObject == null && (isInteractable || hasOutline))
        {
            interactableObject = other.gameObject;
            HighlightObject(interactableObject);
        }

        if (other.CompareTag("Flammable") && lanternScript?.IsUpgraded == true)
            flammableObject = other.gameObject;

        if (other.CompareTag("UpgradeStation"))
            isAtUpgradeStation = true;

        if (other.GetComponent<Pillar>() != null)
            currentPillarCollider = other;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == interactableObject && heldPuzzleObject == null)
        {
            RemoveHighlight(interactableObject);
            interactableObject = null;
        }

        if (other.gameObject == flammableObject)
            flammableObject = null;

        if (other.CompareTag("UpgradeStation"))
            isAtUpgradeStation = false;

        if (other == currentPillarCollider)
            currentPillarCollider = null;
    }
    
    private void HandleHeldObjectInteraction()
    {
        // Try to place on pillar if in one
        if (currentPillarCollider != null)
        {
            Pillar pillar = currentPillarCollider.GetComponent<Pillar>();
            if (pillar != null && !pillar.HasObject)
            {
                heldPuzzleObject.PlaceOnPillar(pillar);
                heldPuzzleObject = null;
                return;
            }
        }

        // Otherwise, drop it at player's feet
        Vector3 dropPos = transform.position + transform.forward + Vector3.up * 0.5f;
        heldPuzzleObject.Drop(dropPos);
        heldPuzzleObject = null;
        
        if (interactableObject != null)
        {
            RemoveHighlight(interactableObject);
            interactableObject = null;
        }
    }

    // --- Interaction Logic Methods ---

    private void OpenDoor(GameObject door)
    {
        Animator animator = door.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            bool isOpen = animator.GetCurrentAnimatorStateInfo(0).IsName("Open");
            animator.SetTrigger(isOpen ? "close" : "open");
        }
        else
        {
            door.SetActive(!door.activeSelf);
        }
    }

    private void InteractWithLockedDoor(GameObject lockedDoor)
    {
        var script = lockedDoor.GetComponent<LockedDoor>();
        if (script != null && currentKey == script.requiredKey)
        {
            script.ToggleDoor(currentKey);
        }
        else
        {
            Debug.Log("You need the correct key!");
        }
    }

    private void PickupKey(GameObject key)
    {
        currentKey = key;
        Destroy(key);
        Debug.Log("Key picked up.");
    }

    private void InteractWithLockedBox(GameObject lockedBox)
    {
        lockedBox.GetComponent<LockedBox>()?.StartInteraction();
    }
    
    private void InteractWithCombinationLock(GameObject obj)
    {
        CombinationLock lockScript = obj.GetComponent<CombinationLock>();
        if (lockScript == null || lockScript.IsUnlocked()) return;


        if (activeCombinationLock == lockScript)
        {
            lockScript.StopInteraction();
            activeCombinationLock = null;
        }
        else
        {
            if (activeCombinationLock != null)
            {
                activeCombinationLock.StopInteraction();
            }

            lockScript.StartInteraction();
            activeCombinationLock = lockScript;
        }
    }
    

    private void PickUpUpgrade(GameObject upgradeObj)
    {
        lanternUpgradeParts++;
        Destroy(upgradeObj);
        Debug.Log($"Upgrade part collected! Total: {lanternUpgradeParts}");
    }

    private void UpgradeLantern()
    {
        if (!isAtUpgradeStation) return;

        if (lanternUpgradeParts >= 3)
        {
            lanternUpgradeParts = 0;
            lanternScript?.UpgradeLantern();
            Debug.Log("Lantern upgraded!");
        }
        else
        {
            Debug.Log($"Need {3 - lanternUpgradeParts} more parts to upgrade.");
        }
    }

    private void BurnObject()
    {
        if (flammableObject != null)
        {
            lanternScript.ConsumeCharge(lanternScript.BurnChargeCost);
            Debug.Log($"Burning {flammableObject.name}, charge left: {lanternScript.CurrentCharge}");
            Destroy(flammableObject);
            flammableObject = null;
        }
    }
    
    private void HighlightObject(GameObject obj)
    {
        if (lastHighlightedObject != null && lastHighlightedObject != obj)
        {
            RemoveHighlight(lastHighlightedObject);
        }

        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
            lastHighlightedObject = obj;
        }
    }

    private void RemoveHighlight(GameObject obj)
    {
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }

        if (lastHighlightedObject == obj)
        {
            lastHighlightedObject = null;
        }
    }
    
    private void InteractWithPuzzleObject(GameObject obj)
    {
        if (PuzzleManager.Instance.IsPuzzleSolved()) return;

        if (heldPuzzleObject == null)
        {
            heldPuzzleObject = obj.GetComponent<PuzzleObject>();
            if (heldPuzzleObject != null)
            {
                heldPuzzleObject.PickUp(handSlot);
                Debug.Log($"Picked up {heldPuzzleObject.name}");

                // Clear highlight since we can't interact while holding
                RemoveHighlight(interactableObject);
                interactableObject = null;
            }
        }
        else
        {
            if (currentPillarCollider != null)
            {
                Pillar pillar = currentPillarCollider.GetComponent<Pillar>();
                if (pillar != null)
                {
                    if (!pillar.HasObject)
                    {
                        heldPuzzleObject.PlaceOnPillar(pillar);
                        heldPuzzleObject = null;
                        return;
                    }
                }
            }

            Vector3 dropPos = transform.position + transform.forward + Vector3.up * 0.5f;
            heldPuzzleObject.Drop(dropPos);
            heldPuzzleObject = null;
        }
    }
}
