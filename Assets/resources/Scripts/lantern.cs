using UnityEngine;

public class Lantern : MonoBehaviour
{
    public GameObject lanternLight; // Reference to the lantern's light component
    public float revealRadius = 5f; // Radius in which the lantern reveals hidden objects

    [Header("Lantern Charge Settings")]
    public float maxCharge = 100f; // Maximum charge the lantern can hold
    public float chargeDepletionRate = 1f; // How quickly the lantern depletes charge (per second)

    [Header("Upgrade Settings")]
    public float upgradeExtraCharge = 50f; // How much extra charge the upgrade gives (modifiable in Inspector)
    public float burnChargeCost = 10f; // How much charge is consumed per burn action

    public float currentCharge; // Current charge of the lantern
    public bool isUpgraded = false; // Flag to check if the lantern can burn objects

    public bool IsLanternOn { get; private set; } = false; // Flag to track if the lantern is on
    private bool isInFog = false; // Whether the lantern is inside a fog collider

    void Start()
    {
        currentCharge = maxCharge; // Initialize the charge to maximum
        if (lanternLight != null)
        {
            lanternLight.SetActive(false); // Initially turn off the lantern light
        }
    }

    void Update()
    {
        // If the lantern is in the fog, disable the light and prevent turning it on
        if (isInFog)
        {
            if (lanternLight != null)
            {
                lanternLight.SetActive(false); // Turn off light when in the fog
            }
            IsLanternOn = false; // Prevent the lantern from being turned on while in the fog
        }
        else if (IsLanternOn && currentCharge > 0)
        {
            // Deplete charge while the lantern light is on
            currentCharge -= chargeDepletionRate * Time.deltaTime;
            if (currentCharge <= 0)
            {
                currentCharge = 0;
                TurnOffLantern(); // Turn off the light if the charge is depleted
            }
        }
    }

    public void TurnOnLantern()
    {
        // If not in fog and the lantern has charge, turn it on
        if (!isInFog && currentCharge > 0)
        {
            IsLanternOn = true;
            lanternLight.SetActive(true); // Turn on the lantern light
            Debug.Log("Lantern turned on"); // Log when lantern is turned on
        }
    }

    public void TurnOffLantern()
    {
        IsLanternOn = false;
        lanternLight.SetActive(false); // Turn off the lantern light
        Debug.Log("Lantern turned off"); // Log when lantern is turned off
    }

    public void RefillCharge()
    {
        currentCharge = maxCharge; // Reset charge to maximum
        Debug.Log("Lantern refilled to full charge"); // Log when the lantern is refilled
    }

    public void UpgradeLantern()
    {
        maxCharge += upgradeExtraCharge; // Add extra charge from upgrade
        isUpgraded = true; // Lantern can now burn objects
        Debug.Log($"Lantern upgraded! Max charge: {maxCharge}. Burning enabled.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fog"))
        {
            isInFog = true; // Set flag when the lantern enters fog
            TurnOffLantern(); // Turn off the lantern light when entering the fog
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fog"))
        {
            isInFog = false; // Set flag when the lantern exits fog
            if (IsLanternOn) // If the lantern was on, reactivate the light
            {
                lanternLight.SetActive(true);
            }
        }
    }
}
