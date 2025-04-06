using UnityEngine;
using System.Collections;

public class Lantern : MonoBehaviour
{
    public GameObject lanternLight; // Reference to the actual light GameObject
    public float maxCharge = 100f; // Maximum energy the lantern can hold
    public float chargeDepletionRate = 1f; // Rate at which the charge depletes per second
    public float burnInterval = 1f; // Time interval for burning objects
    public float burnChargeCost = 10f;
    
    private float currentCharge; // Tracks the current charge level
    private bool isLanternOn = false; // Is the lantern currently active?
    private bool isInFog = false; // Is the player inside a fog area?
    private bool hasBurnUpgrade = false; // Does the lantern have the burn upgrade?

    private Coroutine chargeDepletionCoroutine; // Reference to charge depletion coroutine
    private Coroutine burnCoroutine; // Reference to burn effect coroutine
    private Collider currentBurnable; // Stores the current object that can be burned
    
    public float CurrentCharge => currentCharge;
    public float BurnChargeCost => burnChargeCost;
    public bool IsLanternOn => isLanternOn;
    public bool IsUpgraded => hasBurnUpgrade;
    public event System.Action<bool> OnLanternToggled;

    void Start()
    {
        currentCharge = maxCharge;
        lanternLight.SetActive(false);
    }

    public void ToggleLantern()
    {
        if (isInFog) return; // If in fog, prevent turning on

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
            OnLanternToggled?.Invoke(true);
        }
    }

    private void TurnOffLantern()
    {
        isLanternOn = false;
        lanternLight.SetActive(false);
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
            TurnOffLantern(); // Lantern turns off automatically in fog
        }

        if (hasBurnUpgrade && other.CompareTag("Burnable"))
        {
            currentBurnable = other;
            
            // Start burning if the lantern is already on
            if (isLanternOn)
                burnCoroutine = StartCoroutine(BurnObjects());
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fog"))
        {
            isInFog = false;
        }

        if (hasBurnUpgrade && other.CompareTag("Burnable"))
        {
            currentBurnable = null; // No longer near a burnable object

            // Stop burning if the object is out of range
            if (burnCoroutine != null)
            {
                StopCoroutine(burnCoroutine);
                burnCoroutine = null;
            }
        }
    }
    
    public void ConsumeCharge(float amount)
    {
        currentCharge = Mathf.Max(0, currentCharge - amount);
    }
}

