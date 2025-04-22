using UnityEngine;

public class TriggerSpawner : MonoBehaviour
{
    public GameObject prefab;
    public Transform startPoint;
    public Vector3 offset = new Vector3(1, 0, 0);
    public int count = 5;
    private bool hasSpawned = false;  // Kontroluje, jestli už byly spawnuty objekty

    void OnTriggerEnter(Collider other)
    {
        // Zkontroluj, jestli do trigger zóny vstoupí hráč (předpokládáme tag "Player")
        if (other.CompareTag("Player") && !hasSpawned)
        {
            SpawnObjects();
            hasSpawned = true;  // Zajistí, že spawnování proběhne jen jednou
        }
    }

    void SpawnObjects()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = startPoint.position + i * offset;
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }
}
