using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Mask : MonoBehaviour
{
    [Header("Depletion Multiplier Settings")]
    public float depletionMultiplierWhenActive = 0.1f;

    [Header("Mask Position Transforms")]
    public Transform maskOnPosition;
    public Transform maskOffPosition;

    [Header("3D Interaction Text")]
    public TextMeshPro interactionText;

    private bool isMaskActive = false;

    public bool IsMaskActive => isMaskActive;

    void Start()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }
    
    public float GetDepletionMultiplier()
    {
        return isMaskActive ? depletionMultiplierWhenActive : 1f;
    }

    public void ToggleMask()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
        if (isMaskActive)
            TurnOffMask();
        else
            TurnOnMask();
    }
    
    private void TurnOnMask()
    {
        isMaskActive = true;
        Debug.Log("Mask is now active.");
        MoveToOnPosition();
    }

    private void TurnOffMask()
    {
        isMaskActive = false;
        Debug.Log("Mask is now inactive.");
        MoveToOffPosition();
    }
    
    private void MoveToOnPosition()
    {
        if (maskOnPosition != null)
        {
            transform.SetParent(maskOnPosition);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    private void MoveToOffPosition()
    {
        if (maskOffPosition != null)
        {
            transform.SetParent(maskOffPosition);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && interactionText != null)
        {
            interactionText.text = "Press Q to interact with Mask";
            interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }
}
