using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject enemyPrefab;  // Prefab của enemy
    public Transform spawnLocation; // Vị trí spawn (có thể là chính object này)
    public int enemyCount = 3;      // Số lượng enemy spawn ra
    private bool hasSpawned = false; // Tránh spawn nhiều lần

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasSpawned)
        {
            Debug.Log("Player entered spawn area! Spawning enemies...");
            SpawnEnemies();
            hasSpawned = true; // Đánh dấu đã spawn
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Instantiate(enemyPrefab, spawnLocation.position, Quaternion.identity);
            Debug.Log("Enemy spawned at: " + spawnLocation.position);
        }
    }
}
