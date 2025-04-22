using UnityEngine;

public class SavePoint : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         var interaction = other.GetComponent<PlayerInteraction>();
         if (interaction != null)
         {
            interaction.SetLastSavePoint(transform);
            Debug.Log("💾 Save point updated!");
         }
      }
   }
}
