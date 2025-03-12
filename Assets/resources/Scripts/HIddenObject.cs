using System.Collections;
using UnityEngine;

public class HiddenObject : MonoBehaviour
{
    private Renderer objRenderer;
    private Color originalColor;

    public float revealRange = 5f; // Range of the lantern's light
    public Transform lantern; // Reference to the lantern's position
    public Lantern lanternScript; // Reference to the Lantern script (for checking if it's on)

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            originalColor = objRenderer.material.color;
            SetTransparency(0f); // Start fully invisible
        }
    }

    void Update()
    {
        if (lantern == null || lanternScript == null || objRenderer == null)
            return;

        float distance = Vector3.Distance(transform.position, lantern.position);

        if (lanternScript.IsLanternOn)
        {
            // calculate transparenct based on distance
            float visibilityFactor = Mathf.Clamp01(1f - (distance / revealRange));
            SetTransparency(visibilityFactor);
        }
        else
        {
            // Lantern is off, fully transparent
            SetTransparency(0f);
        }
    }


    private void SetTransparency(float alpha)
    {
        if (objRenderer != null)
        {
            Color newColor = originalColor;
            newColor.a = alpha;
            objRenderer.material.color = newColor;
        }
    }
}
