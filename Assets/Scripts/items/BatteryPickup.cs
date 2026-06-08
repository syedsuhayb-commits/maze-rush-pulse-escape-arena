using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                gameManager.CollectBattery();
            }

            gameObject.SetActive(false);
        }
    }
}