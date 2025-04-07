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

    private bool isAtUpgradeStation = false;
    private int lanternUpgradeParts = 0;

    private Dictionary<string, Action<GameObject>> interactionMap;
    private GameObject lastHighlightedObject = null;

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
            { "Note", obj => noteInteraction?.HandleNoteInteraction(obj) },
            { "Upgrade", PickUpUpgrade },
            { "UpgradeStation", obj => UpgradeLantern() }
        };
    }

    void Update()
    {
        if (!Input.GetKeyDown(interactionKey)) return;

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
        if (interactionMap.ContainsKey(other.tag))
        {
            interactableObject = other.gameObject;
            HighlightObject(interactableObject);
        }

        if (other.CompareTag("Flammable") && lanternScript?.IsUpgraded == true)
            flammableObject = other.gameObject;

        if (other.CompareTag("UpgradeStation"))
            isAtUpgradeStation = true;
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == interactableObject)
        {
            RemoveHighlight(interactableObject);
            interactableObject = null;
        }

        if (other.gameObject == flammableObject)
            flammableObject = null;

        if (other.CompareTag("UpgradeStation"))
            isAtUpgradeStation = false;
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
}
