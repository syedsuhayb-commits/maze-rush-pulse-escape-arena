using UnityEngine;

public class KeyRandomSpawner : MonoBehaviour
{
    [Header("Key Object")]
    public GameObject keyPickup;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    void Start()
    {
        SpawnKeyRandomly();
    }

    public void SpawnKeyRandomly()
    {
        if (keyPickup == null)
        {
            Debug.LogError("Key Pickup is not assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No key spawn points assigned!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);

        keyPickup.transform.position = spawnPoints[randomIndex].position;
        keyPickup.transform.rotation = spawnPoints[randomIndex].rotation;

        keyPickup.SetActive(true);

        Debug.Log("Key spawned at: " + spawnPoints[randomIndex].name);
    }
}
