using UnityEngine;
using System.Collections;

public class Lantern : MonoBehaviour
{
    public GameObject lanternLight; // Reference to the actual light GameObject
    public float maxCharge = 100f; // Maximum energy the lantern can hold
    public float chargeDepletionRate = 1f; // Rate at which the charge depletes per second
    public float burnInterval = 1f; // Time interval for burning objects
    private float currentCharge; // Tracks the current charge level

    private bool isLanternOn = false; // Is the lantern currently active?
    private bool isInFog = false; // Is the player inside a fog area?
    private bool hasBurnUpgrade = false; // Does the lantern have the burn upgrade?

    private Coroutine chargeDepletionCoroutine; // Reference to charge depletion coroutine
    private Coroutine burnCoroutine; // Reference to burn effect coroutine
    private Collider currentBurnable; // Stores the current object that can be burned
    
    public event System.Action<bool> OnLanternToggled;
    // Optional helper property for external scripts
    public bool IsLanternOn => isLanternOn;

    void Start()
    {
        currentCharge = maxCharge; // Start with full charge
        lanternLight.SetActive(false); // The lantern starts off
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
            OnLanternToggled?.Invoke(true); // Broadcast event
        }
    }

    private void TurnOffLantern()
    {
        isLanternOn = false;
        lanternLight.SetActive(false);
        OnLanternToggled?.Invoke(false); // Broadcast event
    }

    public void UseRefill()
    {
        currentCharge = maxCharge; // Restore the lantern's charge
        Debug.Log("Lantern refilled.");
    }

    public void EnableBurnUpgrade()
    {
        hasBurnUpgrade = true;
        Debug.Log("Burn upgrade activated!");

        // If the lantern is already on and near a burnable object, start burning
        if (isLanternOn && currentBurnable != null)
            burnCoroutine = StartCoroutine(BurnObjects());
    }

    public void DisableBurnUpgrade()
    {
        hasBurnUpgrade = false;
        Debug.Log("Burn upgrade deactivated.");
        
        // stop the burn effect if it was active
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }
    }

    private IEnumerator DepleteCharge()
    {
        while (isLanternOn && !isInFog) // Only drain charge when lantern is on and outside fog
        {
            currentCharge -= chargeDepletionRate * Time.deltaTime;
            
            if (currentCharge <= 0)
            {
                TurnOffLantern(); // Turn off when out of charge
                yield break;
            }

            yield return null; // Wait for the next frame
        }  
    }

    private IEnumerator BurnObjects()
    {
        while (hasBurnUpgrade && isLanternOn && currentBurnable != null)
        {
            // Get the burnable object and call its Burn() method
            BurnableObject burnable = currentBurnable.GetComponent<BurnableObject>();

            if (burnable)
            {
                burnable.Burn(); // Destroy the object
                currentBurnable = null; // Reset reference since it's now gone
            }

            yield return new WaitForSeconds(burnInterval); // Wait before checking again
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
}

