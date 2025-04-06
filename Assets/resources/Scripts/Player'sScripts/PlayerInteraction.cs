using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactLanternKey = KeyCode.R;
    public KeyCode interactMaskKey = KeyCode.Q;
    public KeyCode useFillObjectKey = KeyCode.F;

    public TextMeshProUGUI interactionLanternText;
    public TextMeshProUGUI interactionMaskText;

    private Lantern lantern;
    private Mask mask;
    private OilBarrel oilBarrel;

    // Dictionary to map keys to actions
    private Dictionary<KeyCode, Action> keyActions;

    void Start()
    {
        interactionLanternText.gameObject.SetActive(false);
        interactionMaskText.gameObject.SetActive(false);

        // Map keys to relevant actions
        keyActions = new Dictionary<KeyCode, Action>
        {
            { interactLanternKey, () => lantern?.ToggleLantern() },
            { interactMaskKey, () => mask?.ToggleMask() },
            { useFillObjectKey, () => {
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Lantern foundLantern))
        {
            lantern = foundLantern;
            interactionLanternText.text = $"Press {interactLanternKey} to interact with Lantern";
            interactionLanternText.gameObject.SetActive(true);
        }

        if (other.TryGetComponent(out Mask foundMask))
        {
            mask = foundMask;
            interactionMaskText.text = $"Press {interactMaskKey} to interact with Mask";
            interactionMaskText.gameObject.SetActive(true);
        }

        if (other.TryGetComponent(out OilBarrel foundBarrel))
        {
            oilBarrel = foundBarrel;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Lantern _))
        {
            lantern = null;
            interactionLanternText.gameObject.SetActive(false);
        }

        if (other.TryGetComponent(out Mask _))
        {
            mask = null;
            interactionMaskText.gameObject.SetActive(false);
        }

        if (other.TryGetComponent(out OilBarrel _))
        {
            oilBarrel = null;
        }
    }
}
