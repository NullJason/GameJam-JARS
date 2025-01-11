using UnityEngine;

public class ConnectToMap : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D other) {
      if(other != null && MapManager.Instance != null) MapManager.Instance.OnPlrCollided(other);
   }
}
