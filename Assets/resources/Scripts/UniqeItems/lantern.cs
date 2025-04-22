using UnityEngine;
using System.Collections;
using TMPro;

public class Lantern : MonoBehaviour
{
    [Header("Lantern References")]
    public GameObject lanternLight;
    public Transform lanternOnPosition;   // ✅ Position when active
    public Transform lanternOffPosition;  // ✅ Position when inactive

    [Header("Charge Settings")]
    public float maxCharge = 100f;
    public float chargeDepletionRate = 1f;
    public float burnInterval = 1f;
    public float burnChargeCost = 10f;

    [Header("3D Interaction Text")]
    public TextMeshPro interactionText;

    private float currentCharge;
    public bool isLanternOn = false;
    private bool isInFog = false;
    private bool hasBurnUpgrade = false;

    private Coroutine chargeDepletionCoroutine;
    private Coroutine burnCoroutine;
    private Collider currentBurnable;

    public float CurrentCharge => currentCharge;
    public float BurnChargeCost => burnChargeCost;
    public bool IsLanternOn => isLanternOn;
    public bool IsUpgraded => hasBurnUpgrade;
    public event System.Action<bool> OnLanternToggled;

    void Start()
    {
        currentCharge = maxCharge;
        lanternLight.SetActive(false);

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    public void ToggleLantern()
    {
        if (isInFog) return; // If in fog, prevent turning on
        
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

        if (isLanternOn)
            TurnOffLantern();
        else
            TurnOnLantern();
    }

    private void TurnOnLantern()
    {
        if (currentCharge > 0)
        {
            isLanternOn = true;
            lanternLight.SetActive(true);
            MoveToOnPosition();
            OnLanternToggled?.Invoke(true);
        }
    }

    private void TurnOffLantern()
    {
        isLanternOn = false;
        lanternLight.SetActive(false);
        MoveToOffPosition();
        OnLanternToggled?.Invoke(false);
    }

    public void UseRefill()
    {
        currentCharge = maxCharge; // Restore the lantern's charge
        Debug.Log("Lantern refilled.");
    }
    
    public void UpgradeLantern()
    {
        EnableBurnUpgrade();
    }

    public void EnableBurnUpgrade()
    {
        hasBurnUpgrade = true;
        Debug.Log("Burn upgrade activated!");

        if (isLanternOn && currentBurnable != null)
            burnCoroutine = StartCoroutine(BurnObjects());
    }

    public void DisableBurnUpgrade()
    {
        hasBurnUpgrade = false;
        Debug.Log("Burn upgrade deactivated.");

        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }
    }

    private IEnumerator DepleteCharge()
    {
        while (isLanternOn && !isInFog)
        {
            currentCharge -= chargeDepletionRate * Time.deltaTime;

            if (currentCharge <= 0)
            {
                TurnOffLantern();
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator BurnObjects()
    {
        while (hasBurnUpgrade && isLanternOn && currentBurnable != null)
        {
            BurnableObject burnable = currentBurnable.GetComponent<BurnableObject>();

            if (burnable)
            {
                burnable.Burn();
                currentBurnable = null;
            }

            yield return new WaitForSeconds(burnInterval);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fog"))
        {
            isInFog = true;
            TurnOffLantern();
        }

        if (hasBurnUpgrade && other.CompareTag("Burnable"))
        {
            currentBurnable = other;
            if (isLanternOn)
                burnCoroutine = StartCoroutine(BurnObjects());
        }

        if (other.CompareTag("Player") && interactionText != null)
        {
            interactionText.text = "Press R to interact with Lantern";
            interactionText.gameObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fog")) isInFog = false;

        if (hasBurnUpgrade && other.CompareTag("Burnable"))
        {
            currentBurnable = null;
            if (burnCoroutine != null)
            {
                StopCoroutine(burnCoroutine);
                burnCoroutine = null;
            }
        }

        if (other.CompareTag("Player") && interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }
    
    public void ConsumeCharge(float amount)
    {
        currentCharge = Mathf.Max(0, currentCharge - amount);
    }
    
    private void MoveToOnPosition()
    {
        if (lanternOnPosition != null)
        {
            transform.SetParent(lanternOnPosition);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    private void MoveToOffPosition()
    {
        if (lanternOffPosition != null)
        {
            transform.SetParent(lanternOffPosition);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}

