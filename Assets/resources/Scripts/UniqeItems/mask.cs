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

    private bool isMaskActive = false;

    public bool IsMaskActive => isMaskActive;
    
    public float GetDepletionMultiplier()
    {
        return isMaskActive ? depletionMultiplierWhenActive : 1f;
    }

    public void ToggleMask()
    {
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
}
