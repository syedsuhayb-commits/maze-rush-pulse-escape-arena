using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Door Controller")]
    public ExitDoorController exitDoorController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                gameManager.CollectBattery();
            }

            if (exitDoorController != null)
            {
                exitDoorController.UnlockDoor();
            }

            gameObject.SetActive(false);
        }
    }
}