using UnityEngine;

public class NPC_Spawner : MonoBehaviour
{
    [SerializeField] GameObject npcPrefab;
    [SerializeField] int npcAmount = 10;
    [SerializeField] float spawnWidth = 20.0f;
    [SerializeField] float spawnHeight = 20.0f;

    void Start()
    {
        Spawn(npcPrefab, npcAmount, spawnWidth, spawnHeight);
    }

    void Spawn(GameObject obj, int amount, float width, float height)
    {
        for (int i = 0; i < npcAmount; i++)
        {
            float randomX = Random.Range(-spawnWidth / 2, spawnWidth / 2);
            float randomY = Random.Range(-spawnHeight / 2, spawnHeight / 2);

            Vector2 spawnPosition = new Vector2(randomX, randomY);
            Instantiate(obj, spawnPosition, Quaternion.identity);
        }
    }
}
