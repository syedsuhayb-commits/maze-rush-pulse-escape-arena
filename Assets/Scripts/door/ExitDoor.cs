using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager == null)
            {
                Debug.LogWarning("GameManager is not assigned to ExitDoor.");
                return;
            }

            if (gameManager.HasBattery())
            {
                gameManager.WinGame();
            }
            else
            {
                gameManager.ShowMessage("Find the Battery First!");
            }
        }
    }
}