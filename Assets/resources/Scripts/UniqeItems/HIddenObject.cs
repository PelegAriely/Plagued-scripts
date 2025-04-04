using System.Collections;
using UnityEngine;

public class HiddenObject : MonoBehaviour
{
    private Renderer objRenderer;
    private Material objMaterial;

    public float revealRange = 5f; // Range of the lantern's light
    public float fullVisibilityDistance = 1f; // Distance at which the object is fully visible
    public Transform lantern; // Reference to the lantern's position
    public Lantern lanternScript; // Reference to the Lantern script (for checking if it's on)
    
    private static readonly string DissolveProperty = "_Dissolve"; // Shader property name

    void Start()
    {
        // Get the renderer from the child object
        objRenderer = GetComponentInChildren<Renderer>();
        
        if (objRenderer != null && objRenderer.material.HasProperty(DissolveProperty))
        {
            objMaterial = objRenderer.material;
            SetDissolve(1f); // Start fully dissolved (invisible)
        }
    }

    void Update()
    {
        if (lantern == null || lanternScript == null || objMaterial == null)
            return;

        float distance = Vector3.Distance(transform.position, lantern.position);

        if (lanternScript.IsLanternOn)
        {
            // Calculate dissolve value based on distance (inverse of transparency logic)
            float dissolveFactor = Mathf.InverseLerp(fullVisibilityDistance, revealRange, distance);
            SetDissolve(dissolveFactor);
        }
        else
        {
            // Lantern is off, fully dissolved
            SetDissolve(1f);
        }
    }


    private void SetDissolve(float value)
    {
        if (objMaterial != null)
        {
            objMaterial.SetFloat(DissolveProperty, value);
        }
    }
}
