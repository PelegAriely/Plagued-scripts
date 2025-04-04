using UnityEngine;

public class BurnableObject : MonoBehaviour
{
    // This function is called when the lantern burns this object.
    public void Burn()
    {
        Debug.Log($"{gameObject.name} has been burned!");
        Destroy(gameObject); // Destroy the object immediately
    }
}
