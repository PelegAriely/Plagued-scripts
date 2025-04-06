using UnityEngine;

public class LanternUpgradeSystem : MonoBehaviour
{
    [SerializeField] private Lantern lantern;
    [SerializeField] private int partsRequired = 3;
    
    private int collectedParts = 0;
    
    public void CollectPart()
    {
        collectedParts++;
        Debug.Log($"Lantern upgrade part collected. ({collectedParts}/{partsRequired})");
    }
    
    public void TryUpgrade()
    {
        if (collectedParts >= partsRequired)
        {
            collectedParts = 0;
            lantern.UpgradeLantern();
            Debug.Log("Lantern upgraded!");
        }
        else
        {
            Debug.Log($"Need {partsRequired - collectedParts} more parts to upgrade.");
        }
    }
}
