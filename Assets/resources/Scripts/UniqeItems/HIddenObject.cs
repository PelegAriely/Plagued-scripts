using System.Collections;
using UnityEngine;

public class HiddenObject : MonoBehaviour
{
    private Renderer objRenderer;
    private Material objMaterial;

    public float revealRange = 5f;             // Max range for the lantern to reveal objects
    public float fullVisibilityDistance = 1f;  // Distance at which the object is fully visible
    public Transform lantern;                  // Lantern's transform
    public Lantern lanternScript;              // Lantern script to subscribe to events

    private static readonly string DissolveProperty = "_Dissolve";

    private bool isLanternOn;
    private float currentDissolve = 1f;
    
    void Start()
    {
        // Get the renderer from the child object
        objRenderer = GetComponentInChildren<Renderer>();
        
        if (objRenderer != null && objRenderer.material.HasProperty(DissolveProperty))
        {
            objMaterial = objRenderer.material;
            SetDissolve(1f); // Start fully hidden
        }
        
        if (lanternScript != null)
        {
            isLanternOn = lanternScript.IsLanternOn;
            lanternScript.OnLanternToggled += HandleLanternToggle;
        }
    }
  
    void Update()
    {
        if (isLanternOn && lantern != null)
        {
            float distance = Vector3.Distance(transform.position, lantern.position);
            float dissolveFactor = Mathf.InverseLerp(fullVisibilityDistance, revealRange, distance);
            if (!Mathf.Approximately(dissolveFactor, currentDissolve))
            {
                SetDissolve(dissolveFactor);
                currentDissolve = dissolveFactor;
            }
        }
    }

    private void HandleLanternToggle(bool state)
    {
        isLanternOn = state;

        // If lantern was turned off, instantly dissolve
        if (!state)
        {
            SetDissolve(1f);
            currentDissolve = 1f;
        }
    }
    
    private void SetDissolve(float value)
    {
        objMaterial.SetFloat(DissolveProperty, value);
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (lanternScript != null)
            lanternScript.OnLanternToggled -= HandleLanternToggle;
    }
}
