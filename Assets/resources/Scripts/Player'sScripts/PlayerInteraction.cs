using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode interactLanternKey = KeyCode.R;
    public KeyCode interactMaskKey = KeyCode.Q;
    public KeyCode useFillObjectKey = KeyCode.F;

    [Header("Breathing Settings")]
    public float baseBreathDepletionRate = 10f;
    public float breathRegenRate = 5f;

    private float currentBreath = 100f;
    private float maxBreath = 100f;
    private bool isInFog = false;
    private bool hasTeleported = false;

    [Header("References")]
    private Lantern lantern;
    private Mask mask;
    private OilBarrel oilBarrel;

    private Dictionary<KeyCode, Action> keyActions;
    private GameObject lastHighlightedObject = null;

    private Transform lastSavePoint;
    private CharacterController characterController;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        // Map keys to relevant actions
        keyActions = new Dictionary<KeyCode, Action>
        {
            { interactLanternKey, () => lantern?.ToggleLantern() },
            { interactMaskKey, () => mask?.ToggleMask() },
            { useFillObjectKey, () =>
            {
                if (oilBarrel != null)
                    oilBarrel.Refill();
                else
                    lantern?.UseRefill();
            }}
        };
    }

    void Update()
    {
        // Check for key presses and invoke the mapped action
        foreach (var keyAction in keyActions)
        {
            if (Input.GetKeyDown(keyAction.Key))
            {
                keyAction.Value?.Invoke();
                break; // Exit loop after one action per frame
            }
        }
        HandleBreathing();
    }

    private void HandleBreathing()
    {
        if (isInFog)
        {
            float multiplier = mask != null ? mask.GetDepletionMultiplier() : 1f;
            float rate = baseBreathDepletionRate * multiplier;

            currentBreath = Mathf.Max(0f, currentBreath - rate * Time.deltaTime);
            Debug.Log($"[Breath] Current: {currentBreath:0.00} (Multiplier: {multiplier})");

            if (currentBreath <= 0f && !hasTeleported)
            {
                Debug.LogWarning("⚠️ Out of breath!");

                if (lastSavePoint != null)
                {
                    Debug.Log("🧭 Teleporting to last save point...");

                    // Disable controller before teleporting
                    characterController.enabled = false;
                    transform.position = lastSavePoint.position;
                    characterController.enabled = true;

                    // Optional: Reset momentum if using rigidbody (not needed for CharacterController)
                    // GetComponent<Rigidbody>()?.velocity = Vector3.zero;

                    currentBreath = maxBreath;
                    hasTeleported = true;
                    isInFog = false;

                    Debug.Log("✅ Teleport successful.");
                }
                else
                {
                    Debug.LogWarning("🚫 No save point set!");
                }
            }
        }
        else
        {
            if (currentBreath < maxBreath)
            {
                currentBreath = Mathf.Min(maxBreath, currentBreath + breathRegenRate * Time.deltaTime);
                Debug.Log($"[Breath Regenerating] Current: {currentBreath:0.00}");
            }

            if (hasTeleported && currentBreath > 0f)
            {
                hasTeleported = false;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fog"))
        {
            isInFog = true;
            Debug.Log("🌫️ Entered fog.");
        }

        if (other.TryGetComponent(out Lantern foundLantern))
        {
            lantern = foundLantern;
            HighlightObject(other.gameObject);
        }

        if (other.TryGetComponent(out Mask foundMask))
        {
            mask = foundMask;
            HighlightObject(other.gameObject);
        }

        if (other.TryGetComponent(out OilBarrel foundBarrel))
        {
            oilBarrel = foundBarrel;
            HighlightObject(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fog"))
        {
            isInFog = false;
            Debug.Log("🌤️ Exited fog.");
        }

        if (other.TryGetComponent(out Lantern _))
        {
            RemoveHighlight(other.gameObject);
            lantern = null;
        }

        if (other.TryGetComponent(out Mask _))
        {
            RemoveHighlight(other.gameObject);
            mask = null;
        }

        if (other.TryGetComponent(out OilBarrel _))
        {
            RemoveHighlight(other.gameObject);
            oilBarrel = null;
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
    
    public void SetLastSavePoint(Transform savePoint)
    {
        lastSavePoint = savePoint;
        Debug.Log($"💾 Save point updated to: {savePoint.name}");
    }
}
